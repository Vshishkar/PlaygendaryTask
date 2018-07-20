using UnityEngine;
using System.Collections;

public class QuadraticBezier 
{
    public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u;
        float uu;
        float uuu;
        float tt;
        float ttt;
        Vector3 p;
        u = 1 - t;
        uu = u * u;
        uuu = uu * u;
        tt = t * t;
        ttt = tt * t;
        p = uuu * p0; //first term of the equation
        p += 3 * uu * t * p1; //second term of the equation
        p += 3 * u * tt * p2; //third term of the equation
        p += ttt * p3; //fourth term of the equation
        return p;
    }

}
