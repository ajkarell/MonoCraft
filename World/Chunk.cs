using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

public class Chunk
{
    public const int SIZE = 32;
    public const int SIZE_SQUARED = SIZE * SIZE;
    public const int SIZE_CUBED = SIZE * SIZE * SIZE;

    public Vector3Int Coordinate { get; init; }
    public Vector3 WorldPosition { get; init; }
    public BlockType[] Blocks { get; private set; }

    public ChunkMesh Mesh  = null;

    public BoundingBox BoundingBox { get; private set; }

    public Chunk(Vector3Int coordinate)
    {
        Coordinate = coordinate;
        WorldPosition = coordinate * SIZE;
        BoundingBox = new BoundingBox(WorldPosition, WorldPosition + new Vector3(SIZE, SIZE, SIZE));
    }

    public void Generate()
    {
        Blocks = TerrainGenerator.GenerateChunkBlocks(Coordinate);
        Mesh = ChunkMeshGenerator.GenerateChunkMesh(this);
    }

    public bool IsInViewOfPlayer(Player player)
        => player.IsBoundingBoxInView(BoundingBox);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Index(int x, int y, int z)
        => y * SIZE_SQUARED + z * SIZE + x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Index(Vector3Int position)
        => position.Y * SIZE_SQUARED + position.Z * SIZE + position.X;

    public void Destroy()
    {
        Blocks = null;
        Mesh.Destroy();
        Mesh = null;
    }
}