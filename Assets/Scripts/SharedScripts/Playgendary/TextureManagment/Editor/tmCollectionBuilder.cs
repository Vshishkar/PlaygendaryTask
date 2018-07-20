using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public static class tmCollectionBuilder
{
	public static void BuildCollection(tmTextureCollection collection)
	{
		if(collection != null)
		{
			collection.textureDefenitions.RemoveAll(f => !collection.Textures.Contains(f.texture));
			foreach(Texture2D texture in collection.Textures)
			{
				if(texture != null)
				{
					tmTextureDefenition textureRef = collection.textureDefenitions.Find(f => f.texture == texture);
					if(textureRef == null)
					{
						textureRef = new tmTextureDefenition();
						textureRef.texture = texture;
						textureRef.textureName = tmUtility.PlatformlessPath(texture.name); // should attach to name ? or guid better

						textureRef.textureGuid = tmEditorUtility.AssetToGUID(texture);
						textureRef.assetGuid = tmEditorUtility.AssetToGUID(texture);

						collection.textureDefenitions.Add(textureRef);
					}
				}
			}

			EditorUtility.SetDirty(collection);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			BuildPlatformCollections(collection);

			collection = tmEditorUtility.GUIDToAsset(collection.assetGuid, typeof(tmTextureCollection)) as tmTextureCollection;
			tmIndex.Instance.RegisterCollection(collection);
		}
	}


	public static void BuildPlatformCollections(tmTextureCollection collection)
	{
		if(collection != null)
		{
			List<tmTextureCollectionPlatform> modifiedCollections = new List<tmTextureCollectionPlatform>();

			foreach(tmPlatform platform in tmSettings.allPlatfrorms)
			{
                if (collection.forcedScale > 0 && Mathf.Abs(platform.scale - collection.forcedScale) > float.Epsilon)
                {
                    continue;
                }

				tmTextureCollectionPlatformRef collectionPlatformRef = collection.platforms.Find(f => (f != null) && f.platform.Equals(platform));
				if(collectionPlatformRef == null) 
				{
					collectionPlatformRef = new tmTextureCollectionPlatformRef();
					collectionPlatformRef.platform = platform;
					collection.platforms.Add(collectionPlatformRef);
				}

				tmTextureCollectionPlatform collectionPlatform = collectionPlatformRef.collectionPlatform;

				if(collectionPlatform == null)
				{
					collectionPlatform = tmCollectionBuilder.CreatePlatformCollectionFromMainCollection(collection, platform);
				}

				collectionPlatform.platform = platform; // just update platform info
				collectionPlatform.importSettings = collection.importSettings;
                collectionPlatform.useStreamingAssets = collection.useStreamingAssets;

				collectionPlatformRef.platform = platform;
				collectionPlatformRef.collectionName = collectionPlatform.name;
				collectionPlatformRef.collectionGuid = collectionPlatform.collectionGuid;
				collectionPlatformRef.assetnGuid = collectionPlatform.assetGuid;
				collectionPlatformRef.collectionPlatform = collectionPlatform;

				tmCollectionBuilder.UpdatePlatformCollection(collectionPlatform, collection.Textures);
				modifiedCollections.Add(collectionPlatform);
			}

			UpdateRendersInScene(modifiedCollections);

			EditorUtility.SetDirty(collection);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}


	public static tmTextureCollectionPlatform CreatePlatformCollectionFromMainCollection(tmTextureCollection collection, tmPlatform platform)
	{
		string mainCollectionPath = AssetDatabase.GetAssetPath(collection);
		string directory = System.IO.Path.GetDirectoryName(mainCollectionPath);
		string platformDirectory = directory + "/" + collection.name + platform.postfix;
		string platformPath = platformDirectory + "/" + collection.name + platform.postfix + ".prefab";

		if(!System.IO.Directory.Exists(platformDirectory))
		{
			System.IO.Directory.CreateDirectory(platformDirectory);
		}

		tmTextureCollectionPlatform collectionPlatform = AssetDatabase.LoadAssetAtPath(platformPath, typeof(tmTextureCollectionPlatform)) as tmTextureCollectionPlatform;

		if(collectionPlatform == null)
		{
			GameObject g = new GameObject();
			g.AddComponent<tmTextureCollectionPlatform>();
			GameObject prefab = PrefabUtility.CreatePrefab(platformPath, g);
			Object.DestroyImmediate(g);

			collectionPlatform = prefab.GetComponent<tmTextureCollectionPlatform>();
			collectionPlatform.assetGuid = tmEditorUtility.AssetToGUID(prefab);
			collectionPlatform.collectionGuid = collection.collectionGuid + platform.postfix;
		}

		return collectionPlatform;
	}


	public static void UpdatePlatformCollection(tmTextureCollectionPlatform collectionPlatform, List<Texture2D> originalTextures)
	{
		collectionPlatform.Textures.Clear();
		collectionPlatform.textureDefenitions.RemoveAll( def => !originalTextures.Contains(def.texture));
		foreach (var texture in originalTextures) 
		{
			if(texture != null)
			{
				tmTextureDefenition textureRef = collectionPlatform.textureDefenitions.Find(f => f.texture == texture);
				if(textureRef == null)
				{
					textureRef = new tmTextureDefenition();
					textureRef.texture = texture;
					textureRef.textureName = tmUtility.PlatformlessPath(texture.name);
					textureRef.textureGuid = tmEditorUtility.AssetToGUID(texture);
					textureRef.assetGuid = tmEditorUtility.AssetToGUID(texture);
					collectionPlatform.textureDefenitions.Add(textureRef);
				}

				string originalPath = AssetDatabase.GetAssetPath(texture);
				ConfigureSpriteTextureImporter(originalPath);

				bool rebuild = false;
				ulong originalTimestamp = tmEditorUtility.Hash(texture);
				rebuild |= (originalTimestamp != textureRef.originalTimestamp);
				if(textureRef.platformTexture != null)
				{
					ulong platformTimestamp = tmEditorUtility.Hash(textureRef.platformTexture);
					rebuild |= (platformTimestamp != textureRef.platformTimestamp);
				}

				Texture2D asset = PlatformSpecifiedTexture(textureRef.texture, collectionPlatform.platform, rebuild);
				if(asset != null)
				{
					string platformTexturePath = AssetDatabase.GetAssetPath(asset);
					ConfigureSpriteTextureImporter(platformTexturePath);

					textureRef.platformTexture = asset;
					textureRef.textureName = tmUtility.PlatformlessPath(textureRef.platformTexture.name);
					textureRef.assetGuid = tmEditorUtility.AssetToGUID(textureRef.platformTexture);
					textureRef.platformTimestamp = tmEditorUtility.Hash(textureRef.platformTexture);
					textureRef.originalTimestamp = originalTimestamp;
				}
			}
		}

		collectionPlatform.textureDefenitions.Sort( (a, b) => (string.Compare(a.textureName, b.textureName, System.StringComparison.OrdinalIgnoreCase)));

		EditorUtility.SetDirty(collectionPlatform);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

        tmCollectionBuilder.BuildAtlas(collectionPlatform);       
		
		CreatePlatformResourceLink(collectionPlatform);

		collectionPlatform = tmEditorUtility.GUIDToAsset(collectionPlatform.assetGuid, typeof(tmTextureCollectionPlatform)) as tmTextureCollectionPlatform;
		tmIndex.Instance.RegisterPlatformCollection(collectionPlatform);
	}


	static Texture2D PlatformSpecifiedTexture(Texture2D mainTexture, tmPlatform platform, bool force = false)
	{
		string texturePath = AssetDatabase.GetAssetPath(mainTexture);
		string directory = System.IO.Path.GetDirectoryName(texturePath);

		string textureFileName = System.IO.Path.GetFileName(texturePath);
		string platformDirectory = directory + "/" + platform.postfix.Replace("@", "");
		string platformPath = platformDirectory + "/" + textureFileName;

		if(platform.Equals(tmSettings.Instance.DefaultPlatform))
		{
			return mainTexture;
		}

		if(!System.IO.Directory.Exists(platformDirectory))
		{
			System.IO.Directory.CreateDirectory(platformDirectory);
		}

		Texture2D asset = AssetDatabase.LoadAssetAtPath(platformPath, typeof(Texture2D)) as Texture2D;
		if(asset == null || force)
		{
			AssetDatabase.DeleteAsset(platformPath);
			AssetDatabase.CopyAsset(texturePath, platformPath);
            CustomDebug.Log("Create texture : " + mainTexture.name + platform.postfix);

//			int width = mainTexture.width / 2;
//			int height = mainTexture.height / 2;
//
//			asset = new Texture2D(width, height);
//			for(int i = 0; i < width; i++)
//			{
//				for(int j = 0; j < height; j++)
//				{
//					asset.SetPixel(i,j, mainTexture.GetPixel(i*2,j*2));
//				}
//			}
//			asset.Apply();

			asset = tmEditorUtility.RescaleTexture(mainTexture, platform.scale/tmSettings.Instance.DefaultPlatform.scale); 

			var bytes = asset.EncodeToPNG();
			System.IO.File.WriteAllBytes(platformPath, bytes);
			Object.DestroyImmediate(asset);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			AssetDatabase.ImportAsset(platformPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer);

			asset = AssetDatabase.LoadAssetAtPath(platformPath, typeof(Texture2D)) as Texture2D;
		}

		return asset;
	}
   
	public static void BuildAtlas(tmTextureCollectionPlatform collectionPlatform)
	{
		string collectionPath = AssetDatabase.GetAssetPath(collectionPlatform);
		string collectionDirectory = System.IO.Path.GetDirectoryName(collectionPath);
		string collectionFileName = System.IO.Path.GetFileNameWithoutExtension(collectionPath);
        string atlasName = collectionFileName + "_atlas.png";

        string collectionAtlasPath;
        if (collectionPlatform.useStreamingAssets)
        {
			collectionAtlasPath = "Assets/StreamingAssets/" + tmManager.stramingAssetsDirectory + "/" + atlasName;
        }
        else
        {
			collectionAtlasPath = collectionDirectory + "/" + atlasName;
        }

		Texture2D atlas = new Texture2D(4096, 4096, collectionPlatform.importSettings.format, collectionPlatform.importSettings.generateMipMaps);

		List<Texture2D> textures = new List<Texture2D>();
		foreach(tmTextureDefenition td in collectionPlatform.textureDefenitions)
		{
			td.uv = new Rect();
			textures.Add(td.platformTexture);
		}

		Rect[] spritesRect = atlas.PackTextures(textures.ToArray(), collectionPlatform.importSettings.border, 4096, false);
		for (int i = 0; i < spritesRect.Length; i++)
		{
			collectionPlatform.textureDefenitions[i].uv = spritesRect[i];
			Rect offset = new Rect();
			offset.size = collectionPlatform.textureDefenitions[i].offset.size * 1f/atlas.width;
			offset.center = collectionPlatform.textureDefenitions[i].offset.center * 1f/atlas.width;
			collectionPlatform.textureDefenitions[i].offset = offset;
		}

        collectionPlatform.AtlasAssetGUID = AssetDatabase.AssetPathToGUID(collectionAtlasPath);

        var bytes = atlas.EncodeToPNG();
        System.IO.File.WriteAllBytes(collectionAtlasPath, bytes);


        if (!collectionPlatform.useStreamingAssets)
        {
            int startIndex = collectionAtlasPath.IndexOf("Resources/", System.StringComparison.CurrentCulture) + 10;
            int lastIndex = collectionAtlasPath.LastIndexOf('.');
			collectionPlatform.AssetPath = collectionAtlasPath.Substring(startIndex, lastIndex - startIndex);    

            AssetDatabase.Refresh();

            TextureImporter tImporter = AssetImporter.GetAtPath(collectionAtlasPath) as TextureImporter;
            #if UNITY_5_5_OR_NEWER
            tImporter.textureType = TextureImporterType.Default;
            #else
            tImporter.textureType = TextureImporterType.Advanced;
            #endif
            tImporter.isReadable = false;
            tImporter.maxTextureSize = 4096;
            tImporter.mipmapEnabled = collectionPlatform.importSettings.generateMipMaps;
            #if UNITY_5_5_OR_NEWER
            tImporter.textureCompression = TextureImporterCompression.Uncompressed;

            var defaultPlatform = tImporter.GetDefaultPlatformTextureSettings();
            defaultPlatform.format = collectionPlatform.importSettings.importFormat;
            tImporter.SetPlatformTextureSettings(defaultPlatform);
            #else
            tImporter.textureFormat = collectionPlatform.importSettings.importFormat;
            #endif
            AssetDatabase.ImportAsset(collectionAtlasPath, ImportAssetOptions.ForceUpdate);
        }
        else
        {
			collectionPlatform.AssetPath = atlasName;
        }
            
        if(collectionPlatform.Atlas != null)
        {
            TextureCache.UnloadTexture(collectionPlatform.Atlas);
            collectionPlatform.Atlas = null;
            collectionPlatform.LoadTexture();
        }

        Object.DestroyImmediate(atlas);

		EditorUtility.SetDirty(collectionPlatform);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}


	public static void BuildCollectionsForModifiedAssets(string[] importedAssets)
	{
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		List<tmTextureCollection> modifiedCollections = new List<tmTextureCollection>();

		foreach(string path in importedAssets)
		{
			tmTextureCollectionIndex index = tmIndex.Instance.CollectionIndexForTexturePath(path);
			if(index != null)
			{
				tmTextureCollection coll = tmEditorUtility.GUIDToAsset(index.assetGUID, typeof(tmTextureCollection)) as tmTextureCollection;

				if(coll != null && !modifiedCollections.Contains(coll))
				{
					modifiedCollections.Add(coll);
				}
			}
		}

		foreach(tmTextureCollection collection in modifiedCollections)
		{
			BuildCollection(collection);
		}
	}


	public static void UpdateRendersInScene(List<tmTextureCollectionPlatform> modifiedCollections)
	{
		tmManager.Instance.Clear();

		List<tmTextureRender> renders = GameObjectExtension.GetAllObjectsInScene<tmTextureRender>();
		renders.ForEach(
			f =>
			{
				f.ModifiedFlag |= tmTextureRender.ModifiedFlags.ModifiedMesh;
				f.ModifiedFlag |= tmTextureRender.ModifiedFlags.ModifiedUV1;
				f.ModifiedFlag |= tmTextureRender.ModifiedFlags.ModifiedUV2;
				f.ModifiedFlag |= tmTextureRender.ModifiedFlags.ModifiedMaterial;
			}
		);
	}


	public static bool ConfigureSpriteTextureImporter(string assetPath)
	{
		// make sure the source texture is npot and readable, and uncompressed
		TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        if (importer.mipmapEnabled ||
            #if UNITY_5_5_OR_NEWER
            importer.textureType != TextureImporterType.Default ||
            importer.textureCompression != TextureImporterCompression.Uncompressed ||
            importer.GetDefaultPlatformTextureSettings().format != TextureImporterFormat.RGBA32 ||
            #else
            importer.textureType != TextureImporterType.Advanced ||
            importer.textureFormat != TextureImporterFormat.AutomaticTruecolor ||
            #endif
			importer.npotScale != TextureImporterNPOTScale.None ||
			!importer.isReadable ||
			//			!importer.alphaIsTransparency ||
			importer.maxTextureSize < 4096 ||
			importer.wrapMode != TextureWrapMode.Clamp
		)
		{
            #if UNITY_5_5_OR_NEWER
            importer.textureType = TextureImporterType.Default;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            var defaultPlatform = importer.GetDefaultPlatformTextureSettings();
            defaultPlatform.format = TextureImporterFormat.RGBA32;
            importer.SetPlatformTextureSettings(defaultPlatform);
            #else
            importer.textureType = TextureImporterType.Advanced;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            #endif
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.isReadable = true;
			importer.mipmapEnabled = false;
			importer.maxTextureSize = 4096;
			//			importer.alphaIsTransparency = true;

			AssetDatabase.ImportAsset(assetPath);

			return true;
		}

		return false;
	}


	public static void CreatePlatformResourceLink(tmTextureCollectionPlatform collectionPlatform)
	{
		if(!System.IO.Directory.Exists(tmSettings.AssetsFolder))
		{
			System.IO.Directory.CreateDirectory(tmSettings.AssetsFolder);
		}

		string assetPath = tmSettings.AssetsFolder + tmSettings.ResourceLinkPrefix + collectionPlatform.collectionGuid + ".asset";
		string resourcePath = tmSettings.ResourceFolder + tmSettings.ResourceLinkPrefix + collectionPlatform.collectionGuid;

		tmResourceCollectionLink link = Resources.Load(resourcePath, typeof(tmResourceCollectionLink)) as tmResourceCollectionLink;
		if(link == null)
		{
			link = ScriptableObject.CreateInstance(typeof(tmResourceCollectionLink)) as tmResourceCollectionLink;
			AssetDatabase.CreateAsset(link, assetPath);
		}

		link.collectionInEditor = collectionPlatform;
		UpdateResourceLink(link);


		EditorUtility.SetDirty(link);
		AssetDatabase.SaveAssets();
	}


	public static void ValidateResourceLinks()
	{
		List<tmResourceCollectionLink> resourceLink = AssetUtility.GetAssetsAtPath<tmResourceCollectionLink>("tmResources");
		List<tmResourceCollectionLink> assetsForRemove = new List<tmResourceCollectionLink>();

		foreach (var item in resourceLink) 
		{
			if(item.collectionInEditor == null)
			{
				assetsForRemove.Add(item);
			}
			else
			{
				tmCollectionBuilder.UpdateResourceLink(item);
			}
		}

		foreach(tmResourceCollectionLink link in assetsForRemove)
		{
			Object.DestroyImmediate(link, true);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}


	public static void UpdateResourceLink(tmResourceCollectionLink link)
	{
		link.collection = link.collectionInEditor;
		if(tmSettings.Instance.TargetPlatform.name.Equals("iPhone") && link.collection.platform.name.Equals("iPad Retina"))
		{
			link.collection = null;
		}

		EditorUtility.SetDirty(link);
	}
}
