namespace MonoCraft;

public static class BlockTypeExtensions
{
    public static bool IsOpaque(this BlockType that)
    {
        return that != BlockType.Air && that != BlockType.Water;
    }
    public static bool IsLiquid(this BlockType that)
    {
        return that == BlockType.Water;
    }
}
