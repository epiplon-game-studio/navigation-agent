using UnityEngine;

public static class VectorExtensions
{

    public static Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    //Swizzle operations to contract Vector3 down to Vector2
    public static Vector2 XY(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 XZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector2 YZ(this Vector3 v)
    {
        return new Vector2(v.y, v.z);
    }

    public static Vector3 XY(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 XZ(this Vector2 v, float y)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static Vector3 YZ(this Vector2 v, float x)
    {
        return new Vector3(x, v.x, v.y);
    }

    public static Vector3 ZeroX(this Vector3 v) { return new Vector3(0, v.y, v.z); }
    public static Vector3 ZeroY(this Vector3 v) { return new Vector3(v.x, 0, v.z); }
    public static Vector3 ZeroZ(this Vector3 v) { return new Vector3(v.x, v.y, 0); }

    public static Vector3 DirectionTo(this Vector3 a, Vector3 b)
    {
        return ((b - a).normalized);
    }
}
