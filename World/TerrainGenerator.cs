using Microsoft.Xna.Framework;

public static class TerrainGenerator
{
    static FastNoiseLite FastNoiseLite = new(seed: 280204);
    static float NoiseScale => 0.5f;

    public static BlockType[] GenerateChunkBlocks(Vector3Int coordinate)
    {
        var densities = new float[Chunk.SIZE_CUBED];
        var blocks = new BlockType[Chunk.SIZE_CUBED];

        for (int y = 0; y < Chunk.SIZE; y++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    var worldPosition = coordinate * Chunk.SIZE + new Vector3(x, y, z);

                    var density = FastNoiseLite.GetNoise(worldPosition.X * NoiseScale, worldPosition.Y * NoiseScale, worldPosition.Z * NoiseScale)
                                        * 30f;

                    density -= worldPosition.Y;

                    densities[Chunk.Index(x, y, z)] = density;
                }
            }
        }

        for (int y = 0; y < Chunk.SIZE; y++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    int index = Chunk.Index(x, y, z);
                    BlockType block = BlockType.Air;

                    if (densities[index] > 0)
                    {
                        var densityAbove = y + 1 < Chunk.SIZE ? densities[Chunk.Index(x, y + 1, z)] : float.MaxValue;

                        block = densityAbove > 0 ? BlockType.Dirt : BlockType.Grass;
                    }

                    blocks[index] = block;
                }
            }
        }

        return blocks;
    }
}