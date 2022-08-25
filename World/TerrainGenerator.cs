using Microsoft.Xna.Framework;

public static class TerrainGenerator
{
    static FastNoiseLite FastNoiseLite = new(seed: 280204);
    static float NoiseScale => 0.1f;

    public static BlockType[] GenerateChunkBlocks(Vector3Int coordinate)
    {
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

                    blocks[Chunk.Index(x, y, z)] = density < 0 ? BlockType.Air : BlockType.Dirt;
                }
            }
        }

        return blocks;
    }
}