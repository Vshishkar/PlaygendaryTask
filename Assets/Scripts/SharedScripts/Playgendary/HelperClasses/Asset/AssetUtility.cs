using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;

public static class AssetUtility {

    static List<Object> assets = new List<Object>();

    public static void PreloadAssets(string subPath, System.Type type)
    {
        string[] paths = AssetDatabase.GetAllAssetPaths();

        foreach(string path in paths)
        {
            if(path.Contains(subPath))
            {
                Object asset = AssetDatabase.LoadAssetAtPath(path, type);
                if(!assets.Contains(asset))
                {
                    assets.Add(asset);
                }
            }
        }
    }


    public static List<T> GetAssetsAtPath<T>(string subPath) where T : Object
    {
        List<T> loadedAssets = new List<T>();
        string[] paths = AssetDatabase.GetAllAssetPaths();

        foreach(string path in paths)
        {
            if(path.Contains(subPath))
            {
                T asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if(asset != null)
                {
                    loadedAssets.Add(asset);
                }
            }
        }

        return loadedAssets;
    }


	public static List<string> GetAssetPaths(string subPath)
	{
		List<string> filteredPaths = new List<string>();
		string[] paths = AssetDatabase.GetAllAssetPaths();

		foreach(string path in paths)
		{
			if(path.Contains(subPath))
			{
				filteredPaths.Add(path);
			}
		}

		return filteredPaths;
	}


	public static void Reimport(string filter, ImportAssetOptions option = ImportAssetOptions.Default)
	{
		List<string> filteredPaths = GetAssetPaths(filter);
		foreach(string path in filteredPaths)
		{
			AssetDatabase.ImportAsset(path, option);
		}
	}


	public static int AssetHashCode(Object asset)
	{
		return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)).GetHashCode();
	}
}
#endif