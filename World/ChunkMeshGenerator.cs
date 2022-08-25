using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public struct BlockVertex : IVertexType
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector3 TextureCoordinate;

    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
        new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
    );

    public BlockVertex(Vector3 position, Vector3 normal, Vector3 textureCoordinate)
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
    }

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}

public class ChunkMesh
{
    public BlockVertex[] Vertices;
    private List<BlockVertex> verticesList = new();
    public int[] Indices;
    private List<int> indicesList = new();

    public Matrix WorldMatrix { get; init; }
    public Matrix InverseTransposeWorldMatrix { get; init; }

    public int VertexCount { get; private set; }
    public int TriangleCount { get; private set; }
    public bool IsEmpty { get; private set; }

    private int triangleIndex = 0;

    public ChunkMesh(Vector3 worldPosition)
    {
        WorldMatrix = Matrix.CreateTranslation(worldPosition);
        InverseTransposeWorldMatrix = Matrix.Transpose(Matrix.Invert(WorldMatrix));
    }

    public void AddFace(Vector3 blockPosition, BlockType blockType, BlockSide blockSide)
    {
        int blockSideIndex = (int)blockSide;
        int textureIndex = Block.GetTextureIndex(blockType, blockSide);

        var positions = ChunkMeshGenerator.blockPositionsBySide[blockSideIndex];
        var normals = ChunkMeshGenerator.blockNormals;

        var uvsWithTextureIndex = new Vector3[] {
            new Vector3(0, 0, textureIndex),
            new Vector3(0, 1, textureIndex),
            new Vector3(1, 1, textureIndex),
            new Vector3(1, 0, textureIndex),
        };

        var faceNormal = normals[blockSideIndex];

        var faceVertices = new BlockVertex[] {
            new BlockVertex(blockPosition + positions[0], faceNormal, uvsWithTextureIndex[0]),
            new BlockVertex(blockPosition + positions[1], faceNormal, uvsWithTextureIndex[1]),
            new BlockVertex(blockPosition + positions[2], faceNormal, uvsWithTextureIndex[2]),
            new BlockVertex(blockPosition + positions[3], faceNormal, uvsWithTextureIndex[3]),
        };

        verticesList.AddRange(faceVertices);

        var indices = new int[] {
            triangleIndex + ChunkMeshGenerator.blockFaceIndices[0],
            triangleIndex + ChunkMeshGenerator.blockFaceIndices[1],
            triangleIndex + ChunkMeshGenerator.blockFaceIndices[2],
            triangleIndex + ChunkMeshGenerator.blockFaceIndices[3],
            triangleIndex + ChunkMeshGenerator.blockFaceIndices[4],
            triangleIndex + ChunkMeshGenerator.blockFaceIndices[5],
        };

        indicesList.AddRange(indices);

        triangleIndex += 4;
    }

    public void Finish()
    {
        Vertices = verticesList.ToArray();
        Indices = indicesList.ToArray();

        VertexCount = Vertices.Length;
        TriangleCount = Indices.Length / 3;

        IsEmpty = (VertexCount == 0);

        verticesList.Clear();
        verticesList = null;

        indicesList.Clear();
        indicesList = null;
    }
}

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

    public static readonly int[] blockFaceIndices =
    {
        0,1,2,
        2,3,0
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
                            // continue;
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