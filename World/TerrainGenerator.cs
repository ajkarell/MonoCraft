using Microsoft.Xna.Framework;
using MonoCraft.Noise;

namespace MonoCraft;

public static class TerrainGenerator
{
    static readonly FastNoiseLite FastNoiseLite = new(seed: 28/02/04);
    static float NoiseScale => 0.50f;

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

                    var density = FastNoiseLite.GetNoise(worldPosition.X * NoiseScale, worldPosition.Y * NoiseScale, worldPosition.Z * NoiseScale);

                    densities[Chunk.Index(x, y, z)] = density;
                }
            }
        }

        BlockType CalculateBlockType(int x, int y, int z)
        {
            var worldY = y + coordinate.Y * Chunk.SIZE;

            var density = densities[Chunk.Index(x, y, z)];
            var adjustedDensity = density - worldY / 25.0f;

            if (adjustedDensity <= 0)
                return BlockType.Air;

            if (adjustedDensity <= 0.20f) 
            {
                var densityAbove = y + 1 >= Chunk.SIZE
                    ? float.MaxValue
                    : densities[Chunk.Index(x, y + 1, z)];

                if (densityAbove <= 0)
                    return BlockType.Grass;
                else
                    return BlockType.Dirt;
            }

            return BlockType.Stone;
        }

        for (int y = 0; y < Chunk.SIZE; y++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    int index = Chunk.Index(x, y, z);

                    blocks[index] = CalculateBlockType(x, y, z);
                }
            }
        }

        return blocks;
    }
}