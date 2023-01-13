using Microsoft.Xna.Framework;
using MonoCraft.Noise;
using System.Runtime.CompilerServices;

namespace MonoCraft;

public static class TerrainGenerator
{
    static readonly FastNoiseLite terrainNoiseGenerator = new(seed: 28 / 02 / 04);
    static readonly FastNoiseLite biomeNoiseGenerator = new(seed: 21 / 12 / 04);

    static float TerrainNoiseScale => 0.50f;
    static float BiomeNoiseScale => 0.02f;
    static float BiomeNoiseEffect => 0.40f;

    static float HeightDensityEffect => 0.04f;

    static int WaterLevel => 0;

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

                    var terrainNoise = terrainNoiseGenerator.GetNoise(worldPosition.X * TerrainNoiseScale, worldPosition.Y * TerrainNoiseScale, worldPosition.Z * TerrainNoiseScale);
                    var biomeNoise = biomeNoiseGenerator.GetNoise(worldPosition.X * BiomeNoiseScale, worldPosition.Z * BiomeNoiseScale);

                    var density = terrainNoise
                        + biomeNoise * BiomeNoiseEffect
                        - worldPosition.Y * HeightDensityEffect;

                    densities[Chunk.Index(x, y, z)] = density;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        BlockType CalculateBlockType(int x, int y, int z)
        {
            var worldY = y + coordinate.Y * Chunk.SIZE;

            var density = densities[Chunk.Index(x, y, z)];

            if (density <= 0)
            {
                if (worldY == WaterLevel)
                    return BlockType.Water;
                else
                    return BlockType.Air;
            }

            if (density <= 0.20f)
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