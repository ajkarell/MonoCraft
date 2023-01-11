using System;

namespace MonoCraft;

internal static class Settings
{
    public static float FieldOfView = 60.0f * (MathF.PI / 180f);

    public static readonly bool UseFrustumCulling = true;
    public static readonly int RenderDistanceChunks = 5;
    public static readonly int WorldGenThresholdHorizontal = 2;
    public static readonly int WorldGenThresholdVertical = 1;
}