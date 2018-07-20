using UnityEngine;
using System.Collections;

public class TextureProviderStream : TextureProvider 
{
	string loadingKey;


	public TextureProviderStream(string path) : base (path)
	{

	}


	public override Texture2D Load(string path, TextureFormat format, bool mipmaps)
	{
		if(state == TextureState.Loading)
		{
			StopLoading();
		}

        if(state != TextureState.Loaded || !texture)
		{
			texture = TextureHelper.Instance.LoadImageToTexture(path, mipmaps, format);
			state = TextureState.Loaded;
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

			loadingKey = TextureHelper.Instance.LoadImageToTextureAsync(
				path, 
				(tex) =>
				{
					if(state == TextureState.Loading)
					{
						texture = tex;
						state = TextureState.Loaded;
						loadingKey = null;

						if(!callbacks.Call(path, texture))
						{
							UnloadTexture();
						}
					}
				}, 
				mipmaps, 
				format
			);
		}
	}


	public override void UnloadTexture()
	{
		if(state == TextureState.Loaded)
		{
			if(texture != null)
			{
				TextureHelper.Instance.UnloadTexture(texture);
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
		if(state == TextureState.Loading && !string.IsNullOrEmpty(loadingKey))
		{
			TextureHelper.Instance.CancelLoadAsync(loadingKey);
			loadingKey = null;
			callbacks.RemoveAll(url);
		}
	}
}
