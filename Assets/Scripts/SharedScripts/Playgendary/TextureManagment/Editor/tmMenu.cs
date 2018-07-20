using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


public class tmMenu {

	[MenuItem("Assets/Create/tm/TextureAtlas", false, 10000)]
	public static void CreateAtlas() 
	{
		GameObject g = new GameObject();
		g.name = "New Collection";
		g.AddComponent<tmTextureCollection>();

		string path = tmEditorUtility.GetNewPrefabPath("New Collection");
		GameObject prefab = PrefabUtility.CreatePrefab(path, g);

		prefab.GetComponent<tmTextureCollection>().assetGuid = tmEditorUtility.AssetToGUID(prefab);
		prefab.GetComponent<tmTextureCollection>().collectionGuid = tmUtility.NewGUID();

		Object.DestroyImmediate(g);
	}


	static List<tmTextureCollection> collections;

	[MenuItem("TextureManager/Rebuild Index", false, 10000)]
	public static void RebuildIndex() 
	{
		EditorUtility.DisplayProgressBar("Texture Manager", "scanning for collections...", 0);

		collections = AssetUtility.GetAssetsAtPath<tmTextureCollection>("TextureManagmentCollections");

		EditorUtility.DisplayProgressBar("Texture Manager", "collections founded : " + collections.Count , 1);
		System.Threading.Thread.Sleep(500);

		tmIndex.Instance.TextureCollections.Clear();
		tmIndex.Instance.TexturePlatformCollections.Clear();

		int current = 0;
		for (int i = 0; i < collections.Count; i++) 
		{
			tmTextureCollection collection = collections[i];

			if(!collection.IsNull())
			{
				CustomDebug.Log("Rebuild collection : " + collection.name);

				try
				{
					EditorUtility.DisplayProgressBar("Texture Manager", "building : " + collection.name, current++ * 1f / collections.Count);

					tmCollectionBuilder.BuildCollection(collection);

					Resources.UnloadUnusedAssets();
					System.GC.Collect();
				}
				catch (System.Exception ex)
				{
					CustomDebug.Log(ex);
				}
			}
		}

		EditorUtility.SetDirty(tmIndex.Instance);
//		tmCollectionBuilder.ValidateResourceLinks();

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		collections.Clear();
		EditorUtility.ClearProgressBar();

		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}



	public static Mesh GetSharedMesh(GameObject g)
	{
		MeshFilter filter = g.GetComponent<MeshFilter>();
		if(filter != null)
		{
			return filter.sharedMesh;
		}

		SkinnedMeshRenderer smr = g.GetComponent<SkinnedMeshRenderer>();
		if(smr != null)
		{
			return smr.sharedMesh;
		}

		return null; 
	}


	public static void SetSharedMesh(GameObject g, Mesh mesh)
	{

		MeshFilter filter = g.GetComponent<MeshFilter>();

		if(filter != null)
		{
			filter.sharedMesh = mesh;
		}
		else 
		{
			SkinnedMeshRenderer smr = g.GetComponent<SkinnedMeshRenderer>();
			if(smr != null)
			{
				smr.sharedMesh = mesh;
			}
		}
	}


	[MenuItem("TextureManager/Add Texture Render  %#r", false, 10000)]
	public static void AddTextureRender() 
	{
		foreach(GameObject target in Selection.gameObjects)
		{
			target.AddComponent<tmTextureRender>();
		}
	}


	[MenuItem("TextureManager/Convert  %#x", false, 10000)]
	public static void Convert() 
	{
		foreach(GameObject target in Selection.gameObjects)
		{
			Convert(target);
		}
	}


    [MenuItem("TextureManager/Convert Recursively", false, 10000)]
    public static void ConvertRecursively()
    {
        List<Renderer> renderers = new List<Renderer>();
        foreach (GameObject target in Selection.gameObjects)
        {
            renderers.AddRange(target.GetComponentsInChildren<Renderer>());
        }

        foreach (Renderer renderer in renderers)
        {
            Convert(renderer.gameObject);
        }
    }


	public static void Convert(GameObject target) 
	{
		//			GameObject target = Selection.activeGameObject;
		Mesh mesh = null;
		MeshFilter mf = target.GetComponent<MeshFilter>();
		bool isSkinned = false;
		if(mf != null)
		{
			mesh = mf.sharedMesh;
		}
		else
		{
			SkinnedMeshRenderer smr = target.GetComponent<SkinnedMeshRenderer>();
			if(smr != null)
			{
				isSkinned = true;
				mesh = smr.sharedMesh;
			}
		}

		int index = 0;
		for (int i = 0; i < target.GetComponent<Renderer>().sharedMaterials.Length; i++) 
		{
			Material mat = target.GetComponent<Renderer>().sharedMaterials[i];
			Texture main = mat.mainTexture;

			Texture lightmap = mat.GetLightmapTexture();
			if (mat.name.Equals("Shadows_LOW")) 
			{
				main = AssetUtility.GetAssetsAtPath<Texture2D>("Obstacles/Shadow_LOW")[0];
				mat = AssetUtility.GetAssetsAtPath<Material>("Materials/Shadows")[0];
			}
			if (mat.name.Equals("Shadows_MEDIUM")) 
			{
				main = AssetUtility.GetAssetsAtPath<Texture2D>("Obstacles/Shadow_MEDIUM")[0];
				mat = AssetUtility.GetAssetsAtPath<Material>("Materials/Shadows")[0];
			}

			GameObject g = null;
			if (!isSkinned && target.GetComponent<Renderer>().sharedMaterials.Length > 1) 
			{
				g = new GameObject(target.name + index, typeof(MeshRenderer), typeof(MeshFilter));
				g.name = target.name + index;
				g.transform.parent = target.transform;
				g.transform.localPosition = Vector3.zero;
				g.transform.localScale = Vector3.one;
				g.transform.localRotation = Quaternion.identity;
			}
			else 
			{
				g = target;
			}

            tmTextureRenderBase render = null;
			g.GetComponent<Renderer>().sharedMaterial = mat;

            if (g.GetComponent<ParticleSystem>())
            {
                render = g.AddComponent<tmParticleSystemRender>();
            }

			if (main != null) 
			{
				string path = AssetDatabase.GetAssetPath(main);
				tmTextureCollectionIndex inst = tmIndex.Instance.CollectionIndexForTexturePath(path);
				if (inst != null) 
				{
					if (render == null) 
					{
						render = g.AddComponent<tmTextureRender>();
					}
					render.MainTexCollectionGUID = inst.textureCollectionGUID;
					render.MainTextureID = render.MainTexCollection.GetTextureDefenitionByName(main.name).textureGuid;
				}
			}

			if (lightmap != null) 
			{
				string path = AssetDatabase.GetAssetPath(lightmap);
				tmTextureCollectionIndex inst = tmIndex.Instance.CollectionIndexForTexturePath(path);
				if (inst != null)
				{
					if (render == null)
					{
						render = g.AddComponent<tmTextureRender>();
					}
					render.LightmapCollectionGUID = inst.textureCollectionGUID;
					render.LightmapTextureID = render.LightmapCollection.GetTextureDefenitionByName(lightmap.name).textureGuid;
				}
			}

			if (target.GetComponent<Renderer>().sharedMaterials.Length == 1) {
				SetSharedMesh(g, mesh);
				if (render != null)
					render.Mesh = mesh;
			}
			else
				if (mesh != null) {
					string meshName = mesh.name + "_" + index;
					string[] guids = AssetDatabase.FindAssets(meshName);
					if (guids.Length > 0) {
						int indexCount = mesh.GetIndices(index).Length;
						Object[] meshes = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guids[0]));
						foreach (Object submesh in meshes) {
							Mesh subm = submesh as Mesh;
							if (subm != null)
								if (subm.GetIndices(0).Length == indexCount) {
									SetSharedMesh(g, subm);
									if (render != null)
										render.Mesh = subm;
									g.name = subm.name;
								}
						}
					}
				}
			if (render != null) {
				render.Material = tmMaterialUtility.SharedMaterial(mat);
			}
			{
				tmBatchObject bb = g.GetComponent<tmBatchObject>();
				if (bb == null) {
					bb = g.AddComponent<tmBatchObject>();
				}
				bb.SetUp();
			}
			index++;
		}
	}



	[MenuItem("TextureManager/Utils/Separete mesh by submeshes", false, 10000)]
	public static void SplitMesh() 
	{
		foreach(Object selected in Selection.objects)
		{
			var mesh = selected as Mesh;
			if(mesh != null)
			{
				if(mesh != null && mesh.subMeshCount > 1)
				{
					tmEditorUtility.SplitMesh(mesh);
				}
			}
			else if(selected != null)
			{
				string path = AssetDatabase.GetAssetPath(selected);
				string ext = System.IO.Path.GetExtension(path);
				if(ext.Equals(".fbx"))
				{
					Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

					for (int i = 0; i < subAssets.Length; i++)
					{
						var mesh2 = selected as Mesh;
						if(mesh2 != null)
						{
							if(mesh2 != null && mesh2.subMeshCount > 1)
							{
								tmEditorUtility.SplitMesh(mesh2);
							}
						}
					}
				}
			}
		}
	}


	[MenuItem("TextureManager/Batching/Static batch selected", false, 10000)]
	public static void StaticBatch() 
	{
		tmBatchingManager.Instance.StaticBatch(Selection.activeGameObject);
	}


	[MenuItem("TextureManager/Batching/Create static copy for selected", false, 10000)]
	public static void StaticCopy() 
	{
		BuildStaticGeometryForObjects(Selection.gameObjects);
	}


	[MenuItem("TextureManager/Batching/Rebuild static geometry", false, 10000)]
	public static void StaticPrebuild() 
	{
		tmIndex.Instance.staticPrefabs.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
		BuildStaticGeometryForObjects(tmIndex.Instance.staticPrefabs.ToArray());
	}


	public static void BuildStaticGeometryForObjects(GameObject[] gameObjects)
	{
		EditorUtility.DisplayProgressBar("Static batching...", "scanning", 0);
		int total = gameObjects.Length;
		int current = 0;

        CustomDebug.Log("BuildStaticGeometryForObjects");
		CustomDebug.Log("Total objects for static batching : " + gameObjects.Length);

		try
		{
			foreach(GameObject sourceObject in gameObjects)
			{
				EditorUtility.DisplayProgressBar("Static batching...", sourceObject.name, current++ * 1f/total);

				tmStaticBatchedGameObject bo = sourceObject.GetComponent<tmStaticBatchedGameObject>();

				string staticPath = string.Empty;
				if((bo != null) && (!string.IsNullOrEmpty(bo.StaticLink.FullPath)))
				{
					staticPath = bo.StaticLink.FullPath;
				}
				else
				{
					string sourcePath = AssetDatabase.GetAssetPath(sourceObject);
					staticPath = sourcePath.Replace(".prefab", "_static.prefab");
				}
				CustomDebug.Log("static batch : " + staticPath);

				GameObject targetObject = sourceObject;
				if(PrefabUtility.GetPrefabType(targetObject) == PrefabType.Prefab)
					targetObject = PrefabUtility.InstantiatePrefab(targetObject) as GameObject;

				tmBatchingManager.Instance.StaticBatch(targetObject);
				targetObject.name = sourceObject.name + "_static";

				if(sourceObject != targetObject)
				{
                    GameObject result = PrefabUtility.CreatePrefab(staticPath, targetObject);
                    tmStaticBatchedGameObject sb = result.GetComponent<tmStaticBatchedGameObject>();
                    if(sb != null)
                    {
                        Object.DestroyImmediate(sb, true);
                    }
					Object.DestroyImmediate(targetObject);
				}
			}
		}
		catch (System.Exception ex)
		{
			EditorUtility.ClearProgressBar();
			throw ex;
		}
		finally
		{
			EditorUtility.ClearProgressBar();
		}

		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}



	[MenuItem("Assets/Print dependent prefabs", false, 10000)]
	public static void PrintDependency() 
	{
		List<GameObject> prefabs = AssetUtility.GetAssetsAtPath<GameObject>(".prefab");
		FindReferences(prefabs.ToArray(), Selection.activeObject);
	}


	private static void FindReferences(GameObject[] objects, Object target)
	{
		bool any = false;

		foreach (var go in objects)
		{
			var components = go.GetComponentsInChildren<Component>(true);

			foreach (var c in components)
			{
				if (!c)
				{
					CustomDebug.LogError("Missing Component in GO: " + tmEditorUtility.FullPath(go), go);
					continue;
				}

				SerializedObject so = new SerializedObject(c);
				var sp = so.GetIterator();

				while (sp.NextVisible(true))
				{
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if (sp.objectReferenceValue == target)
						{
							CustomDebug.Log(tmEditorUtility.FullPath(c.gameObject), go);
							any = true;
						}
					}
				}
			}
		}

		if(!any)
		{
			CustomDebug.Log("Nothing found");
		}
	}



	[MenuItem("TextureManager/Validate Meshes", false, 10000)]
	public static void ValidateMeshes() 
	{
		List<GameObject> prefabs = AssetUtility.GetAssetsAtPath<GameObject>(".prefab");

		foreach (GameObject go in prefabs)
		{
			var components = go.GetComponentsInChildren<tmTextureRenderBase>(true);

			foreach (tmTextureRenderBase tmr in components)
			{
				if(tmr.Mesh != null)
				{

				}
			}
		}
	}



	[MenuItem("TextureManager/Remove unused materials", false, 10000)]
	public static void RemoveMaterials() 
	{
		HashSet<Object> usedObjects = new HashSet<Object>();

		//search in assets
		List<GameObject> prefabs = AssetUtility.GetAssetsAtPath<GameObject>(".prefab");
		foreach (var go in prefabs)
		{
			var components = go.GetComponentsInChildren<Component>(true);

			foreach (var c in components)
			{
				if (!c)
				{
					CustomDebug.LogError("Missing Component in GO: " + tmEditorUtility.FullPath(go), go);
					continue;
				}

				SerializedObject so = new SerializedObject(c);
				var sp = so.GetIterator();

				while (sp.NextVisible(true))
				{
					if (sp.propertyType == SerializedPropertyType.ObjectReference)
					{
						if(!usedObjects.Contains(sp.objectReferenceValue))
						{
							usedObjects.Add(sp.objectReferenceValue);
						}
					}
				}
			}
		}

		//search in scenes
		foreach (var scene in EditorBuildSettings.scenes)
		{
            #if UNITY_5_4_OR_NEWER
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);
            #else
            EditorApplication.OpenScene(scene.path);
            #endif
			GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();
			foreach (var go in sceneObjects)
			{
				var components = go.GetComponentsInChildren<Component>(true);

				foreach (var c in components)
				{
					if (!c)
					{
						CustomDebug.LogError("Missing Component in GO: " + tmEditorUtility.FullPath(go), go);
						continue;
					}

					SerializedObject so = new SerializedObject(c);
					var sp = so.GetIterator();

					while (sp.NextVisible(true))
					{
						if (sp.propertyType == SerializedPropertyType.ObjectReference)
						{
							if(!usedObjects.Contains(sp.objectReferenceValue))
							{
								usedObjects.Add(sp.objectReferenceValue);
							}
						}
					}
				}
			}
		}


        CustomDebug.Log(usedObjects.Count);

		List<Material> materials = AssetUtility.GetAssetsAtPath<Material>(".mat");
		foreach (var mat in materials)
		{
			if(!usedObjects.Contains(mat))
			{
                CustomDebug.Log("unused material " + mat.name, mat);
				string path = AssetDatabase.GetAssetPath(mat);
				AssetDatabase.DeleteAsset(path);
			}
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
//
//	[MenuItem("TextureManager/Fix %#t", false, 10000)]
//	public static void Fix() 
//	{
//		List<Material> mats = AssetUtility.GetAssetsAtPath<Material>("");
//		foreach (var m in mats) 
//		{
//			bool needFix = false;
//
//			SerializedObject obj = new SerializedObject(m);
//			var sp = obj.GetIterator();
//			while (sp.NextVisible(true))
//			{
//				if(sp.propertyPath.Contains(".first.name"))
//				{
//					if(sp.stringValue.Equals("_FogDistance") || sp.stringValue.Equals("_StartFogDistance"))
//					{
//						needFix = true;
//						break;
//					}
//				}
//			}
//
//			if(needFix)
//			{
//				string assetPath = AssetDatabase.GetAssetPath(m);
//				string fileFullPath = Application.dataPath.Replace("Assets", "") + assetPath;
//				Debug.Log(fileFullPath);
//
//				FileInfo fileInfo = new FileInfo(fileFullPath);
//				StreamReader streamReader = fileInfo.OpenText();
//				string content = streamReader.ReadToEnd();
//				streamReader.Close();
//
//				content = content.Replace("      data:\n        first:\n          name: _FogDistance\n        second: 150\n", "");
//				content = content.Replace("      data:\n        first:\n          name: _StartFogDistance\n        second: 20\n", "");
//
//				content = content.Replace("      data:\n        first:\n          name: _FogDistance\n        second: 100\n", "");
//				content = content.Replace("      data:\n        first:\n          name: _StartFogDistance\n        second: 70\n", "");
//
//				StreamWriter streamWriter = new StreamWriter(fileFullPath);
//				streamWriter.Write(content);
//				streamWriter.Close();
//			}
//		}
//
//		AssetDatabase.Refresh();
//		AssetDatabase.SaveAssets();
//	}
}
