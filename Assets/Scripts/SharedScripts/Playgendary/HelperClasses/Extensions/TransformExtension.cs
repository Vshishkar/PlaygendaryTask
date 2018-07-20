using UnityEngine;

public static class TransformExtension {

	public static void SetLocalPositionX(this Transform t, float x) {
		Vector3 pos = t.localPosition;
		pos.x = x;
		t.localPosition = pos;
	}

	public static void SetLocalPositionY(this Transform t, float y) {
		Vector3 pos = t.localPosition;
		pos.y = y;
		t.localPosition = pos;
	}

	public static void SetLocalPositionZ(this Transform t, float z) {
		Vector3 pos = t.localPosition;
		pos.z = z;
		t.localPosition = pos;
	}


	public static string FullTransformName(this Transform t)
	{
		Transform current = t;
		string path = string.Empty;

		while(current != null)
		{
			path = "/" + current.name + path;
			current = current.parent;
		}

		return path;
	}


	public static void SetGlobalPositionX(this Transform t, float x) {
		Vector3 pos = t.position;
		pos.x = x;
		t.position = pos;
	}

    public static void SetGlobalPositionY(this Transform t, float y) {
        Vector3 pos = t.position;
        pos.y = y;
        t.position = pos;
    }

    public static void SetGlobalPositionZ(this Transform t, float z) {
        Vector3 pos = t.position;
        pos.z = z;
        t.position = pos;
    }
}