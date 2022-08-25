public enum BlockType : ushort
{
    Air = 0,
    Dirt = 1,
    Grass = 2,
}

public static class BlockTypeExtensions
{
    public static bool IsOpaque(this BlockType that)
    {
        return that != BlockType.Air;
    }
}
