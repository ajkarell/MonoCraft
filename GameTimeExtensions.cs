using Microsoft.Xna.Framework;

public static class GameTimeExtensions
{
    public static float GetDeltaTimeSeconds(this GameTime that)
    {
        return (float)that.ElapsedGameTime.TotalSeconds;
    }
}