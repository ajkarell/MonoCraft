using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonoCraft;

public class World : IDebugRowProvider
{
    private readonly Dictionary<Vector3Int, Chunk> chunks = new();
    private readonly Player player;

    public World(Player player)
    {
        this.player = player;
        this.player.OnWorldGenThresholdCrossed += () => GenerateChunks();

        GenerateChunks();
    }

    void GenerateChunks()
    {
        var renderDistance = Settings.RenderDistanceChunks * Chunk.SIZE;
        var chunksToBeGenerated = new List<Chunk>();
        var chunksToBeRemoved = new List<Chunk>();

        bool IsInsideRenderDistance(Vector3 worldPosition)
        {
            return (player.Position - worldPosition).LengthSquared() <= renderDistance * renderDistance;
        }

        foreach (var (coordinate, chunk) in chunks)
        {
            if (!IsInsideRenderDistance(chunk.WorldPosition))
            {
                chunksToBeRemoved.Add(chunk);
            }
        }

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    var coordinate = player.ChunkCoordinate + new Vector3Int(x, y, z);
                    var worldPosition = coordinate * Chunk.SIZE;

                    if (!IsInsideRenderDistance(worldPosition))
                        continue;

                    if (chunks.ContainsKey(coordinate))
                        continue;

                    var chunk = new Chunk(coordinate);

                    chunksToBeGenerated.Add(chunk);
                }
            }
        }

        var chunksOrderedByDistance = chunksToBeGenerated.OrderBy(chunk => (chunk.WorldPosition - player.Position).LengthSquared());

        Parallel.ForEach(chunksToBeGenerated, new ParallelOptions { MaxDegreeOfParallelism = 5 }, chunk =>
        {
            chunk.Generate();
        });

        foreach (var chunk in chunksToBeRemoved)
        {
            chunk.Destroy();
            chunks.Remove(chunk.Coordinate);
        }

        foreach (var chunk in chunksToBeGenerated)
        {
            chunks.Add(chunk.Coordinate, chunk);
        }
    }

    public IEnumerable<ChunkMesh> GetChunkMeshesDueRender()
    {
        foreach (var (_, chunk) in chunks)
        {
            if (chunk.Mesh == null)
                continue;

            if (chunk.Mesh.IsEmpty)
                continue;

            if (Settings.UseFrustumCulling && !player.ViewFrustum.Intersects(chunk.BoundingBox))
                continue;

            yield return chunk.Mesh;
        }
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"Chunks in memory: {chunks.Count}";
    }
}