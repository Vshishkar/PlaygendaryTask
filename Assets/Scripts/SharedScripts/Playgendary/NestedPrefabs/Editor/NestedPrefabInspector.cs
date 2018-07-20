using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(NestedPrefab))]
public class NestedPrefabInspector : Editor {

	List<System.Action> actions = new List<System.Action>();
	bool cancel;


	void OnEnable()
	{
		EditorApplication.update -= Update;
		EditorApplication.update += Update;
	}


    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();

        GameObject gameobject = (target as NestedPrefab).gameObject;
        GameObject g = PrefabUtility.FindPrefabRoot(gameobject);

        if(g == gameobject)
        {
            if(GUILayout.Button("Update prefabs"))
            {
                UpdatePrefabs();
            }
        }
        else
        {
            if(GUILayout.Button("Revert"))
            {
                Revert();
            }
        }
    }


    public void Revert()
    {
		ReportProgress("", 0);

		AssetUtility.PreloadAssets("Prefabs", typeof(GameObject));

        NestedPrefab[] nestedObjects = Resources.FindObjectsOfTypeAll<NestedPrefab>();
        List<NestedPrefab> prefabsForSearch = new List<NestedPrefab>();
        foreach(Object subtarget in targets)
        {
            NestedPrefab nestedCurrent = (subtarget as NestedPrefab);
            prefabsForSearch.Add(nestedCurrent);
        }
        Dictionary<string, GameObject> nestedDictionary = CollectNestedDictionary(nestedObjects, prefabsForSearch);


        Dictionary<GameObject, List<GameObject>> subPrefabsDictionary = new Dictionary<GameObject, List<GameObject>>();
        foreach(Object subtarget in targets)
        {
            NestedPrefab targetNestedPrefab = (subtarget as NestedPrefab);
            GameObject nestedPrefabForReplace = targetNestedPrefab.gameObject;
            GameObject prefab = PrefabUtility.FindPrefabRoot(nestedPrefabForReplace);

            List<GameObject> prefabChilds = null;

            if(!subPrefabsDictionary.ContainsKey(prefab))
            {
                prefabChilds = new List<GameObject>();
                subPrefabsDictionary.Add(prefab, prefabChilds);
            }
            else
            {
                prefabChilds = subPrefabsDictionary[prefab];
            }

            prefabChilds.Add(nestedPrefabForReplace);
        }

		float progress = 0;
		float prefabPercent = 1f / subPrefabsDictionary.Keys.Count;

        foreach(GameObject prefab in subPrefabsDictionary.Keys)
        {
            List<GameObject> prefabChilds = subPrefabsDictionary[prefab];

            if(PrefabUtility.GetPrefabType(prefab) == PrefabType.PrefabInstance)
            {
				float childProgress = 1f / prefabChilds.Count;
                foreach(GameObject child in prefabChilds)
                {
					progress += childProgress * prefabPercent;
					ReportProgress("Updated  " + child.name + "  in  "  + prefab.name, progress);

                    CustomDebug.Log("Updated  " + child.name + "  in  "  + prefab.name);

                    NestedPrefab targetNestedPrefab = child.GetComponent<NestedPrefab>();
					if(nestedDictionary.ContainsKey(targetNestedPrefab.prefabGuid))
					{
						ReplaceGameobject(child, nestedDictionary[targetNestedPrefab.prefabGuid]);
					}
                    DestroyImmediate(child);
                }
            }
            else if(PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab)
            {
                List<int> indexSet = new List<int>();

                foreach(GameObject childPrefab in prefabChilds)
                {
                    for(int i = 0; i < prefab.transform.childCount; i++)
                    {
                        GameObject child = prefab.transform.GetChild(i).gameObject;
                        if(child == childPrefab)
                        {
                            indexSet.Add(i);
                        }
                    }
                }


                GameObject newInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

				float childProgress = 1f / indexSet.Count;
                for(int i = 0; i < indexSet.Count; i++)
                {
                    GameObject childInstance = newInstance.transform.GetChild(indexSet[i]).gameObject;
                    NestedPrefab targetNestedPrefab = prefabChilds[i].GetComponent<NestedPrefab>();
                    ReplaceGameobject(childInstance, nestedDictionary[targetNestedPrefab.prefabGuid]);
                    DestroyImmediate(childInstance);

					progress += childProgress * prefabPercent;
					ReportProgress("Updated  " + childInstance.name + "  in  "  + prefab.name, progress);

                    CustomDebug.Log("Updated  " + childInstance.name + "  in  "  + prefab.name);
                }

                PrefabUtility.ReplacePrefab(newInstance, prefab);
                DestroyImmediate(newInstance);
            }
            else
            {
                CustomDebug.Log(prefab.name + " : prefab is broken");
            }
        }

		EditorUtility.ClearProgressBar();
    }


	void Update()
	{
		if(actions.Count > 0)
		{
			actions[0]();
			actions.RemoveAt(0);
		}

		if(cancel)
		{
			cancel = false;
			EditorUtility.ClearProgressBar();
			actions.Clear();
		}
	}


    public void UpdatePrefabs()
    {
		ReportProgress("", 0);

		AssetUtility.PreloadAssets("Prefabs", typeof(GameObject));

        NestedPrefab[] nestedObjects = Resources.FindObjectsOfTypeAll<NestedPrefab>();
        List<GameObject> modifiedPrefabs = new List<GameObject>();
        Dictionary<string, GameObject> nestedDictionary = new Dictionary<string, GameObject>();

        foreach(Object subtarget in targets)
        {
            GameObject gameobject = (subtarget as NestedPrefab).gameObject;

            //find prefab in assets and check
            GameObject sourcePrefab = PrefabUtility.GetPrefabType(gameobject) == PrefabType.Prefab ? gameobject : (PrefabUtility.GetPrefabParent(gameobject) as GameObject);
            if(sourcePrefab == null)
            {
                CustomDebug.Log("SourcePrefab missing");
                return;
            }

            NestedPrefab nestedSource = sourcePrefab.GetComponent<NestedPrefab>();
            if(nestedSource == null)
            {
                CustomDebug.Log("NestedPrefab missing");
                return;
            }

            nestedDictionary.Add(nestedSource.prefabGuid, sourcePrefab);

			ReportProgress("Find Prefabs", 0);

            //find all prefabs with nested prefab
            foreach(NestedPrefab nestedTarget in nestedObjects)
            {
                if(PrefabUtility.GetPrefabType(nestedTarget.gameObject) == PrefabType.Prefab)
                {
                    if(string.Equals(nestedTarget.prefabGuid, nestedSource.prefabGuid))
                    {
                        GameObject findedPrefab = PrefabUtility.FindPrefabRoot(nestedTarget.gameObject);

                        if(findedPrefab != sourcePrefab)
                        {
                            if(!modifiedPrefabs.Contains(findedPrefab))
							{
								ReportProgress("Find Prefabs : " + findedPrefab.name, 0);
                                modifiedPrefabs.Add(findedPrefab);
							}
                            //                            CustomDebug.Log(AssetDatabase.GetAssetPath(findedPrefab));
                        }
                    }
                }
            }
        }


		float progress = 0;
		float prefabPercent = 1f / modifiedPrefabs.Count;
        List<GameObject> objectsForRemove = new List<GameObject>();

		for(int index = 0; index < modifiedPrefabs.Count; index++)
        {
			int i = index;
			System.Action action = () => 
			{
	            GameObject prefab = modifiedPrefabs[i];
	            GameObject newInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

				ReportProgress("Updating : " + prefab.name, progress);

	            NestedPrefab[] children = newInstance.GetComponentsInChildren<NestedPrefab>();
				float childProgress = 1f / children.Length;

	            foreach(NestedPrefab childForReplace in children)
	            {
	                if(nestedDictionary.ContainsKey(childForReplace.prefabGuid))
	                {
	                    GameObject nestedSource = nestedDictionary[childForReplace.prefabGuid];

						ReportProgress("Updated  " + nestedSource.name + "  in  "  + prefab.name, progress);
	                    CustomDebug.Log("Updated  " + nestedSource.name + "  in  "  + prefab.name);

	                    objectsForRemove.Add(childForReplace.gameObject);
	                    ReplaceGameobject(childForReplace.gameObject, nestedSource);
	                }

					progress += childProgress * prefabPercent;
	            }


	            objectsForRemove.ForEach(DestroyImmediate);
	            objectsForRemove.Clear();


	            PrefabUtility.ReplacePrefab(newInstance, prefab);
	            DestroyImmediate(newInstance);
			};

			actions.Add(action);
        }

		actions.Add(EditorUtility.ClearProgressBar);
    }


	void ReportProgress(string currentAction, float progress)
	{
		cancel |= EditorUtility.DisplayCancelableProgressBar(
			"Update Nested Prefabs",
			currentAction,
			progress);
	}


    public static GameObject ReplaceGameobject(GameObject old, GameObject sourcePrefab)
    {
        GameObject newObj = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;
        newObj.name = old.name;
        newObj.transform.parent = old.transform.parent;
        newObj.transform.localPosition = old.transform.localPosition;

		var components = newObj.GetComponents(typeof(INestedPrefab));
		foreach(INestedPrefab component in components)
		{
			component.OverridePrefab(old);
		}

        return newObj;
    }


    public static Dictionary<string, GameObject> CollectNestedDictionary(NestedPrefab[] allNestedObjects, List<NestedPrefab> nestedPrefabsForSearch)
    {
        Dictionary<string, GameObject> nestedDictionary = new Dictionary<string, GameObject>();

        foreach(NestedPrefab nestedCurrent in nestedPrefabsForSearch)
        {
            if(!nestedDictionary.ContainsKey(nestedCurrent.prefabGuid))
            {
                // find prefabs for selected
                foreach(NestedPrefab np in allNestedObjects)
                {
                    GameObject gameobject = np.gameObject;

                    if(PrefabUtility.GetPrefabType(gameobject) == PrefabType.Prefab && PrefabUtility.FindPrefabRoot(gameobject) == gameobject)
                    {
                        NestedPrefab nestedSource = gameobject.GetComponent<NestedPrefab>();
                        if(nestedSource != null)
                        {
                            if(nestedSource.prefabGuid == nestedCurrent.prefabGuid)
                            {
                                if(!nestedDictionary.ContainsKey(nestedSource.prefabGuid))
                                    nestedDictionary.Add(nestedSource.prefabGuid, nestedSource.gameObject);
                                else
                                    CustomDebug.LogError("Duplicate GUID : " + nestedSource.gameObject + "  +  " + nestedDictionary[nestedSource.prefabGuid] + " -> Solution : дать леща" );
                            }
                        }
                        else
                            CustomDebug.Log("NestedPrefab component missing");

                    }
                }


                if(!nestedDictionary.ContainsKey(nestedCurrent.prefabGuid))
                {
                    CustomDebug.LogError("can't find prefab for " + nestedCurrent.name);
                }
            }
        }

        return nestedDictionary;
    }



    public static void UpdateNestedPrefabsInGameobject(GameObject root)
    {
        NestedPrefab[] nestedObjects = Resources.FindObjectsOfTypeAll<NestedPrefab>();

        List<NestedPrefab> nestedPrefabsForReplace = new List<NestedPrefab>();
        root.FindComponentsInChildren<NestedPrefab>(ref nestedPrefabsForReplace);

        Dictionary<string, GameObject> nestedDictionary = CollectNestedDictionary(nestedObjects, nestedPrefabsForReplace);
        List<GameObject> objectsForRemove = new List<GameObject>();

        GameObject prefab = PrefabUtility.FindPrefabRoot(root);
        if(PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab)
        {
            GameObject newInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            NestedPrefab[] childNestedPrefabs = newInstance.GetComponentsInChildren<NestedPrefab>();

            foreach(NestedPrefab childInstance in childNestedPrefabs)
            {
                CustomDebug.Log("Updated  " + childInstance.name + "  in  "  + prefab.name);

                objectsForRemove.Add(childInstance.gameObject);
                ReplaceGameobject(childInstance.gameObject, nestedDictionary[childInstance.prefabGuid]);
                DestroyImmediate(childInstance);
            }

            objectsForRemove.ForEach(DestroyImmediate);
            objectsForRemove.Clear();

            PrefabUtility.ReplacePrefab(newInstance, prefab);
            DestroyImmediate(newInstance);
        }
        else if(PrefabUtility.GetPrefabType(prefab) == PrefabType.PrefabInstance)
        {
            foreach(NestedPrefab childInstance in nestedPrefabsForReplace)
            {
                CustomDebug.Log("Updated  " + childInstance.name + "  in  "  + prefab.name);

                objectsForRemove.Add(childInstance.gameObject);
                ReplaceGameobject(childInstance.gameObject, nestedDictionary[childInstance.prefabGuid]);
                DestroyImmediate(childInstance);
            }
            objectsForRemove.ForEach(DestroyImmediate);
            objectsForRemove.Clear();
        }
        else
        {
            CustomDebug.Log(prefab.name + " : prefab is broken");
        }
    }
}
