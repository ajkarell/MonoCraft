using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

public class TextureArray : Texture2D
{
    public TextureArray(GraphicsDevice graphicsDevice, int textureWidth, int textureHeight, int arraySize) :
        base(graphicsDevice, textureWidth, textureHeight, true, SurfaceFormat.Color, SurfaceType.Texture, false, arraySize)
    { }
    public void LoadTexturesFromContent(ContentManager contentManager)
    {
        foreach (var (texture, index) in Block.GetRegisteredTextures().Select(x => (x.Key, x.Value)))
        {
            string filePath = @"Textures\" + texture;
            AddTextureToArray(index, contentManager.Load<Texture2D>(filePath));
        }
    }
    private void AddTextureToArray(int index, Texture2D texture)
    {
        for (int i = 0; i < texture.LevelCount; i++) // mipmap levels
        {
            float divisor = 1.0f / (1 << i);
            int[] pixelData = new int[(int)(texture.Width * texture.Height * divisor * divisor)];

            texture.GetData<int>(
                i,
                0,
                new Rectangle(0, 0, (int)(texture.Width * divisor), (int)(texture.Height * divisor)),
                pixelData,
                0,
                pixelData.Length);

            this.SetData<int>(
                i,
                index,
                new Rectangle(0, 0, (int)(texture.Width * divisor), (int)(texture.Height * divisor)),
                pixelData,
                0,
                pixelData.Length);
        }

        texture.Dispose();
    }
}