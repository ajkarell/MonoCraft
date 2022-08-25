using Microsoft.Xna.Framework;

public class Chunk
{
    public const int SIZE = 32;
    public const int SIZE_SQUARED = SIZE * SIZE;
    public const int SIZE_CUBED = SIZE * SIZE * SIZE;

    public Vector3Int Coordinate { get; init; }
    public Vector3 WorldPosition { get; init; }
    public BlockType[] Blocks { get; private set; }

    public ChunkMesh Mesh { get; private set; }

    public Chunk(Vector3Int coordinate)
    {
        Coordinate = coordinate;
        WorldPosition = coordinate * SIZE;
    }

    public void Generate()
    {
        Blocks = WorldGenerator.GenerateChunkBlocks(Coordinate);
        Mesh = ChunkMeshGenerator.GenerateChunkMesh(this);
    }

    public static int Index(int x, int y, int z)
        => y * SIZE_SQUARED + z * SIZE + x;
}