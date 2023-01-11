using Microsoft.Xna.Framework;

namespace MonoCraft;

public static class ChunkMeshGenerator
{
    static readonly Vector3 FORWARD = Vector3.Forward;
    static readonly Vector3 UP = Vector3.Up;
    static readonly Vector3 RIGHT = Vector3.Right;
    static readonly Vector3 ZERO = Vector3.Zero;

    public static readonly Vector3[] blockNormals =
    {
        -RIGHT ,    // CUBE_LEFT    0
        RIGHT,      // CUBE_RIGHT   1
        -UP,        // CUBE_BOTTOM  2
        UP,         // CUBE_TOP     3
        -FORWARD,   // CUBE_BACK    4
        FORWARD,    // CUBE_FRONT   5
    };

    public static readonly Vector3[][] blockPositionsBySide = {
        new[] {   UP,                     ZERO,               FORWARD,            UP+FORWARD          },  // CUBE_LEFT    0
        new[] {   RIGHT + UP + FORWARD,   RIGHT + FORWARD,    RIGHT,              RIGHT + UP          },  // CUBE_RIGHT   1
        new[] {   RIGHT,                  RIGHT + FORWARD,    FORWARD,            ZERO                },  // CUBE_BOTTOM  2
        new[] {   RIGHT + UP + FORWARD,   RIGHT + UP,         UP,                 UP + FORWARD        },  // CUBE_TOP     3
        new[] {   RIGHT + UP,             RIGHT,              ZERO,               UP                  },  // CUBE_BACK    4
        new[] {   UP+FORWARD,             FORWARD,            RIGHT+FORWARD,      RIGHT + UP + FORWARD},  // CUBE_FRONT   5
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
                        continue;

                    var position = new Vector3(x, y, z);
                    for (BlockSide side = 0; (int)side < 6; side++)
                    {
                        var neighborPosition = (position + blockNormals[(int)side]).FloorToInt();

                        if (neighborPosition.X >= Chunk.SIZE || neighborPosition.X < 0
                        || neighborPosition.Y >= Chunk.SIZE || neighborPosition.Y < 0
                        || neighborPosition.Z >= Chunk.SIZE || neighborPosition.Z < 0)
                        {
                            // still add face for now...
                        }
                        else
                        {
                            var neighborBlockType = chunk.Blocks[Chunk.Index(neighborPosition)];

                            if (neighborBlockType.IsOpaque())
                                continue;
                        }

                        chunkMesh.AddFace(new Vector3(x, y, z), blockType, side);
                    }
                }
            }
        }

        chunkMesh.Finish();
        return chunkMesh;
    }
}