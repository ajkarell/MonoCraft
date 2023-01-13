using System;

namespace MonoCraft;

internal static class Settings
{
    public const float FieldOfView = 60.0f * (MathF.PI / 180f);
    public const float LookSensitivity = 40.0f;

    public const int RenderDistanceChunks = 8;
    public const int WorldGenThresholdHorizontal = 2;
    public const int WorldGenThresholdVertical = 1;
}