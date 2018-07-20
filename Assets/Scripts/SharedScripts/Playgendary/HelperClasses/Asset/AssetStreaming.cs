using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AssetStreaming : Asset
{
    #region Variables

	const string STREAMING_ASSETS_FOLDER = "StreamingAssets/";

	public string name;

    string fullPath = null;
    string streamingAssetPath = null;
    string cachedWWWStreamingAssetPath = null;

    WWW    loadedWWW;

    #endregion


    #region Properties

	public string FullPath
	{
        get { return string.IsNullOrEmpty(fullPath) ? (string.Empty) : (fullPath); }
	}


    public string StreamingAssetPath
	{
		get
		{
            if (streamingAssetPath == null)
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
                    streamingAssetPath = FullPath;
				}
			}

			return streamingAssetPath;
		}
	}


    public override bool IsLoaded
    {
        get { return (loadedWWW != null); }
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
                    cachedWWWStreamingAssetPath = StreamingAssetPath;
                }
            }

            return cachedWWWStreamingAssetPath;
        }
    }
        
    #endregion

	
    #region Unity lifecycles

    // Only work for STREAMING ASSETS
    public AssetStreaming(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {            
            name = System.IO.Path.GetFileNameWithoutExtension(path);
            fullPath = path;
        }
    }

    #endregion


    #region Public methods

    // Use Async method
    public override void Load()
    {
        if (!IsLoaded)
        {
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


    public override void LoadAsync(System.Action callback)
    {
        if (!IsLoaded)
        {
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


    public override void Unload()
    {
        if (IsLoaded)
        {
            loadedWWW.Dispose();
            loadedWWW = null;
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
                    }
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
        }
    }

    #endregion
}
