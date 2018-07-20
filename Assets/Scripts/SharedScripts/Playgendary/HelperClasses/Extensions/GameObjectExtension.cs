using UnityEngine;
using System.Collections.Generic;


public static class GameObjectExtension 
{

    static List<SkinnedMeshRenderer> smrList = new List<SkinnedMeshRenderer>();
    static List<MeshFilter> mfList = new List<MeshFilter> ();

    public static Shader replaceShader;

	
    public static void Render(this GameObject go, Vector3 position, Quaternion rotation) 
    {
        go.Render(position, rotation, Vector3.one);
    }


	public static void Render(this GameObject go, Vector3 position, Quaternion rotation, Vector3 scale) 
    {
        smrList.Clear();
        go.FindComponentsInChildren<SkinnedMeshRenderer>(ref smrList);
        foreach (SkinnedMeshRenderer smr in smrList)
        {
            GameObject renderObject = smr.gameObject;
            Mesh amesh = smr.sharedMesh;
            Material[] amaterials = smr.GetComponent<Renderer>().sharedMaterials;
            Quaternion q = rotation * renderObject.transform.rotation;
            Matrix4x4 m = Matrix4x4.TRS(position + (renderObject.transform.position - go.transform.position), q, new Vector3(renderObject.transform.lossyScale.x * scale.x, renderObject.transform.lossyScale.y * scale.y, renderObject.transform.lossyScale.z * scale.z));

            for (int i = 0; i < amaterials.Length; i++) 
            {
				if(amaterials[i] != null && amesh != null)
				{
	                Shader oldShader = amaterials[i].shader;
	                if(replaceShader != null)
	                {
	                    amaterials[i].shader = replaceShader;
	                }

	                amaterials[i].SetPass(0);
	                Graphics.DrawMeshNow(amesh, m, i);

	                amaterials[i].shader = oldShader;
				}
            }
        }

        mfList.Clear();
        go.FindComponentsInChildren<MeshFilter>(ref mfList);
        foreach (MeshFilter mf in mfList)
        {
            GameObject renderObject = mf.gameObject;
            Mesh amesh = mf.sharedMesh;
            Material[] amaterials = mf.GetComponent<Renderer>().sharedMaterials;
            Quaternion q = rotation * renderObject.transform.rotation;
            Matrix4x4 m = Matrix4x4.TRS(position + (renderObject.transform.position - go.transform.position), q, new Vector3(renderObject.transform.lossyScale.x * scale.x, renderObject.transform.lossyScale.y * scale.y, renderObject.transform.lossyScale.z * scale.z));
        
            for (int i = 0; i < amaterials.Length; i++) 
            {
                if(amaterials[i] != null && amesh != null)
                {
                    Shader oldShader = amaterials[i].shader;
                    if(replaceShader != null)
                    {
                        amaterials[i].shader = replaceShader;
                    }

                    amaterials[i].SetPass(0);
                    Graphics.DrawMeshNow(amesh, m, i);

                    amaterials[i].shader = oldShader;
                }
            }
        }
    }

	/// <summary>
	/// Find any component in gameobject and children.
	/// </summary>
	public static T FindComponentInChildren<T>(this GameObject go) where T : Component {
		if (go == null) {
			return default(T);
		}
		T findingComponent = go.GetComponent<T>();
		for (int i = 0; (i < go.transform.childCount && (findingComponent == null)); i++) {
			GameObject child = go.transform.GetChild(i).gameObject;
			findingComponent = FindComponentInChildren<T> (child);
		}
		return findingComponent;
	}

	/// <summary>
	/// Find components in gameobject and children.
	/// </summary>
	public static void FindComponentsInChildren<T>(this GameObject go, ref List<T> array) where T : Component {
		if (go == null) {
			return;
		}
		T findingComponent = go.GetComponent<T>();
		if (findingComponent != null) {
			array.Add(findingComponent);
		}
		for (int i = 0; i < go.transform.childCount; i++) {
			GameObject child = go.transform.GetChild(i).gameObject;
			FindComponentsInChildren<T>(child, ref array);
		}
	}
	
	/// <summary>
	/// Find any component in gameobject and parents.
	/// </summary>
	public static T FindComponentInParent<T>(this GameObject go) where T : Component 
    {
        T findingComponent = default(T);
		if (go != null) 
        {
			Transform currentTransform = go.transform;
			while ((findingComponent == null) && (currentTransform != null)) 
            {
				findingComponent = currentTransform.GetComponent<T>();
				currentTransform = currentTransform.parent;
			}
		}
		return findingComponent;
	}


	public static void SetLayerRecursively(this GameObject go, int newLayer) 
    {
		if (go == null) return;

		go.layer = newLayer;
		foreach (Transform child in go.transform) 
        {
			if (child != null) 
            {
				SetLayerRecursively(child.gameObject, newLayer);
			}
        }
    }

    public static void SetSortingLayerRecursively(this GameObject go, int newLayer)
    {
        if (go == null) return;

        MeshRenderer goRenderer = go.GetComponent<MeshRenderer>();

        if (goRenderer != null)
        {
            goRenderer.sortingLayerID = newLayer;
        }

        Canvas canvas = go.GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.sortingLayerID = newLayer;
        }

        foreach (Transform child in go.transform)
        {
            if (child != null)
            {
                SetSortingLayerRecursively(child.gameObject, newLayer);
            }
        }
    }

    public static List<T> GetAllObjectsInScene<T>() where T:Object
    {
        List<T> objectsInScene = new List<T>();

        foreach (T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            if (obj.hideFlags != HideFlags.NotEditable && obj.hideFlags != HideFlags.HideAndDontSave)
            {
                #if UNITY_EDITOR
                if( UnityEditor.PrefabUtility.GetPrefabType(obj) != UnityEditor.PrefabType.Prefab)
                {
                    objectsInScene.Add(obj);
                }
                #endif
            }
        }

        return objectsInScene;
    }


	public static List<Object> GetAllObjectsInScene(System.Type type)
	{
		List<Object> objectsInScene = new List<Object>();

		foreach (var obj in Resources.FindObjectsOfTypeAll(type))
		{
			if (obj.hideFlags != HideFlags.NotEditable && obj.hideFlags != HideFlags.HideAndDontSave)
			{
				#if UNITY_EDITOR
				if( UnityEditor.PrefabUtility.GetPrefabType(obj) != UnityEditor.PrefabType.Prefab)
				{
					objectsInScene.Add(obj);
				}
				#endif
			}
		}

		return objectsInScene;
	}


    public static T GetComponentOnlyInParent<T>(this GameObject go) where T : Component
    {
        return go.GetComponentOnlyInParent(typeof(T)) as T;
    }


    public static Component GetComponentOnlyInParent(this GameObject go, System.Type type)
    {
        Transform parent = go.transform.parent;
        if (parent != null) 
        {
            while (parent != null) 
            {
                if (parent.gameObject.activeInHierarchy)
                {
                    Component component = parent.gameObject.GetComponent(type);
                    if (component != null) {
                        return component;
                    }
                }
                parent = parent.parent;
            }
        }
        return null;
    }


    public static T GetRequiredComponent<T>(this GameObject go, ref T component) where T : Component
    {
        if (component == null)
        {
            component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
        }

        return component;
    }
}


public static class ObjectExtension 
{
	public static bool IsNull(this object obj)
	{
		if(obj is UnityEngine.Object)
		{
			return !(obj as UnityEngine.Object);
		}

		return obj == null;
	}
}