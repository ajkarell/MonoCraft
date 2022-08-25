public static class WorldGenerator
{
    public static BlockType[] GenerateChunkBlocks(Vector3Int coordinate)
    {
        var blocks = new BlockType[Chunk.SIZE_CUBED];
        for (int y = 0; y < Chunk.SIZE; y++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    blocks[Chunk.Index(x, y, z)] = y <= 16 ? BlockType.Dirt : BlockType.Air;
                }
            }
        }

        return blocks;
    }
}