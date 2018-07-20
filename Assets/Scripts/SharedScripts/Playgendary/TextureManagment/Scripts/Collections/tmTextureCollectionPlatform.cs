using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class tmTextureCollectionPlatform : tmTextureCollectionBase
{
	#region Variables
	public tmPlatform platform;
	[SerializeField] string atlasAssetGUID;
    [SerializeField] string assetPath;
	protected Texture2D atlas;
	protected Coroutine unloading;
	#endregion


	public string AtlasAssetGUID 
	{
		get { return atlasAssetGUID; }
		set { atlasAssetGUID = value; }
	}


    public string AssetPath
    {
        get
        {
			return assetPath;
        }
        set
        {
			assetPath = value;
        }
    }


	public Texture2D Atlas 
	{
		get 
		{
			return atlas;
		}
        set
        {
            atlas = value;
        }
	}


	#region Retain handle

	int retainCount;
	HashSet<tmTextureRenderBase> renderRefs = new HashSet<tmTextureRenderBase>();


	int RetainCount 
	{
		get { return retainCount; }
		set 
		{
			if(retainCount == 0 && value > 0)
			{
				LoadTexture();
			}

			if(retainCount > 0 && value == 0)
			{
				UnloadTexture();
			}

			retainCount = value;
		}
	}


	void Add(tmTextureRenderBase render)
	{
		if(renderRefs.Add(render))
		{
			RetainCount++;
		}
	}


	void Remove(tmTextureRenderBase render)
	{
		if(renderRefs.Remove(render))
		{
			RetainCount--;
		}
	}


	public static tmTextureCollectionPlatform operator + (tmTextureCollectionPlatform platform, tmTextureRenderBase render) 
	{
		if(platform != null)
		{
			platform.Add(render);
		}
		return platform;
	}


	public static tmTextureCollectionPlatform operator - (tmTextureCollectionPlatform platform, tmTextureRenderBase render) 
	{
		if(platform != null)
		{
			platform.Remove(render);
		}
		return platform;
	}

	#endregion


	#region Texture Managment
	//TODO repplace from cache
	public static int CollectionWaitingForLoadCount {get; private set;}


	public void LoadTexture()
	{
		unloading.StopCoroutine();
		unloading = null;

		if(atlas == null)
		{
			string path = useStreamingAssets ?
                Application.streamingAssetsPath + "/" + tmManager.stramingAssetsDirectory + "/" + assetPath :
				assetPath;
			
            if(
                #if UNITY_EDITOR
                !Application.isPlaying ||
                #endif
                tmSettings.Instance.isImmediateTextureLoadEnabled)
			{
				atlas = TextureCache.LoadTexture(path, useStreamingAssets ? TextureDataSource.StreamingAssets : TextureDataSource.Resources, importSettings.format, importSettings.generateMipMaps);
				if(tmManager.InstanceIfExist)
				{
					tmManager.Instance.TextureLoadedForCollection(this);
				}
			}
			else
			{
				TextureCache.LoadTextureAsync(path, useStreamingAssets ? TextureDataSource.StreamingAssets : TextureDataSource.Resources, importSettings.format, importSettings.generateMipMaps, TextureDidLoaded);
			}
		}
	}


	void TextureDidLoaded(Texture2D tex)
	{
		atlas = tex;
		if (tmManager.InstanceIfExist)
		{
			tmManager.Instance.TextureLoadedForCollection(this);
		}
	}


	void UnloadTexture()
	{
        if(tmSettings.Instance.isAtlasesUnloadEnabled && atlas != null)
		{
            unloading = UnloadTextureIterator().StartCoroutine();
		}
	}


	IEnumerator UnloadTextureIterator()
	{
		yield return new WaitForEndOfFrame();

		if(atlas != null && RetainCount == 0)
		{
			{
				if(tmManager.InstanceIfExist)
				{
					tmManager.Instance.TextureWillUnloadForCollection(this);
				}

				atlas = null;
				TextureCache.UnloadTexture(assetPath);
				unloading = null;
			}
		}
	}

	#endregion
}