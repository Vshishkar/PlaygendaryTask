using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class tmAtlasImportSettings
{
//	[EnumAttribute(16, 32 ,64, 128, 256, 512, 1024, 2048, 4096)] public int maxAtlasSize = 4096;
	[SerializeField] public int border;
	[SerializeField] public bool generateMipMaps;
	[SerializeField] public TextureFormat format = TextureFormat.RGBA32;

	#if UNITY_EDITOR
	[SerializeField] public TextureImporterFormat importFormat = TextureImporterFormat.RGBA32;
	#endif
}


[System.Serializable]
public class tmTextureDefenition
{
	public string textureName;

	#if UNITY_EDITOR
	public Texture2D texture;
	public Texture2D platformTexture;
	#endif

	public string textureGuid;
	public string assetGuid;
	public ulong originalTimestamp;
	public ulong platformTimestamp;
	public Rect uv;
	public Rect offset;


	public tmTextureDefenition Copy()
	{
		tmTextureDefenition tr = new tmTextureDefenition();
		#if UNITY_EDITOR
		tr.texture = texture;
		tr.platformTexture = platformTexture;
		#endif
		tr.textureName = textureName;
		tr.textureGuid = textureGuid;
		tr.assetGuid = assetGuid;
		tr.originalTimestamp = originalTimestamp;
		tr.platformTimestamp = platformTimestamp;
		tr.offset = offset;

		return tr;
	}
}


public class tmTextureCollectionBase : MonoBehaviour
{
	#if UNITY_EDITOR
	[SerializeField] List<Texture2D> textures = new List<Texture2D>();
	#endif

	[SerializeField] public List<tmTextureDefenition> textureDefenitions = new List<tmTextureDefenition>();
	[SerializeField] public string assetGuid;
	[SerializeField] public string collectionGuid;
	[SerializeField] public tmAtlasImportSettings importSettings;
    [SerializeField] public float forcedScale = -1;
    [SerializeField] public bool useStreamingAssets;

	#if UNITY_EDITOR
	public List<Texture2D> Textures 
	{
		get {
			return textures;
		}
	}
	#endif


	public tmTextureDefenition GetTextureDefenitionByID(string textureID)
	{
		return textureDefenitions.Find(f => f.textureGuid.Equals(textureID));
	}


	public tmTextureDefenition GetTextureDefenitionByName(string textureName)
	{
		return textureDefenitions.Find(f => f.textureName.Equals(textureName));
	}
}
