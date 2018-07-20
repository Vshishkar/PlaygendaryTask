using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AssetResource : Asset
{
    #region Variables

    const string RESOURCE_FOLDER = "Resources/";

	public string name;

    string fullPath = null;
    string resourcePath = null;

    Object loadedAsset;

    #endregion


    #region Properties

	public string FullPath
	{
        get { return string.IsNullOrEmpty(fullPath) ? (string.Empty) : (fullPath); }
	}


	public string ResourcePath
	{
		get
		{
            if (resourcePath == null)
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
                    resourcePath = FullPath;
				}
			}

			return resourcePath;
		}
	}


    public override bool IsLoaded
    {
        get { return (loadedAsset != null); }
    }


    #endregion

	
    #region Unity lifecycles

    // Only work for RESOURCES
    public AssetResource(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            name = System.IO.Path.GetFileNameWithoutExtension(path);
            fullPath = path;
        }
    }

    #endregion


    #region Public methods

    public override void Load()
    {
        #if UNITY_EDITOR
        loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(FullPath, typeof(Object));
        #else
        if (!IsLoaded)
        {
            loadedAsset = Resources.Load(ResourcePath, typeof(Object));
        }
        #endif
    }


    public override void LoadAsync(System.Action callback)
    {
        #if UNITY_EDITOR
        loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(FullPath, typeof(Object));
        #else
        if (!IsLoaded)
        {
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
        #endif            
    }


    public override void Unload()
    {
        #if UNITY_EDITOR
        loadedAsset = null;
        #else
        if (IsLoaded)
        {
            Resources.UnloadAsset(loadedAsset);
            loadedAsset = null;
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
