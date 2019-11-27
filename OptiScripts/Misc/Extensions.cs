using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    // Get last object in list
    public static T GetLast<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }

    // Converts Vector2 to Vector3 with 0 value z
    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y);
    }
}
