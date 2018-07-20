using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


public interface INestedPrefab
{
	void OverridePrefab(GameObject oldPrefab);
}


public class NestedPrefab : MonoBehaviour {

	public string prefabGuid;

	void Reset()
	{
		GenerateNewGUID();
	}


	public void GenerateNewGUID()
	{
		prefabGuid = System.Guid.NewGuid().ToString();
	}


	#if UNITY_EDITOR

	static NestedPrefab()
	{
		EditorApplication.projectWindowChanged += CheckForModifiedAssets;
	}


	static void CheckForModifiedAssets()
	{
		foreach(GameObject g in Selection.gameObjects)
		{
			if(g.GetComponent<NestedPrefab>())
			{
				string path = AssetDatabase.GetAssetPath(g);
				if(!string.IsNullOrEmpty(path))
				{
					string subPath = System.IO.Path.GetDirectoryName(path);
					if(!string.IsNullOrEmpty(subPath))
					{
						CollectGUIDMap(subPath);
					}
				}
			}
		}
	}


	public static Dictionary<string, NestedPrefab> CollectGUIDMap(string filter, List<GameObject> duplicates = null)
	{
		Dictionary<string, NestedPrefab> map = new Dictionary<string, NestedPrefab>();
		List<NestedPrefab> prefabs = AssetUtility.GetAssetsAtPath<NestedPrefab>(filter);

		foreach(NestedPrefab p in prefabs)
		{
			if(!p.IsNull())
			{
				if(!map.ContainsKey(p.prefabGuid))
				{
					map.Add(p.prefabGuid, p); 
				}
				else
				{
					if(duplicates != null)
					{
						duplicates.Add(p.gameObject);
					}

					CustomDebug.LogError("NestedPrefab: Duplicate GUID pair :  " + p.name + "   +   " + map[p.prefabGuid].name);

					p.GenerateNewGUID();
                    EditorUtility.SetDirty(p);
					map.Add(p.prefabGuid, p);

                    CustomDebug.LogError("NestedPrefab: Rebuid GUID for " + p.name, p.gameObject);
				}
			}
		}

		return map;
	}


	[MenuItem("Inventain/Nested Prefabs/Check Duplicates")]
	static void CheckNestedPrefabs()
	{
		List<GameObject> duplicates = new List<GameObject>();
		CollectGUIDMap(".prefab", duplicates);

		if(duplicates.Count > 0)
		{
			Selection.objects = duplicates.ToArray();
		}
		else
		{
            CustomDebug.Log("No duplicated GUID found");
		}
	}
	#endif
}