using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class World : GameComponent, IDebugRowProvider
{
    private Effect effect;

    private Dictionary<Vector3Int, Chunk> chunks = new();
    private List<Chunk> chunksRenderable = new();

    public World(Game game) : base(game)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                var coordinate = new Vector3Int(x, 0, z);
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

    public override void Initialize()
    {
        base.Initialize();
    }

    public void SetEffect(Effect effect)
        => this.effect = effect;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public IEnumerable<ChunkMesh> GetChunkMeshes()
        => chunksRenderable.Select(x => x.Mesh);

    public IEnumerable<string> GetDebugRows()
    {
        yield return $"ChunksGenerated: {chunks.Count}";
    }
}