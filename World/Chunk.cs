using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace MonoCraft;

public enum ChunkState
{
    NotGenerated,
    Generating,
    FullyGenerated
}

public class Chunk
{
    public const int SIZE = 32;
    public const int SIZE_SQUARED = SIZE * SIZE;
    public const int SIZE_CUBED = SIZE * SIZE * SIZE;

    public Vector3Int Coordinate { get; init; }
    public Vector3 WorldPosition { get; init; }
    public BlockType[] Blocks { get; private set; }

    public ChunkMesh Mesh { get; private set; } = null;

    public BoundingBox BoundingBox { get; private set; }

    public ChunkState State { get; private set; } = ChunkState.NotGenerated;

    public bool IsMeshEmpty => Mesh?.IsEmpty ?? true;

    public Chunk(Vector3Int coordinate)
    {
        Coordinate = coordinate;
        WorldPosition = coordinate * SIZE;
        BoundingBox = new BoundingBox(WorldPosition, WorldPosition + new Vector3(SIZE, SIZE, SIZE));
    }

    public void Generate()
    {
        State = ChunkState.Generating;

        Blocks = TerrainGenerator.GenerateChunkBlocks(Coordinate);
        Mesh = ChunkMeshGenerator.GenerateChunkMesh(this);

        State = ChunkState.FullyGenerated;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Index(int x, int y, int z)
        => y * SIZE_SQUARED + z * SIZE + x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Index(Vector3Int position)
        => position.Y * SIZE_SQUARED + position.Z * SIZE + position.X;

    public void Destroy()
    {
        if (State != ChunkState.FullyGenerated)
            return;

        Blocks = null;
        Mesh?.Destroy();
        Mesh = null;
    }
}