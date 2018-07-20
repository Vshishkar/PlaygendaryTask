using UnityEngine;

public static class VectorExtension 
{
    public static Vector2 ToVector2(this Vector3 vec3)
    {
        return vec3;
    }

    public static Vector3 ToVector3(this Vector2 vec2)
    {
        return vec2;
    }

    public static Vector3 Add(this Vector3 thisVector, Vector2 vec2)
    {
        return thisVector + vec2.ToVector3();
    }

    public static Vector2 Add(this Vector2 thisVector, Vector3 vec3)
    {
        return thisVector + vec3.ToVector2();
    }

    public static Vector3 Sub(this Vector3 thisVector, Vector2 vec2)
    {
        return thisVector - vec2.ToVector3();
    }

    public static Vector2 Sub(this Vector2 thisVector, Vector3 vec3)
    {
        return thisVector - vec3.ToVector2();
    }
}