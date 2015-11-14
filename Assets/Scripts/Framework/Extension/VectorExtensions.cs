using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
    public static Vector3 SwapXY (this Vector3 vector)
    {
        float swap = vector.x;
        vector.x = vector.y;
        vector.y = swap;
        return vector;
    }

    public static Vector3 SwapYZ (this Vector3 vector)
    {
        float swap = vector.y;
        vector.y = vector.z;
        vector.z = swap;
        return vector;
    }

    public static Vector3 SwapZX (this Vector3 vector)
    {
        float swap = vector.x;
        vector.x = vector.z;
        vector.z = swap;
        return vector;
    }

    public static Vector2 SwapXY (this Vector2 vector)
    {
        float swap = vector.x;
        vector.x = vector.y;
        vector.y = swap;
        return vector;
    }
}
