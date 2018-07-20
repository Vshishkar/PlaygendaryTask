using UnityEngine;


public class TextureProviderResources : TextureProvider
{
	Coroutine loading = null;


	public TextureProviderResources(string path) : base (path)
	{
		
	}


	public override Texture2D Load(string path, TextureFormat format, bool mipmaps)
	{
		if(state == TextureState.Loading)
		{
			StopLoading();
		}

		if(state != TextureState.Loaded)
		{
			texture = Resources.Load(path) as Texture2D;
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

			ResourceRequest req = Resources.LoadAsync(path, typeof(Texture2D));
			loading = req.StartCoroutine(
				() =>
				{
					if(state == TextureState.Loading)
					{
						loading = null;
						texture = req.asset as Texture2D;
						state = TextureState.Loaded;

						if(!callbacks.Call(path, texture))
						{
							UnloadTexture();
						}
					}
				}
			);
		}
	}


	public override void UnloadTexture()
	{
		if(state == TextureState.Loaded)
		{
			if(texture != null)
			{
				Resources.UnloadAsset(texture);
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
