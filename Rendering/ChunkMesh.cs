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

    public void AddFace(Vector3 blockPosition, BlockType blockType, BlockSide blockSide)
    {
        int blockSideIndex = (int)blockSide;
        int textureIndex = Block.GetTextureIndex(blockType, blockSide);

        var positions = ChunkMeshGenerator.blockPositionsBySide[blockSideIndex];
        var normals = ChunkMeshGenerator.blockNormals;

        var uvsWithTextureIndex = new Vector3[] {
            new(0, 0, textureIndex),
            new(0, 1, textureIndex),
            new(1, 1, textureIndex),
            new(1, 0, textureIndex),
        };

        var faceNormal = normals[blockSideIndex];

        var faceVertices = new BlockVertex[] {
            new(blockPosition + positions[0], faceNormal, uvsWithTextureIndex[0]),
            new(blockPosition + positions[1], faceNormal, uvsWithTextureIndex[1]),
            new(blockPosition + positions[2], faceNormal, uvsWithTextureIndex[2]),
            new(blockPosition + positions[3], faceNormal, uvsWithTextureIndex[3]),
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

    public void Destroy()
    {
        Vertices = null;
        Indices = null;
    }
}
