public static class MonoMath
{
    public static int Floor(float x)
    {
        return x < 0 ? (int)x - 1 : (int)x;
    }
}