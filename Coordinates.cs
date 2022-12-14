using Microsoft.Xna.Framework;
using System;

namespace MonoCraft;

public struct Vector3Int
{
    public int X, Y, Z;
    public int SquareMagnitude => X * X + Y * Y + Z * Z;

    public Vector3Int(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3Int operator +(Vector3Int lhs, Vector3Int rhs)
        => new(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);

    public static Vector3Int operator -(Vector3Int lhs, Vector3Int rhs)
        => new(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);

    public static Vector3 operator *(Vector3Int lhs, float rhs)
        => lhs.AsVector3() * rhs;

    public static Vector3 operator *(Vector3Int lhs, int rhs)
        => new(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);

    public static bool operator ==(Vector3Int left, Vector3Int right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector3Int left, Vector3Int right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3Int other && this.Equals(other);
    }

    public bool Equals(Vector3Int other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
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

    public static int SquaredDistanceTo(this Vector3Int that, Vector3Int to)
    {
        return (to - that).SquareMagnitude;
    }
}

public static class Vector3Extensions
{
    public static Vector3Int FloorToInt(this Vector3 that)
    {
        return new Vector3Int(Math.Floor(that.X), Math.Floor(that.Y), Math.Floor(that.Z));
    }

    public static Vector3Int AsChunkCoordinate(this Vector3 that)
    {
        return FloorToInt(that / Chunk.SIZE);
    }
}