using System;
using Microsoft.Xna.Framework;

public struct Vector3Int
{
    public int X, Y, Z;

    public Vector3Int(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public static Vector3 operator *(Vector3Int lhs, float rhs)
        => lhs.AsVector3() * rhs;

    public override bool Equals(object obj)
    {
        if (obj is Vector3Int) return this.Equals((Vector3Int)obj);
        else return false;
    }

    public bool Equals(Vector3Int other)
    {
        return ((this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override string ToString()
    {
        return string.Format("{{X:{0} Y:{1} Z:{2}}}", X, Y, Z);
    }
}

public static class Vector3IntExtensions
{
    public static Vector3 AsVector3(this Vector3Int that)
    {
        return new Vector3(that.X, that.Y, that.Z);
    }
}

public static class Vector3Extensions
{
    public static Vector3Int FloorToInt(this Vector3 that)
    {
        return new Vector3Int(MonoMath.Floor(that.X), MonoMath.Floor(that.Y), MonoMath.Floor(that.Z));
    }

    public static Vector3Int AsChunkCoordinate(this Vector3 that)
    {
        return FloorToInt(that / (float)Chunk.SIZE);
    }
}