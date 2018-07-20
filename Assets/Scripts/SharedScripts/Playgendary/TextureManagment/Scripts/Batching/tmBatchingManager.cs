using UnityEngine;
using System.Collections.Generic;


[ExecuteInEditMode]
public class tmBatchingManager : SingletonMonoBehaviour<tmBatchingManager> 
{
	public const int DYNAMIC_BATCHING_LIMIT = 1024;
	public const int VERTEX_LIMIT = 65535;
	public const int VERTEX_PER_FRAME_LIMIT = 1;

	#region Variables
	Dictionary<string, List<tmBatchedInstance>> batchTable = new Dictionary<string, List<tmBatchedInstance>>();
	List<tmBatchedInstance> modifiedBatchInstances = new List<tmBatchedInstance>();


	public static new tmBatchingManager Instance
	{ 
		get 
		{ 
			tmBatchingManager inst = SingletonMonoBehaviour<tmBatchingManager>.InstanceIfExist;
			#if UNITY_EDITOR
			if(inst == null)
			{
				GameObject manager = AssetUtility.GetAssetsAtPath<GameObject>("tmManager")[0];
				inst = manager.Clone().GetComponent<tmBatchingManager>();
				inst.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
			#endif
			return inst; 
		} 
	}
	#endregion


	#region Temp Variables
	List<tmBatchedInstance> forcedInstancesToRemove = new List<tmBatchedInstance>();
	#endregion


	#region Interfaces
	#endregion



	#region Unity

	#if UNITY_EDITOR
	void LateUpdate()
	{
        string currentScene = 
    #if UNITY_5_4_OR_NEWER
        UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
    #else
        UnityEditor.EditorApplication.currentScene;
    #endif

        if(!currentScene.Equals("Main"))
		{
			Flush();
		}
	}
	#endif

	#endregion



	#region Public

	public void RegisterModifiedBatchedInstance(tmBatchedInstance batchedInstance)
	{
		if(!modifiedBatchInstances.Contains(batchedInstance))
		{
			modifiedBatchInstances.Add(batchedInstance);
		}
	}


	public void Flush(int vertexLimit = VERTEX_PER_FRAME_LIMIT)
	{
#if PROFILE_TM_BATCHING
		float time = Time.realtimeSinceStartup;
#endif


		if (modifiedBatchInstances.Count > 0)
		{
			int vertices = 0;
			int currentBatchInstance = 0;

			for (; (currentBatchInstance < modifiedBatchInstances.Count) && (vertices < vertexLimit); ++currentBatchInstance)
			{
				tmBatchedInstance inst = modifiedBatchInstances[currentBatchInstance];
				if(!inst.IsNull())
				{
					inst.Recombine();

					if (!inst.ForcedBatching)
					{
						vertices += inst.VertexCount;
					}
				}
			}

			modifiedBatchInstances.RemoveRange(0, currentBatchInstance);


			forcedInstancesToRemove.Clear();

			foreach (var inst in modifiedBatchInstances)
			{
				if(!inst.IsNull() && inst.ForcedBatching)
				{
					inst.Recombine();
					forcedInstancesToRemove.Add(inst);
				}
			}
			
			modifiedBatchInstances.RemoveAll((x) => forcedInstancesToRemove.Contains(x));
			forcedInstancesToRemove.Clear();
		}


#if PROFILE_TM_BATCHING
		if(vertices > 0)
		{
			time = (Time.realtimeSinceStartup - time) * 1000;
            CustomDebug.Log("Total recombine : " + vertices + " by " + time.ToString("f10") + " miliseconds");
		}
#endif
	}


	public void StaticBatch(GameObject root)
	{
		tmTextureRenderBase[] renders = root.GetComponentsInChildren<tmTextureRenderBase>();
		tmManager.Instance.Flush(renders);

		List<tmBatchedInstance> batchedInstances = new List<tmBatchedInstance>();
		tmBatchObject[] objects = root.GetComponentsInChildren<tmBatchObject>();
		foreach(tmBatchObject batch in objects)
		{
			if(batch.BatchInstance == null)
			{
				if(batch.BatchingType == tmBatchingType.Static)
				{
					var sb = tmBatchingManager.Instance.BatchObject(batch, root.transform, false);
					if(!batchedInstances.Contains(sb))
					{
						batchedInstances.Add(sb);
					}
				}
			}
		}

		Flush(int.MaxValue);

		HashSet<string> set = new HashSet<string> ();

		for(int i = 0; i < batchedInstances.Count; i++)
		{
			string name = root.name + "_" + batchedInstances[i].Mesh.name;
            name = name.Replace("_Batch", "");
            name = name.Replace("_batch", "");
            name = name.Substring(0, Mathf.Min(name.Length, 120));
			if(set.Contains(name))
			{
                name = name + "_" + batchedInstances[i].Mesh.vertexCount;
			}
			else
			{
				set.Add(name);
			}

            batchedInstances[i].Mesh.name = name;

			#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				string folderPath = "Assets/Objects/ProcedureMeshes/";
                string meshName = name;
				string path = folderPath + meshName + ".asset";
				
				batchedInstances[i].Mesh.UploadMeshData(true);
				
				UnityEditor.AssetDatabase.DeleteAsset(path);
				UnityEditor.AssetDatabase.CreateAsset(batchedInstances[i].Mesh, path );
				UnityEditor.AssetDatabase.SaveAssets();
			}
			#endif
		}

		foreach(var batchInstance in batchedInstances)
		{
			if(Application.isPlaying)
			{
				Object.Destroy(batchInstance);
			}
			else
			{
				Object.DestroyImmediate(batchInstance);
			}
		}

		foreach(tmBatchObject bo in objects)
		{
			if(!bo.IsNull() && bo.BatchingType == tmBatchingType.Static)
			{
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					DestroyImmediate(bo.gameObject);
				}
				else
				#endif
				{
					Destroy(bo.gameObject);
				}
			}
		}

		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}


	public void Unbatch(GameObject root)
	{
		tmBatchedInstance[] batchers = root.GetComponentsInChildren<tmBatchedInstance>();
		foreach(tmBatchedInstance bo in batchers)
		{
			bo.Clear();
		}
	}


	public void BatchObject(tmBatchObject render, bool move = true)
	{
		BatchObject(render, transform, move);
	}


	public tmBatchedInstance BatchObject(tmBatchObject bo, Transform root, bool move = true)
	{
		if (bo.BatchInstance != null || bo.Mesh == null || bo.Material == null)
		{
			CustomDebug.LogError("invalid batch object : " + bo.name 
			                     + "  reason: instance:" + (bo.BatchInstance != null) 
			                     + ", mesh:" + (bo.Mesh == null)
			                     + ", material:" + (bo.Material == null),
			                     bo);
			return null;
		}

		move = root == null;

		if(root == null)
		{
			root = transform;
		}

		bool needSkin = (bo.BatchingType == tmBatchingType.Skinning);
		string key = "" + root.GetInstanceID() + bo.Material.GetInstanceID() + needSkin;
		List<tmBatchedInstance> batches = null;

		if(batchTable.ContainsKey(key))
		{
			batches = batchTable[key];
		}
		else
		{
			batches = new List<tmBatchedInstance>();
			batchTable.Add(key, batches);
		}

		tmBatchedInstance availibleBatch = null;
		foreach(tmBatchedInstance batch in batches)
		{
			if(batch.CanAdd(bo))
			{
				availibleBatch = batch;
				break;
			}
		}

		if(availibleBatch == null)
		{
			GameObject go = new GameObject();
			go.transform.parent = root;
			go.transform.localPosition = Vector3.zero;
			go.name = "Batch_" + root.name + "_" + bo.Material.name.Replace(tk2dSpriteCollectionData.internalResourcePrefix, "") + "_" + needSkin;
            go.layer = bo.gameObject.layer;

			availibleBatch = needSkin ? go.AddComponent<tmSkinnedBatchedInstance>() : go.AddComponent<tmBatchedInstance>();
			availibleBatch.shouldMove = move;
			availibleBatch.SharedMaterial = bo.Material;

			tmTextureRenderBase original = bo.GetComponent<tmTextureRenderBase>();
			if(original)
			{
				tmBatchRender render = go.AddComponent<tmBatchRender>();
				render.MainTexCollectionGUID = original.MainTexCollectionGUID;
				render.MainTextureID = original.MainTextureID;
				render.LightmapCollectionGUID = original.LightmapCollectionGUID;
				render.LightmapTextureID = original.LightmapTextureID;
                render.Material = original.Material;
			}

			batches.Add(availibleBatch);
			availibleBatch.OnDestroyEvent += () => batches.Remove(availibleBatch);
		}

		if(availibleBatch != null)
		{
			availibleBatch.Add(bo);
		}

		return availibleBatch;
	}

	#endregion


	#region Private
	#endregion
}
