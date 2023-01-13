using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCraft;

public struct BlockVertex : IVertexType
{
    public Vector3 Position;
    public Vector4 Normal;
    public Vector3 TextureCoordinate;

    public static readonly VertexDeclaration VertexDeclaration = new
    (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Normal, 0),
        new VertexElement(28, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
    );

    public BlockVertex(Vector3 position, Vector4 normal, Vector3 textureCoordinate)
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
    }

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}
