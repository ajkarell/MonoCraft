using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonoCraft;

public class World : IDebugRowProvider
{
    private readonly Dictionary<Vector3Int, Chunk> chunks = new();
    private readonly Player player;

    private IEnumerable<Chunk> chunksInView;

    private bool isGenerating = false;

    public World(Player player)
    {
        this.player = player;
        this.player.OnWorldScanThresholdCrossed += ScanChunks;
        this.player.OnViewChanged += Update;

        ScanChunks();

        var chunksOrderedByDistance = chunks.Values
            .OrderBy(chunk => (chunk.WorldPosition - player.Position).LengthSquared());

        Task.Run(() =>
        {
            foreach (var chunk in chunksOrderedByDistance)
                chunk.Generate();
        });

        Update();
    }

    void ScanChunks()
    {
        var renderDistance = Settings.RenderDistanceChunks * Chunk.SIZE;

        bool IsInsideRenderDistance(Vector3 worldPosition)
        {
            return (player.Position - worldPosition).LengthSquared() <= renderDistance * renderDistance;
        }

        foreach (var (coordinate, chunk) in chunks)
        {
            if (!IsInsideRenderDistance(chunk.WorldPosition) && chunk.State == ChunkState.FullyGenerated)
            {
                chunks[coordinate] = null;
                chunks.Remove(coordinate);
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

                    chunks.Add(coordinate, new Chunk(coordinate));
                }
            }
        }
    }

    public void Update()
    {
        chunksInView = chunks.Values
            .Where(chunk => player.ViewFrustum.Intersects(chunk.BoundingBox))
            .OrderByDescending(chunk => (chunk.WorldPosition - player.Position).LengthSquared()) // order like this to fix transparency issues between chunks
            .ToList(); // explicit ToList() to avoid collection changes during loop

        if(!isGenerating)
        {
            Task.Run(() =>
            {
                isGenerating = true;

                foreach (var chunk in chunksInView.Reverse())
                {
                    if (chunk.State == ChunkState.NotGenerated)
                        chunk.Generate();
                }

                isGenerating = false;
            });
        }
    }

    public IEnumerable<ChunkMesh> GetChunkMeshesDueRender()
    {
        foreach (var chunk in chunksInView)
        {
            if (chunk.IsMeshEmpty)
                continue;

            yield return chunk.Mesh;
        }
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"Chunks in memory: {chunks.Count} ({chunks.Values.Where(chunk => !chunk.IsMeshEmpty).Count()} with mesh)";
        yield return $"Chunks in view: {chunksInView?.Count() ?? 0}";
    }
}