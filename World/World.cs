using System.Threading.Tasks;

public class World : IDebugRowProvider
{
    private Dictionary<Vector3Int, Chunk> chunks = new();
    private List<ChunkMesh> chunkMeshes = new();
    private static object _chunkMeshesLock = new object();

    public World(Player player)
    {
        player.OnChunkCoordinateChanged += () => GenerateChunks(player);

        GenerateChunks(player);
    }

    void GenerateChunks(Player player)
    {
        var renderDistance = Settings.RenderDistanceChunks;
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                var coordinate = player.ChunkCoordinate + new Vector3Int(x, 0, z);

                if (chunks.ContainsKey(coordinate))
                    continue;

                var chunk = new Chunk(coordinate);

                chunks.Add(coordinate, chunk);

                Task.Run(() =>
                {
                    chunk.Generate();
                    lock (_chunkMeshesLock)
                    {
                        chunkMeshes.Add(chunk.Mesh);
                    }
                });
            }
        }
    }

    public IEnumerable<ChunkMesh> GetChunkMeshes()
    {
        lock (_chunkMeshesLock)
        {
            foreach (var mesh in chunkMeshes)
            {
                if (mesh.IsEmpty)
                    continue;

                yield return mesh;
            }
        }
    }

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"ChunksGenerated: {chunks.Count}";
    }
}