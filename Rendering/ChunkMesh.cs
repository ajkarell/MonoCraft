using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoCraft;

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

    public void AddFace(Vector3 blockPosition, BlockType blockType, BlockSide blockSide, Vector3 offset)
    {
        int blockSideIndex = (int)blockSide;
        int textureIndex = Block.GetTextureIndex(blockType, blockSide);

        var positions = ChunkMeshGenerator.BlockPositionsBySide[blockSideIndex];

        var uvs = new Vector3[] {
            new(0, 0, textureIndex),
            new(0, 1, textureIndex),
            new(1, 1, textureIndex),
            new(1, 0, textureIndex),
        };

        var faceNormal = ChunkMeshGenerator.BlockNormals[blockSideIndex];
        var alpha = blockType.IsLiquid() ? 0.8f : 1f;

        var faceNormalAndAlpha = new Vector4(faceNormal.X, faceNormal.Y, faceNormal.Z, alpha);

        var faceVertices = new BlockVertex[] {
            new(blockPosition + positions[0] + offset, faceNormalAndAlpha, uvs[0]),
            new(blockPosition + positions[1] + offset, faceNormalAndAlpha, uvs[1]),
            new(blockPosition + positions[2] + offset, faceNormalAndAlpha, uvs[2]),
            new(blockPosition + positions[3] + offset, faceNormalAndAlpha, uvs[3]),
        };

        verticesList.AddRange(faceVertices);

        var indices = new int[] {
            triangleIndex + 0,
            triangleIndex + 1,
            triangleIndex + 2,
            triangleIndex + 2,
            triangleIndex + 3,
            triangleIndex + 0,
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
