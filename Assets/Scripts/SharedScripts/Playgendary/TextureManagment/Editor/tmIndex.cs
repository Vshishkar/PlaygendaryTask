using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class tmTextureCollectionIndex
{
	public string name;
	public string assetGUID;
	public string textureCollectionGUID;
	public string[] textureNames = new string[0];
	public string[] textureGUIDs = new string[0];
}



public class tmIndex : ScriptableSingleton<tmIndex> 
{
	[SerializeField] List<tmTextureCollectionIndex> textureCollections = new List<tmTextureCollectionIndex>();
	[SerializeField] List<tmTextureCollectionIndex> texturePlatformCollections = new List<tmTextureCollectionIndex>();
	#if UNITY_EDITOR
	public List<GameObject> staticPrefabs = new List<GameObject>();
    #endif

	public List<tmTextureCollectionIndex> TextureCollections {
		get {
			return textureCollections;
		}
	}


	public List<tmTextureCollectionIndex> TexturePlatformCollections {
		get {
			return texturePlatformCollections;
		}
	}



	#region Public

	public void RegisterCollection(tmTextureCollectionBase collection)
	{
		RegisterCollection(collection, tmIndex.Instance.TextureCollections);
	}


	public void RegisterPlatformCollection(tmTextureCollectionBase collection)
	{
		RegisterCollection(collection, tmIndex.Instance.TexturePlatformCollections);
	}


	public tmTextureCollectionIndex PlatformCollectionIndexForTexturePath(string path)
	{
		string assetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(path);
		foreach(tmTextureCollectionIndex index in tmIndex.Instance.TexturePlatformCollections)
		{
			foreach(string guid in index.textureGUIDs)
			{
				if(guid.Equals(assetGUID))
				{
					return index;
				}
			}
		}

		return null;
	}

	public tmTextureCollectionIndex CollectionIndexForTexturePath(string path)
	{
		string assetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(path);
		foreach(tmTextureCollectionIndex index in tmIndex.Instance.TextureCollections)
		{
			foreach(string guid in index.textureGUIDs)
			{
				if(guid.Equals(assetGUID))
				{
					return index;
				}
			}
		}

		return null;
	}


	static void RegisterCollection(tmTextureCollectionBase collection, List<tmTextureCollectionIndex> collections)
	{
		tmTextureCollectionIndex index = collections.Find(f => f.textureCollectionGUID.Equals(collection.collectionGuid));
		if(index == null)
		{
			index = new tmTextureCollectionIndex();
			collections.Add(index);
		}

		index.name = collection.name;
		index.assetGUID = collection.assetGuid;
		index.textureCollectionGUID = collection.collectionGuid;
		//		index.editorLink = collection;

		collection.textureDefenitions.Sort( (a, b) => (string.Compare(a.textureName, b.textureName, System.StringComparison.OrdinalIgnoreCase)));

		List<string> names = new List<string>();
		List<string> guids = new List<string>();
		foreach(tmTextureDefenition def in collection.textureDefenitions)
		{
			names.Add(def.textureName);
			guids.Add(def.assetGuid);
		}

		index.textureNames = names.ToArray();
		index.textureGUIDs = guids.ToArray();

		collections.Sort( (a, b) => (string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase)));
		UnityEditor.EditorUtility.SetDirty(tmIndex.Instance);
	}

	#endregion
}
