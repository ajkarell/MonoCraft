public static class MonoMath
{
    public static int Floor(float x)
    {
        return x < 0 ? (int)x - 1 : (int)x;
    }

    public static float Clamp(float value, float min, float max)
    {
        if(value < min)
            return min;
        if(value > max)
            return max;

        return value;
    }

    public static float Repeat(float value, float min, float max)
    {
        if(value > max)
            return value % max;
        if(value < min)
             return max - Abs(value % max);
        return value;
    }

    public static float Abs(float x)
        => x < 0 ? -x : x;
}