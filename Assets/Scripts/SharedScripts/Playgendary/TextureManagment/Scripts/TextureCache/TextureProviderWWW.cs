using UnityEngine;
using System;
using System.Collections;


public class TextureProviderWWW : TextureProvider
{
	Coroutine loading = null;


	public TextureProviderWWW(string path) : base (path)
	{

	}


	public override Texture2D Load(string path, TextureFormat format, bool mipmaps)
	{
		if(state != TextureState.Loaded)
		{
			CustomDebug.LogError("TextureProviderWWW.Load : cant load instatly from WWW");
		}

		return texture;
	}


	public override void LoadAsync(string path, TextureFormat format, bool mipmaps, System.Action<Texture2D> callback)
	{
		if(state == TextureState.Loaded)
		{
			callback(texture);
		}
		else if(state == TextureState.NotLoaded)
		{
			state = TextureState.Loading;
			callbacks.AddCallback(path, callback);
			loading = Loading(path).StartCoroutine();
		}
	}


	IEnumerator Loading(string path) 
	{
		WWW loader = new WWW(path);

		yield return loader;

		state = TextureState.NotLoaded;

		if(loader.isDone)
		{
			if(string.IsNullOrEmpty(loader.error))
			{
				texture = loader.textureNonReadable;
				state = TextureState.Loaded;
			}
			else
			{
				CustomDebug.LogError("Can't download texture by url : " + path + "\n" + loader.error);
			}
		}

        loader.Dispose();
		loading = null;

		if(!callbacks.Call(path, texture))
		{
			UnloadTexture();
		}
	}


	public override void UnloadTexture()
	{
		if(state == TextureState.Loaded)
		{
			if(texture != null)
			{
				UnityEngine.Object.Destroy(texture);
				texture = null;
			}
		}

		if(state == TextureState.Loading)
		{
			StopLoading();
		}

		state = TextureState.NotLoaded;
	}


	void StopLoading()
	{
		if(state == TextureState.Loading && loading != null)
		{
			loading.StopCoroutine();
			loading = null;
			callbacks.RemoveAll(url);
		}
	}
}