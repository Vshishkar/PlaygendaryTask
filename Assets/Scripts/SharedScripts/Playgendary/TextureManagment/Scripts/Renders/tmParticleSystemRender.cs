using UnityEngine;
using System.Collections;


public class tmParticleSystemRender : tmTextureRenderBase
{
	#region Variables
	[SerializeField] bool useRenderQueue;
	[SerializeField] int renderQueue;


	public bool UseRenderQueue 
	{
		get 
		{
			return useRenderQueue;
		}
		#if UNITY_EDITOR
		set 
		{
			bool prevValue = useRenderQueue;
			useRenderQueue = value;
			if(prevValue ^ useRenderQueue)
			{
				ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
				UpdateMaterial();
			}
		}
		#endif
	}


	public int RenderQueue 
	{
		get 
		{
			return renderQueue;
		}
		#if UNITY_EDITOR
		set 
		{
			int prevValue = renderQueue;
			renderQueue = value;
			if(renderQueue != prevValue)
			{
				ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
				UpdateMaterial();
			}
		}
		#endif
	}
	#endregion



	#region Unity

	protected override void Awake()
	{
		MainTexCollection -= this;
		MainTexCollection += this;

		base.Awake();
	}

//
//	protected override void OnEnable()
//	{
//		base.OnEnable();
//
//		#if UNITY_EDITOR
//		if(Application.isPlaying)
//		#endif
//		{
//			MainTexCollection += this;
//			LightmapCollection += this;
//		}
//	}
//
//
//	protected override void OnDisable()
//	{
//		#if UNITY_EDITOR
//		if(Application.isPlaying)
//		#endif
//		{
//			MainTexCollection -= this;
//			LightmapCollection -= this;
//		}
//
//		base.OnDisable();
//	}

	#endregion



	#region Private

	protected override Material MaterialInstance(Material original, tmTextureCollectionPlatform mainCollection, tmTextureCollectionPlatform lightmapCollection)
	{		
		string hashKey = "mat" + original.GetHashCode();
		string materialUniqueName = "";

		if(mainCollection != null)
		{
			hashKey += mainCollection.collectionGuid;
			materialUniqueName += mainCollection.name;
		}

		if(lightmapCollection != null)
		{
			hashKey += lightmapCollection.collectionGuid;
			materialUniqueName += lightmapCollection.name;
		}

		hashKey += UseRenderQueue ? RenderQueue : Material.renderQueue;

		Vector2 offset = Vector2.zero;
		Vector2 scale = Vector2.zero;

		if(MainTextureDefenition != null)
		{
			Rect uvRect = MainTextureDefenition.uv;
			Rect uvOffset = MainTextureDefenition.offset;
			uvRect.center += uvOffset.center;
			uvRect.size += uvOffset.size;

			offset = new Vector2(
				uvRect.x,
				uvRect.y
			);

			scale = new Vector2(
				uvRect.width,
				uvRect.height
			);
		}

		hashKey += "" + offset.x + offset.y + scale.x + scale.y;

		Material copy;
		if(tmManager.Instance.GetSharedMaterial(original, mainCollection, lightmapCollection, hashKey, out copy))
		{
			copy.name = original.name + "_" + materialUniqueName;
			copy.hideFlags = HideFlags.HideAndDontSave;

			copy.mainTextureOffset = offset;
			copy.mainTextureScale = scale;

			if(UseRenderQueue)
			{
				copy.renderQueue = RenderQueue;
			}
		}

		return copy;
	}

	#endregion
}
