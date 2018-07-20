using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class tmManager : SingletonMonoBehaviour<tmManager> 
{
    public static string stramingAssetsDirectory = "tmAtlases";

	#region Variables
	[SerializeField] List<Mesh> keepImMemoryMeshes = new List<Mesh>();

	List<tmIRebuildable> modifiedRenders = new List<tmIRebuildable>();
	Dictionary<string, Material> materialMap = new Dictionary<string, Material>();
	Dictionary<tmTextureCollectionPlatform, List<Material>> mainTextureMap = new Dictionary<tmTextureCollectionPlatform, List<Material>>();
	Dictionary<tmTextureCollectionPlatform, List<Material>> lightmapMap = new Dictionary<tmTextureCollectionPlatform, List<Material>>();
    Dictionary<string, WeakReference> meshMap = new Dictionary<string, WeakReference>();
	Dictionary<string, tmTextureCollectionPlatform> collectionMap = new Dictionary<string, tmTextureCollectionPlatform>();
    Dictionary<int, string> destroyedMeshes = new Dictionary<int, string>(); // need track meshes with different materials

	bool hasNew = false;

	[ButtonAttribute] public string Btn = "Clear";
	#endregion

	public static new tmManager Instance
	{ 
		get 
		{ 
			tmManager inst = SingletonMonoBehaviour<tmManager>.InstanceIfExist;
            #if UNITY_EDITOR
			if(inst == null)
			{
				GameObject manager = AssetUtility.GetAssetsAtPath<GameObject>("tmManager")[0];
				inst = manager.Clone().GetComponent<tmManager>();
				inst.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
            #endif
			return inst; 
		} 
	}


	#region Unity

    protected override void Awake()
    {
        base.Awake();

        #if UNITY_EDITOR
        if (Application.isPlaying)
        #endif
        {
            if (tmSettings.Instance.isAtlasesPreloadEnabled)
            {
                List<tmResourceCollectionLink> platformCollectionLinks = tmUtility.GetAllResourceLinks(tmSettings.Instance.CurrentPlatform);

                for (int i = 0; i < platformCollectionLinks.Count; ++i)
                {
                    platformCollectionLinks[i].collection.LoadTexture();
                    CustomDebug.LogFormat("Texture preload for {0}", platformCollectionLinks[i].collection.name);
                }
            }
        }
    }

//    void OnGUI()
//    {
//        if (GUI.Button(new Rect(100, 100, 100, 100), "unload"))
//        {
//            Resources.UnloadUnusedAssets();
//        }
//    }
//
// 	protected override void OnDestroy()
//	{
//		base.OnDestroy();
//
//		foreach(Mesh mesh in meshMap.Values)
//		{
//			if(mesh)
//			{
//				Debug.Log("Destroy : " + mesh.name);
//				if(Application.isPlaying)
//				{
//					Destroy(mesh);
//				}
//				else
//				{
//					DestroyImmediate(mesh);
//				}
//			}
//		}
//	}
	#endregion



	#region Public

	public void Clear()
	{
		meshMap.Clear();
		materialMap.Clear();
		collectionMap.Clear();
		mainTextureMap.Clear();
		lightmapMap.Clear();
	}


	public void ClearMaterials()
	{
		materialMap.Clear();
	}


	public void Flush () 
	{
		if(hasNew)
		{
			float time = Time.realtimeSinceStartup;
			foreach(tmIRebuildable render in modifiedRenders.ToArray())
			{
				if(!render.IsNull())
				{
					render.Rebuild();
				}

				modifiedRenders.Remove(render);
			}
			time = (Time.realtimeSinceStartup - time) * 1000;
//			Debug.Log("tmManager.Flush by : " + time.ToString("f10") + " miliseconds");

			hasNew = modifiedRenders.Count > 0;
		}
	}


	public void Flush (tmIRebuildable[] renders) 
	{
		foreach(tmIRebuildable render in renders)
		{
			if(!render.IsNull())
			{                
				render.Rebuild();
				modifiedRenders.Remove(render);
			}
		}

		hasNew = modifiedRenders.Count > 0;
	}


    public void PreloadAtlases(List<tmResourceCollectionLink> _links)
    {
        List<tmResourceCollectionLink> platformCollectionLinks = tmUtility.GetAllResourceLinksFor(tmSettings.Instance.CurrentPlatform, _links);

        for (int i = 0; i < platformCollectionLinks.Count; ++i)
        {            
            platformCollectionLinks[i].collection.LoadTexture();
            CustomDebug.LogFormat("Texture preload for {0}", platformCollectionLinks[i].collection.name);
        }
    }

	#endregion



	#region Handle tmIRebuildable objects

	public void RegisterModifiedRender(tmIRebuildable render)
	{
		modifiedRenders.Add(render);
		hasNew = true;

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.delayCall -= Flush;
		UnityEditor.EditorApplication.delayCall += Flush;
		#endif
	}

	#endregion



	#region Cached Materials


	/// <summary>
	/// Gets the shared material.
	/// </summary>
	/// <returns><c>true</c>, if shared material was created, <c>false</c> if already created.</returns>
	/// <param name="original">Original.</param>
	/// <param name="mainCollection">Main collection.</param>
	/// <param name="lightmapCollection">Lightmap collection.</param>
	/// <param name="hashKey">Hash key.</param>
	/// <param name="copy">Shared material.</param>
	public bool GetSharedMaterial(Material original, tmTextureCollectionPlatform mainCollection, tmTextureCollectionPlatform lightmapCollection, string hashKey, out Material copy)
	{	
		bool wasCreated = false;
		if(!materialMap.TryGetValue(hashKey, out copy))
		{
            copy = Instantiate(original);
			materialMap.Add(hashKey, copy);

			if(mainCollection != null)
			{
				copy.mainTexture = mainCollection.Atlas;

				List<Material> mats;
				if(!mainTextureMap.TryGetValue(mainCollection, out mats))
				{
					mats = new List<Material>();
					mainTextureMap.Add(mainCollection, mats);
				}

				mats.Add(copy);
			}

			if(lightmapCollection != null)
			{
				copy.SetLightmapTexture(lightmapCollection.Atlas);

				List<Material> mats;
				if(!lightmapMap.TryGetValue(lightmapCollection, out mats))
				{
					mats = new List<Material>();
					lightmapMap.Add(lightmapCollection, mats);
				}

				mats.Add(copy);
			}

			wasCreated = true;
		}

		return wasCreated;
	}

	#endregion



	#region Texture Managment
	public void TextureLoadedForCollection(tmTextureCollectionPlatform collection)
	{
		List<Material> mats;
		if(mainTextureMap.TryGetValue(collection, out mats))
		{
			mats.ForEach((m) =>
            {
                m.mainTexture = collection.Atlas;
            });
		}
		else if(lightmapMap.TryGetValue(collection, out mats))
		{
            mats.ForEach((m) =>
            {
                m.SetLightmapTexture(collection.Atlas);
            });
		}
	}


	public void TextureWillUnloadForCollection(tmTextureCollectionPlatform collection)
	{
		foreach(Material mat in materialMap.Values)
		{
			if(mat.mainTexture == collection.Atlas)
			{
				mat.mainTexture = null;
			}
			if(mat.GetLightmapTexture() == collection.Atlas)
			{
				mat.SetLightmapTexture(null);
			}
		}
	}
	#endregion



	#region Cached Meshes


	public void MeshInstance(Mesh original, out Mesh sharedMesh, tmTextureDefenition mainTextureDef, tmTextureDefenition lightmapTextureDef, bool makeNoLongerReadable)
	{
		string key = 
			"mesh" + 
			original.GetHashCode() + 
			(mainTextureDef != null ? mainTextureDef.textureGuid : "") +
			(lightmapTextureDef != null ? lightmapTextureDef.textureGuid : "");

		string keys = 	
			"" +		
			(mainTextureDef != null ? mainTextureDef.textureName : "") +
			(lightmapTextureDef != null ? lightmapTextureDef.textureName : "");

        sharedMesh = null;
        WeakReference meshRef = null;
        int originalMeshID = original.GetInstanceID();

        if (meshMap.TryGetValue(key, out meshRef))
        {
            if (meshRef.IsAlive && !meshRef.Target.IsNull())
            {
                sharedMesh = meshRef.Target as Mesh;
            }
            else
            {
                meshMap.Remove(key);
                destroyedMeshes.Remove(originalMeshID);
            }
        }

        if(sharedMesh == null)
        {
            if(Application.isPlaying && destroyedMeshes.ContainsKey(originalMeshID))
            {
                CustomDebug.LogError("duplicate : " + original.name + "  " + keys + " vs " + destroyedMeshes[originalMeshID]);
            }

            if(!original.isReadable)
            {
                CustomDebug.LogError("NOT READABLE = " + original.name);
            }

            sharedMesh = 
                #if UNITY_EDITOR
                Instantiate(original) as Mesh;
                #else
                keepImMemoryMeshes.Contains(original) ? (Instantiate(original) as Mesh) : original;
                #endif

            #if UNITY_EDITOR
            sharedMesh.hideFlags = HideFlags.HideAndDontSave;
            #endif

            sharedMesh.name = original.name + "_" + mainTextureDef.textureName;
            meshRef = new WeakReference(sharedMesh);
            meshMap.Add(key, meshRef);

            if(!keepImMemoryMeshes.Contains(original))
            {
                if(!destroyedMeshes.ContainsKey(originalMeshID))
                {
                    destroyedMeshes.Add(originalMeshID, keys);
                }
            }

            RemapUV(ref sharedMesh, mainTextureDef, lightmapTextureDef);
            if(makeNoLongerReadable)
            {
                sharedMesh.UploadMeshData(true);
            }
        }
	}


	public void RemapUV(ref Mesh mesh, tmTextureDefenition mainTextureDefenition, tmTextureDefenition lightmapTextureDefenition)
	{
		if(!tmSettings.Instance.rebuildMesh)
		{
			return;
		}

		//============================ recalculate uv ============================
		Vector2[] oldUVs = mesh.uv;
		if(oldUVs != null && oldUVs.Length > 0)
		{
			Vector2 oldUV, newUV;

			Rect uvRect = mainTextureDefenition.uv;
			Rect offset = mainTextureDefenition.offset;
			uvRect.center += offset.center;
			uvRect.size += offset.size;

			int vertexCount = oldUVs.Length;
			for (int i = 0; i < vertexCount; i++)
			{
				oldUV = oldUVs[i];
				newUV = new Vector2(
					uvRect.x + uvRect.width * oldUV.x, 
					uvRect.y + uvRect.height * oldUV.y
				);
				oldUVs[i] = newUV;
			}

			mesh.uv = oldUVs;
		}	

		//==========================recalculate uv2===============================
		if(lightmapTextureDefenition != null)
		{
			Vector2[] oldUVs2 = mesh.uv2;

			if(oldUVs2 != null && oldUVs2.Length > 0)
			{
				Rect uvRect = lightmapTextureDefenition.uv;
				Rect offset = lightmapTextureDefenition.offset;
				uvRect.center += offset.center;
				uvRect.size += offset.size;

				int vertexCount = oldUVs2.Length;
				for (int i = 0; i < vertexCount; i++)
				{
					Vector2 oldUV2 = oldUVs2[i];
					Vector2 newUV2 = new Vector2(
						uvRect.x + uvRect.width * oldUV2.x, 
						uvRect.y + uvRect.height * oldUV2.y
					);
					oldUVs2[i] = newUV2;
				}

				mesh.uv2 = oldUVs2;
			}
		}
	}

	#endregion



	#region Cached Collections

    public tmTextureCollectionPlatform GetMainTexPlatformCollection(string collectionGuid)
    {
		return GetPlatformCollectionWithFallback(collectionGuid, tmSettings.Instance.CurrentPlatform);
    }


	public tmTextureCollectionPlatform GetLightmapPlatformCollection(string collectionGuid)
	{
		return GetPlatformCollectionWithFallback(collectionGuid, tmSettings.Instance.LightmapPlatform);
	}


	public tmTextureCollectionPlatform GetPlatformCollectionWithFallback(string collectionGuid, tmPlatform platform)
	{
		tmTextureCollectionPlatform plTexColl = GetPlatformCollection(collectionGuid, platform);

		if (plTexColl == null)
		{
			foreach (var pl in tmSettings.allPlatfrorms)
			{
				plTexColl = GetPlatformCollection(collectionGuid, pl);
				if (plTexColl != null)
				{
					return plTexColl;
				}
			}
		}

		return plTexColl;
	}


	public tmTextureCollectionPlatform GetPlatformCollection(string collectionGuid, tmPlatform platform)
	{
		string collectionGuidPath = tmUtility.PathForPlatform(collectionGuid, platform);

        if(collectionMap.ContainsKey(collectionGuidPath))
		{
            return collectionMap[collectionGuidPath];
		}
		else
		{
            tmResourceCollectionLink link = tmUtility.ResourceLinkByGUID(collectionGuidPath);

            if (link != null)
            {
				#if UNITY_EDITOR
				if(!Application.isPlaying)
				{
					link.collection.LoadTexture();
				}
				#endif

                collectionMap.Add(collectionGuidPath, link.collection);
                return link.collection;
            }
		}

		return null;
	}

	#endregion

}
