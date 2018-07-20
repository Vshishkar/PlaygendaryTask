using UnityEngine;
using System.Collections;

public static class TK2D_RoundShiftExtentions
{
    static Vector2 resultVector2;
    static Vector3 resultVector3;

    public static Vector2 FloorVector(this Vector2 vector)
	{
        resultVector2.x = Mathf.Floor(vector.x);
        resultVector2.y = Mathf.Floor(vector.y);
        return resultVector2;
	}


	public static Vector3 FloorVector(this Vector3 vector)
	{
        resultVector3.x = Mathf.Floor(vector.x);
        resultVector3.y = Mathf.Floor(vector.y);
        resultVector3.z = Mathf.Floor(vector.z);
        return resultVector3;
	}


	public static Vector3 RoundVector(this Vector3 vector)
	{
        resultVector3.x = Mathf.Round(vector.x);
        resultVector3.y = Mathf.Round(vector.y);
        resultVector3.z = Mathf.Round(vector.z);
        return resultVector3;
	}


    public static Vector2 RoundShift(this Transform transform, bool useSelfShift = true)
	{
        resultVector2.x = 0;
        resultVector2.y = 0;

        for (Transform curTransform = useSelfShift ? transform : transform.parent; curTransform != null; curTransform = curTransform.parent)
		{
			Vector2 curShift = curTransform.localPosition - curTransform.localPosition.FloorVector();
            resultVector2 += curShift;
		}

        return resultVector2;
	}	
}
