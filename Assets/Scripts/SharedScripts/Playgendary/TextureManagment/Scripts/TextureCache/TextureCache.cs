using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public enum TextureDataSource
{
	WWW = 1,
	Resources = 2,
	StreamingAssets = 3,
}


public static class TextureCache
{
	#region variables
	static Dictionary<string, TextureProvider> providers = new Dictionary<string, TextureProvider>();


	public static bool HasIncompleteDownloads
	{
		get
		{
			foreach (var provider in providers.Values) 
			{
				if(provider.InProgress)
				{
					return true;
				}
			}

			return false;
		}
	}
	#endregion



	#region Public methods

	public static string PlatformPath(string path)
	{
		path = path.Replace("2x", tk2dSystem.CurrentPlatform);
		path = path.Replace("4x", tk2dSystem.CurrentPlatform);
		return path;
	}


	public static Texture2D LoadTexture(string path, TextureDataSource source, TextureFormat format, bool mipmaps)
	{
		TextureProvider provider = GetProviderForPath(path, source);
		return provider.Load(path, format, mipmaps);
	}


	public static void LoadTextureAsync(string path, TextureDataSource source, TextureFormat format, bool mipmaps, System.Action<Texture2D> action)
	{
		TextureProvider provider = GetProviderForPath(path, source);
		provider.LoadAsync(path, format, mipmaps, action);
	}


	public static void UnloadTexture(string path)
	{
        TextureProvider provider;
		if(providers.TryGetValue(path, out provider))
		{
			provider.UnloadTexture();
		}
	}


    public static void UnloadTexture(Texture2D texture)
    {
        if (texture == null)
        {
            return;
        }

        foreach (TextureProvider provider in providers.Values)
        {
            if (provider.Texture == texture)
            {
                provider.UnloadTexture();
            }
        }
    }

	#endregion



	#region Private methods

	static TextureProvider GetProviderForPath(string path, TextureDataSource source)
	{	
		TextureProvider provider;
		if(!providers.TryGetValue(path, out provider))
		{
			switch(source)
			{
				case TextureDataSource.WWW:
					provider = new TextureProviderWWW(path);
					break;
				case TextureDataSource.Resources:
					provider = new TextureProviderResources(path);
					break;
				case TextureDataSource.StreamingAssets:
					provider = new TextureProviderStream(path);
					break;
			}

			providers.Add(path, provider);
		}

		return provider;
	}

	#endregion
}
