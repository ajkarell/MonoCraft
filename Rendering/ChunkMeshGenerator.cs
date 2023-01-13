using Microsoft.Xna.Framework;

namespace MonoCraft;

public static class ChunkMeshGenerator
{
    static readonly Vector3 FORWARD = Vector3.Forward;
    static readonly Vector3 UP = Vector3.Up;
    static readonly Vector3 RIGHT = Vector3.Right;
    static readonly Vector3 ZERO = Vector3.Zero;

    public static readonly Vector3[] BlockNormals =
    {
        -RIGHT ,    // LEFT   0
        RIGHT,      // RIGHT  1
        -UP,        // DOWN   2
        UP,         // UP     3
        -FORWARD,   // BACK   4
        FORWARD,    // FRONT  5
    };

    public static readonly Vector3[][] BlockPositionsBySide = {
        new[] {   UP,                     ZERO,               FORWARD,            UP + FORWARD        },  // LEFT   0
        new[] {   RIGHT + UP + FORWARD,   RIGHT + FORWARD,    RIGHT,              RIGHT + UP          },  // RIGHT  1
        new[] {   RIGHT,                  RIGHT + FORWARD,    FORWARD,            ZERO                },  // DOWN   2
        new[] {   RIGHT + UP + FORWARD,   RIGHT + UP,         UP,                 UP + FORWARD        },  // UP     3
        new[] {   RIGHT + UP,             RIGHT,              ZERO,               UP                  },  // BACK   4
        new[] {   UP+FORWARD,             FORWARD,            RIGHT+FORWARD,      RIGHT + UP + FORWARD},  // FRONT  5
    };

    public static ChunkMesh GenerateChunkMesh(Chunk chunk)
    {
        var chunkMesh = new ChunkMesh(chunk.WorldPosition);

        for (int y = 0; y < Chunk.SIZE; y++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    var blockType = chunk.Blocks[Chunk.Index(x, y, z)];

                    if (!blockType.IsOpaque())
                    {
                        if (blockType.IsLiquid())
                        {
                            chunkMesh.AddFace(new Vector3(x, y, z), blockType, Direction.Up, new(0.0f, -0.2f, 0.0f));
                            chunkMesh.AddFace(new Vector3(x, y, z), blockType, Direction.Down, new(0.0f, 0.8f, 0.0f));
                        }

                        continue;
                    }

                    var position = new Vector3(x, y, z);

                    for (Direction side = 0; (int)side < 6; side++)
                    {
                        var neighboringPosition = (position + BlockNormals[(int)side]).FloorToInt();

                        if (neighboringPosition.X >= Chunk.SIZE || neighboringPosition.X < 0
                        || neighboringPosition.Y >= Chunk.SIZE || neighboringPosition.Y < 0
                        || neighboringPosition.Z >= Chunk.SIZE || neighboringPosition.Z < 0)
                        {
                            chunkMesh.AddFace(new Vector3(x, y, z), blockType, side, offset: Vector3.Zero);
                        }
                        else
                        {
                            var neighboringBlockType = chunk.Blocks[Chunk.Index(neighboringPosition)];

                            if (!neighboringBlockType.IsOpaque())
                                chunkMesh.AddFace(new Vector3(x, y, z), blockType, side, offset: Vector3.Zero);
                        }

                    }
                }
            }
        }

        chunkMesh.Finish();

        return chunkMesh;
    }
}