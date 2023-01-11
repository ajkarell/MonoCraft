using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace MonoCraft;

public static class Math
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Floor(float x)
    {
        return x < 0 ? (int)x - 1 : (int)x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Repeat(float value, float min, float max)
    {
        if (value > max)
            return value % max;
        if (value < min)
            return max - Abs(value % max);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(float x)
        => x < 0 ? -x : x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix CalculateViewMatrix(Vector3 position, Vector3 eulerAngles)
    {
        var pitchRadians = MathHelper.ToRadians(eulerAngles.X);
        var yawRadians = MathHelper.ToRadians(eulerAngles.Y);

        var cosPitch = MathF.Cos(pitchRadians);
        var sinPitch = MathF.Sin(pitchRadians);
        var cosYaw = MathF.Cos(yawRadians);
        var sinYaw = MathF.Sin(yawRadians);

        var xAxis = new Vector3(cosYaw, 0, -sinYaw);
        var yAxis = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
        var zAxis = new Vector3(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw);

        var dotX = Vector3.Dot(xAxis, position);
        var dotY = Vector3.Dot(yAxis, position);
        var dotZ = Vector3.Dot(zAxis, position);

        return new Matrix(
            new(xAxis.X, yAxis.X, zAxis.X, 0),
            new(xAxis.Y, yAxis.Y, zAxis.Y, 0),
            new(xAxis.Z, yAxis.Z, zAxis.Z, 0),
            new(-dotX, -dotY, -dotZ, 1));
    }
}