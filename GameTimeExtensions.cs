using Microsoft.Xna.Framework;

namespace MonoCraft;

internal static class GameTimeExtensions
{
    public static float GetDeltaTimeSeconds(this GameTime that)
    {
        return (float)that.ElapsedGameTime.TotalSeconds;
    }
}