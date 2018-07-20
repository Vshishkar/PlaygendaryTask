using UnityEngine;
using System.Collections;


public class tmBatchObject : MonoBehaviour 
{
	#region Variables

	[HideInInspector][SerializeField] tmBatchingType batchingType;
	[SerializeField] Transform root;
	tmBatchedInstance batchInstance;
	bool batched;
	bool forcedBatching;

	Transform cachedTransform;
	MeshFilter cachedMeshFilter;
	Renderer cachedRenderer;
	SkinnedMeshRenderer cachedSkinnedRenderer;
    bool skinnedMeshRender = false;
	#endregion



	#region Gettets && Setters

	public bool Batched 
	{
		get
		{
			return batched;
		}
		set 
		{
			if(batched ^ value)
			{
				batched = value;
				CachedRenderer.enabled = !batched;
			}
		}
	}


	public Transform CachedTransform 
	{
		get 
		{
			if(cachedTransform == null)
			{
				cachedTransform = transform;
			}
			return cachedTransform;
		}
	}


	public Renderer CachedRenderer 
	{
		get 
		{
			if(cachedRenderer == null)
			{
				cachedRenderer = GetComponent<Renderer>();
			}
			return cachedRenderer;
		}
	}


	public MeshFilter MeshFilter
	{
		get 
		{
			if(cachedMeshFilter == null)
			{
				cachedMeshFilter = GetComponent<MeshFilter>();
			}
			return cachedMeshFilter;
		}
	}


	public SkinnedMeshRenderer SkinnedMeshRender
	{
		get 
		{
            if(!skinnedMeshRender)
			{
                skinnedMeshRender = true;
				cachedSkinnedRenderer = GetComponent<SkinnedMeshRenderer>();
			}
			return cachedSkinnedRenderer;
		}
	}


	public Mesh Mesh 
	{
		get 
		{ 
			if(MeshFilter != null)
			{
				return MeshFilter.sharedMesh;
			}

			if(SkinnedMeshRender != null)
			{
				return SkinnedMeshRender.sharedMesh;
			}

			return null; 
		}
	}


	public Material Material 
	{
		get 
		{ 
			return CachedRenderer.sharedMaterial;
		}
	}


	public tmBatchedInstance BatchInstance 
	{
		get { return batchInstance; }
		set { batchInstance = value; }
	}


	public tmBatchingType BatchingType 
	{
		get 
		{ 
			return batchingType; 
		}
		set 
		{
			if (batchingType != value)
			{
				Unbatch();
				batchingType = value;
				Batch();
			}
		}
	}


	public Transform Root 
	{
		set { root = value; }
	}


	tmTextureRenderBase cachedTextureRender;
	bool hasTextureRender;

	public tmTextureRenderBase CachedTextureRender 
	{
		get 
		{ 
			if(!hasTextureRender && cachedTextureRender == null)
			{
				cachedTextureRender = GetComponent<tmTextureRenderBase>();
				hasTextureRender = true;
			}

			return cachedTextureRender; 
		}
	}


	// setting this to true makes batching manager to not use vertex limit to this object
	// WARNING: forcing abtching on an object
	// makes the whole instance to force batching
	// so use it carefully
	public bool ForcedBatching
	{
		get { return forcedBatching; }
		set
		{
			forcedBatching = value;

			if (ForcedBatching != value && BatchInstance != null)
			{
				BatchInstance.ForcedBatchChanged(this);
			}
		}
	}

	#endregion



	#region Unity

	public void SetUp()
	{
//		Root = gameObject.FindComponentInParent<Chunk>().transform;
		if(GetComponent<SkinnedMeshRenderer>() != null)
		{
			BatchingType = tmBatchingType.None;
			return;
		}

		if(Mesh != null)
		{
			if(Mesh.vertexCount < tmBatchingManager.DYNAMIC_BATCHING_LIMIT)
			{
				BatchingType = tmBatchingType.Dynamic;
				Root = null;
			}
			else
			{
				BatchingType = tmBatchingType.Static;				
			}
		}
	}


	protected virtual void OnEnable()
	{
		if( (BatchingType == tmBatchingType.Dynamic || BatchingType == tmBatchingType.Skinning) && CachedTextureRender == null)
		{
            if (batchInstance != null)
            {
                Unbatch();    
            }

			Batch();
		}
	}


	protected virtual void OnDisable()
	{		
		Unbatch();		
	}

	#endregion



	#region Public

	public void Rebuild()
	{
		if(BatchingType == tmBatchingType.Dynamic || BatchingType == tmBatchingType.Skinning)
		{
			Unbatch();
			Batch();
		}
	}


	public void Batch()
	{
		// inactive batch objects can have their type changed, but should not be affected untill OnEnable
		if (isActiveAndEnabled)
		{
			if (gameObject.activeInHierarchy && BatchingType != tmBatchingType.None)
			{
				#if UNITY_EDITOR
				if(Application.isPlaying)
				#endif
				{
					if (tmSettings.Instance.batching)
					{
						tmBatchingManager.Instance.BatchObject(this, root, false);
					}
				}
			}
		}
	}


	public void Unbatch()
	{
		if(BatchingType == tmBatchingType.Dynamic || BatchingType == tmBatchingType.Skinning)
		{
			#if UNITY_EDITOR
			if(Application.isPlaying)
			#endif
			{
				if(BatchInstance != null)
				{
					BatchInstance.Remove(this);
				}
			}
		}
	}


	public void MarkMeshModified()
	{
		if (BatchInstance != null)
		{
			BatchInstance.Modified = true;
		}
	}
	#endregion

}
