using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

public class World : IDebugRowProvider
{
    private Effect effect;

    private Dictionary<Vector3Int, Chunk> chunks = new();
    private ConcurrentBag<Chunk> chunksRenderable = new();

    public void Update(Player player)
    {
        for (int x = -3; x <= 3; x++)
        {
            for (int z = -3; z <= 3; z++)
            {
                var coordinate = player.ChunkCoordinate + new Vector3Int(x, 0, z);

                if (chunks.ContainsKey(coordinate))
                    continue;

                var chunk = new Chunk(coordinate);

                chunks.Add(coordinate, chunk);

                Task.Run(() =>
                {
                    chunk.Generate();
                    chunksRenderable.Add(chunk);
                });
            }
        }
    }

    public void SetEffect(Effect effect)
        => this.effect = effect;

    public IEnumerable<ChunkMesh> GetChunkMeshes()
    {
        foreach (var chunk in chunksRenderable)
        {
            if (chunk.Mesh.IsEmpty)
                continue;

            yield return chunk.Mesh;
        }
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"ChunksGenerated: {chunks.Count}";
    }
}