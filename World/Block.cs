using System.Collections.Generic;

namespace MonoCraft;

public static class Block
{
    private static int CurrentTextureIndex = 0;
    private static readonly Dictionary<string, int> TextureIndices = new();

    /// <summary>
    /// Tries to get texture index. If it doesn't exist, generate one.
    /// </summary>
    private static int GetTextureIndex(string textureFileName)
    {
        if (TextureIndices.TryGetValue(textureFileName, out var index))
        {
            return index;
        }
        else
        {
            TextureIndices[textureFileName] = CurrentTextureIndex;
            CurrentTextureIndex++;
            return TextureIndices[textureFileName];
        }
    }

    private static readonly Dictionary<(BlockType, BlockSide), int> BlockSivuTextureIndices = new();

    /// <summary>
    /// Register texture for every side
    /// </summary>
    private static void RegisterBlockTextures(
        BlockType tyyppi,
        string textureTop,
        string textureBottom,
        string textureFront,
        string textureRight,
        string textureBack,
        string textureLeft)
    {
        BlockSivuTextureIndices[(tyyppi, BlockSide.Top)] = GetTextureIndex(textureTop);
        BlockSivuTextureIndices[(tyyppi, BlockSide.Bottom)] = GetTextureIndex(textureBottom);
        BlockSivuTextureIndices[(tyyppi, BlockSide.Front)] = GetTextureIndex(textureFront);
        BlockSivuTextureIndices[(tyyppi, BlockSide.Right)] = GetTextureIndex(textureRight);
        BlockSivuTextureIndices[(tyyppi, BlockSide.Back)] = GetTextureIndex(textureBack);
        BlockSivuTextureIndices[(tyyppi, BlockSide.Left)] = GetTextureIndex(textureLeft);
    }

    /// <summary>
    /// Register textures to top, bottom and sides
    /// </summary>
    private static void RegisterBlockTextures(
        BlockType tyyppi,
        string textureTop,
        string textureBottom,
        string textureSide)
    {
        RegisterBlockTextures(
            tyyppi,
            textureTop: textureTop,
            textureBottom: textureBottom,
            textureFront: textureSide,
            textureRight: textureSide,
            textureBack: textureSide,
            textureLeft: textureSide);
    }

    /// <summary>
    /// Registers same texture to all sides
    /// </summary>
    private static void RegisterBlockTextures(BlockType type, string texture)
    {
        RegisterBlockTextures(
            type,
            textureTop: texture,
            textureBottom: texture,
            textureFront: texture,
            textureRight: texture,
            textureBack: texture,
            textureLeft: texture);
    }

    public static int GetTextureIndex(BlockType type, BlockSide side)
    {
        return BlockSivuTextureIndices[(type, side)];
    }

    private const string TEXTURE_DIRT = "dirt";
    private const string TEXTURE_GRASS_TOP = "grass_top";
    private const string TEXTURE_GRASS_SIDE = "grass_side";

    public static int RegisterBlockTextures()
    {
        RegisterBlockTextures(BlockType.Dirt, TEXTURE_DIRT);
        RegisterBlockTextures(
            BlockType.Grass,
            textureTop: TEXTURE_GRASS_TOP,
            textureBottom: TEXTURE_DIRT,
            textureSide: TEXTURE_GRASS_SIDE);
        // RegisterBlockTextures(BlockType.Hiekka, TEXTURE_HIEKKA);
        // RegisterBlockTextures(BlockType.Vesi, TEXTURE_VESI);
        // RegisterBlockTextures(BlockType.Kivi, TEXTURE_KIVI);

        return TextureIndices.Count;
    }

    public static IEnumerable<KeyValuePair<string, int>> GetRegisteredTextures()
        => TextureIndices;
}