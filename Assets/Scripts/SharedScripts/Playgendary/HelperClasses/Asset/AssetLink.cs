using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class AssetLink : Asset
{
    #region Variables

    const string RESOURCE_FOLDER = "Resources/";
	const string STREAMING_ASSETS_FOLDER = "StreamingAssets/";

	public string name;
	public string assetGUID;

    string fullPath = null;
    string resourcePath = null;
    string streamingAssetPath = null;
    string cachedWWWStreamingAssetPath = null;

    Object loadedAsset;
    WWW    loadedWWW;

    #endregion


    #region Properties

	public string FullPath
	{
		get
		{
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(fullPath))
            #else
            if (fullPath == null)
            #endif
			{
				fullPath = GUIDMapper.GUIDToAssetPath(assetGUID);
			}
			return fullPath;
		}
	}


	public string ResourcePath
	{
		get
		{
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(resourcePath))
            #else
            if (resourcePath == null)
            #endif
			{
				int startIndex = FullPath.IndexOf(RESOURCE_FOLDER, System.StringComparison.Ordinal);
				if(startIndex > -1)
				{
					startIndex += RESOURCE_FOLDER.Length;
                    resourcePath = FullPath.Substring(startIndex);
                    resourcePath = resourcePath.RemovePathExtention();
				}
				else
				{
                    resourcePath = string.Empty;
				}
			}

			return resourcePath;
		}
	}


    public string StreamingAssetPath
	{
		get
		{
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(streamingAssetPath))
            #else
            if (streamingAssetPath == null)
            #endif
			{
				int startIndex = FullPath.IndexOf(STREAMING_ASSETS_FOLDER, System.StringComparison.Ordinal);
				if(startIndex > -1)
				{
                    startIndex += STREAMING_ASSETS_FOLDER.Length;
                    streamingAssetPath = FullPath.Substring(startIndex);
                    streamingAssetPath = Application.streamingAssetsPath.AppendPathComponent(streamingAssetPath);
				}
				else
				{
                    streamingAssetPath = string.Empty;
				}
			}

			return streamingAssetPath;
		}
	}


    public bool IsResource
    {
        get { return ResourcePath.Length > 0; }
    }


    public bool IsStreamingAsset
    {
        get { return StreamingAssetPath.Length > 0; }
    }

       
    public override bool IsLoaded
    {
        get
        {
            return ((loadedAsset != null) || (loadedWWW != null));
        }
    }


    string CachedWWWStreamingAssetPath
    {
        get
        {
            if (cachedWWWStreamingAssetPath == null)
            {
                if (!string.IsNullOrEmpty(StreamingAssetPath))
                {
                    cachedWWWStreamingAssetPath = StreamingAssetPath.AppendWWWFilePrefix();
                    #if UNITY_ANDROID
                    cachedWWWStreamingAssetPath = cachedWWWStreamingAssetPath.AppendWWWJARPrefix();
                    #endif
                }
                else
                {
                    cachedWWWStreamingAssetPath = string.Empty;
                }
            }

            return cachedWWWStreamingAssetPath;
        }
    }
        
    #endregion

	
    #region Unity lifecycles

	public AssetLink(Object asset)
	{
        this.SetAsset(asset);
	}

    [System.Obsolete("For RESOURCES use AssetResource, for STREAMING ASSETS use AssetStreaming")]
    public AssetLink(string path)
    {
        name = System.IO.Path.GetFileNameWithoutExtension(path);
        fullPath = path;
    }

    #endregion


    #region Public methods

    public override void Load()
    {
        #if UNITY_EDITOR
        loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(FullPath, typeof(Object));
        loadedWWW = null;
        #else
        if (!IsLoaded)
        {
            if (IsResource)
            {
                loadedWWW = null;
                loadedAsset = Resources.Load(ResourcePath, typeof(Object));
            }
            else if (IsStreamingAsset)
            {
                loadedAsset = null;
                loadedWWW = new WWW(CachedWWWStreamingAssetPath);
                while (!loadedWWW.isDone) { System.Threading.Thread.Sleep(1); }
                if (!string.IsNullOrEmpty(loadedWWW.error))
                {
                    CustomDebug.Log("AssetLink Error WWW :: " + loadedWWW.error);
                    loadedWWW.Dispose();
                    loadedWWW = null;
                }
            }
        }
        #endif
    }


    public override void LoadAsync(System.Action callback)
    {
        #if UNITY_EDITOR
        loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(FullPath, typeof(Object));
        loadedWWW = null;
        #else
        if (!IsLoaded)
        {
            if (IsResource)
            {
                loadedWWW = null;
                ResourceRequest request = Resources.LoadAsync(ResourcePath, typeof(Object));
                request.StartCoroutine(delegate() 
                    {
                        loadedAsset = request.asset;
                        if (callback != null)
                        {
                            callback();
                        }
                    });
            }
            else if (IsStreamingAsset)
            {
                loadedAsset = null;
                loadedWWW = new WWW(CachedWWWStreamingAssetPath);
                loadedWWW.StartCoroutine(delegate() 
                    {
                        if (!string.IsNullOrEmpty(loadedWWW.error))
                        {
                            CustomDebug.Log("AssetLink Error WWW :: " + loadedWWW.error);
                            loadedWWW.Dispose();
                            loadedWWW = null;
                        }

                        if (callback != null)
                        {
                            callback();
                        }
                    });
            }
        }
        #endif            
    }


    public override void Unload()
    {
        #if UNITY_EDITOR
        loadedAsset = null;
        loadedWWW = null;
        #else
        if (IsLoaded)
        {
            if (IsResource)
            {
                Resources.UnloadAsset(loadedAsset);
                loadedAsset = null;
                loadedWWW = null;
            }
            else if (IsStreamingAsset)
            {
                loadedWWW.Dispose();
                loadedWWW = null;
                loadedAsset = null;
            }
        }
        #endif
    }


    // Use IDisposable interface or Unload for free resources
    public Object GetAsset()
    {
        Object result = null;
        if (IsLoaded)
        {
            result = loadedAsset;
        }
        else
        {
            Load();
            if (IsLoaded)
            {
                result = loadedAsset;
            }
        }
        return result;
    }


    // Use IDisposable interface or Unload for free resources
    public void GetAssetAsync(System.Action<Object> callback)
    {
        Object result = null;
        if (IsLoaded)
        {
            result = loadedAsset;
            if (callback != null)
            {
                callback(result);
            }
        }
        else
        {
            LoadAsync(delegate()
                {
                    if (IsLoaded)
                    {
                        result = loadedAsset;
                    }                
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
        }
    }

    // Use IDisposable interface or Unload for free resources
    public Object GetInstance()
    {
        Object result = null;
        if (IsLoaded)
        {
            result = Object.Instantiate(loadedAsset);
        }
        else
        {
            Load();
            if (IsLoaded)
            {
                result = Object.Instantiate(loadedAsset);
//                    Unload();
            }
        }
        return result;
    }


    // Use IDisposable interface or Unload for free resources
    public void GetInstanceAsync(System.Action<Object> callback)
    {
        Object result = null;
        if (IsLoaded)
        {
            result = Object.Instantiate(loadedAsset);
            if (callback != null)
            {
                callback(result);
            }
        }
        else
        {
            LoadAsync(delegate()
                {
                    if (IsLoaded)
                    {
                        result = Object.Instantiate(loadedAsset);
//                        Unload();
                    }                
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
        }
    }


    // Use Async method
    // Use IDisposable interface or Unload for free resources
    public WWW GetWWW()
    {
        WWW result = null;
        if (IsLoaded)
        {
            result = loadedWWW;
        }
        else
        {
            Load();
            if (IsLoaded)
            {
                result = loadedWWW;
//                Unload();
            }
        }
        return result;
    }


    // Use IDisposable interface or Unload for free resources
    public void GetWWWAsync(System.Action<WWW> callback)
    {
        WWW result = null;
        if (IsLoaded)
        {
            result = loadedWWW;
            if (callback != null)
            {
                callback(result);
            }
        }
        else
        {
            LoadAsync(delegate() 
                {
                    if (IsLoaded)
                    {
                        result = loadedWWW;
//                        Unload();
                    }
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
        }
    }

    #endregion


    #region Private methods

    protected virtual void SetAsset(Object asset)
    {
        #if UNITY_EDITOR
        if (asset != null)
        {
            string newAssetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
            assetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(newAssetPath);
            name = asset.name;
        }
        else
        {
            assetGUID = string.Empty;
            name = string.Empty;
        }

        fullPath = string.Empty;
        resourcePath = string.Empty;
        streamingAssetPath = string.Empty;
        #endif
    }
        
    #endregion
}
