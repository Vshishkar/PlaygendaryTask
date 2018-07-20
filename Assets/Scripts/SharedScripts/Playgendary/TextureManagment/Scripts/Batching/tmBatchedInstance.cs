using UnityEngine;
using System.Collections.Generic;


[ExecuteInEditMode]
public class tmBatchedInstance : MonoBehaviour
{
	public event System.Action OnDestroyEvent;
    public static event System.Action<tmBatchedInstance> OnActivated;
    public static event System.Action<tmBatchedInstance> OnDisabled;

	#region Variables

	[SerializeField] List<tmBatchObject> parts = new List<tmBatchObject>();
	List<tmBatchObject> batchedParts = new List<tmBatchObject>();
	public bool shouldMove;
	bool modified;
	bool orderDirty;

	[SerializeField] protected int vertexCount;
	Mesh mesh;

	Transform cachedTransform;
	Renderer cachedRenderer;
	SkinnedMeshRenderer cachedSkinnedRenderer;

	HashSet<tmBatchObject> forcedParts = new HashSet<tmBatchObject>();

	#endregion



	#region Temp Variables

	CombineInstance[] tempCombos;

	#endregion



	#region Properties

	public Renderer CachedRender
	{
		get
		{
			if (cachedRenderer == null)
			{
				cachedRenderer = GetComponent<Renderer>();
			}

			return cachedRenderer;
		}
	}


	public SkinnedMeshRenderer CachedSkinnedRender
	{
		get
		{
			if (cachedSkinnedRenderer == null)
			{
				cachedSkinnedRenderer = GetComponent<SkinnedMeshRenderer>();
			}

			return cachedSkinnedRenderer;
		}
	}


	public Material SharedMaterial
	{
		get 
		{
			return CachedRender.sharedMaterial;
		}
		set 
		{
			CachedRender.sharedMaterial = value;
		}
	}


	public bool Modified
	{
		get 
		{
			return modified;
		}
		set 
		{
			if (!modified && value)
			{
				if(tmBatchingManager.InstanceIfExist)
				{
					tmBatchingManager.Instance.RegisterModifiedBatchedInstance(this);
				}
			}
			modified = value;
		}
	}


	public bool OrderDirty
	{
		get { return orderDirty; }
		set
		{
			if (!orderDirty && value)
			{
				Modified = true;
			}

			orderDirty = value;
		}
	}


	public Mesh Mesh 
	{
		get 
		{
			if(mesh == null)
			{
				mesh = new Mesh();
				mesh.name = "batch_" + gameObject.name;

				MeshFilter mf = GetComponent<MeshFilter>();
				if(mf != null)
				{
					mf.sharedMesh = mesh;
				}

				SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
				if(smr != null)
				{
					smr.sharedMesh = mesh;
				}
			}

			return mesh;
		}
		set
		{
			mesh = value;

			MeshFilter mf = GetComponent<MeshFilter>();
			if(mf != null)
			{
				mf.sharedMesh = mesh;
			}

			SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
			if(smr != null)
			{
				smr.sharedMesh = mesh;
			}
		}
	}


	public Transform CachedTransform
	{
		get
		{
			if (cachedTransform == null)
			{
				cachedTransform = transform;
			}

			return cachedTransform;
		}
	}


	public int VertexCount
	{
		get { return vertexCount; }
	}


	public bool ForcedBatching
	{
		get { return forcedParts.Count > 0; }
	}


	protected List<tmBatchObject> BatchedParts
	{
		get { return batchedParts; }
	}
	
	#endregion



	#region Public

	public bool CanAdd(tmBatchObject part)
	{
		return (vertexCount + part.Mesh.vertexCount) < tmBatchingManager.VERTEX_LIMIT;
	}


	public void Add(tmBatchObject part)
	{
		if(part.Mesh != null && !parts.Contains(part))
		{
			part.BatchInstance = this;
			parts.Add(part);

			Modified = true;
			OrderDirty = true;

			vertexCount += part.Mesh.vertexCount;

			if (part.ForcedBatching)
			{
				AddToForced(part);
			}
		}
	}


	public bool TryAdd(tmBatchObject part)
	{
		if(CanAdd(part))
		{
			Add(part);
			return true;
		}

		return false;
	}


	public void Remove(tmBatchObject part)
	{
		if(parts.Contains(part))
		{
			part.Batched = false;
			part.BatchInstance = null;
			parts.Remove(part);

			Modified = true;

			vertexCount -= part.Mesh.vertexCount;


			if (part.ForcedBatching)
			{
				RemoveFromForced(part);
			}
		}
	}


	public void ForcedBatchChanged(tmBatchObject part)
	{
		if (part.ForcedBatching)
		{
			AddToForced(part);
		}
		else
		{
			RemoveFromForced(part);
		}
	}


	protected virtual void PostRecombine(CombineInstance[] combos)
	{

	}


	protected virtual void ClearMesh()
	{

	}


	public void Recombine()
	{
		// update order
		if (OrderDirty && parts.Count > 0)
		{
			parts.Sort((a,b) => b.CachedTransform.position.z.CompareTo(a.CachedTransform.position.z));
		}

		OrderDirty = false;


		// update mesh
		// recalc non empty meshes
		batchedParts.Clear();
		foreach (var curObject in parts)
		{
			if (curObject.Mesh != null && curObject.Mesh.vertexCount > 0)
			{
				batchedParts.Add(curObject);
			}
		}


		if (batchedParts.Count > 0)
		{
			CachedTransform.position = Vector3.zero;

			tempCombos = ArrayExtention.EnsureLength(tempCombos, batchedParts.Count);

			for (int index = 0, partsCount = batchedParts.Count; index < partsCount; index++)
			{
				tmBatchObject part = batchedParts[index];
				part.Batched = true;

				#if UNITY_EDITOR
				if (part.Mesh != null && !part.Mesh.isReadable)  //this work only for batches without tmTextureRender
				{
                    CustomDebug.Log(part.Mesh);
					string path = UnityEditor.AssetDatabase.GetAssetPath(part.Mesh);
					UnityEditor.ModelImporter mImporter = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.ModelImporter;
					if(mImporter != null)
					{
						mImporter.isReadable = true;
						UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
					}
				}
				#endif


				tempCombos[index].mesh = part.Mesh;
				tempCombos[index].subMeshIndex = 0;
				tempCombos[index].transform = CachedTransform.worldToLocalMatrix * part.CachedTransform.localToWorldMatrix;
			}

			Mesh.CombineMeshes(tempCombos, true);

			PostRecombine(tempCombos);
		}
		else
		{
			Mesh.Clear();

			ClearMesh();
		}

		Modified = false;
	}


	public void Clear()
	{
		foreach(tmBatchObject bp in parts)
		{
			if(bp)
			{
				bp.Batched = false;
				bp.BatchInstance = null;
			}
		}
		parts.Clear();

		if(mesh)
		{
       		mesh.Clear();
		}

		vertexCount = 0;
	}
	#endregion


	#region Unity


	protected virtual void Awake()
	{
		gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshFilter>();
	}


	void OnEnable()
	{
        if (OnActivated != null)
        {
            OnActivated(this);
        }
	}


	void OnDisable()
	{
        if (OnDisabled != null)
        {
            OnDisabled(this);
        }
	}


	void OnDestroy()
	{
		if(Application.isPlaying)
		{
			Clear();
			Object.Destroy(mesh);
		}

		if(OnDestroyEvent != null)
		{
			OnDestroyEvent();
		}
	}

	#endregion


	#region Private


	public void UpdatePosition(float delta)
	{
		if(shouldMove)
		{
            CachedTransform.position -= Vector3.forward * delta;
		}
	}


	void AddToForced(tmBatchObject part)
	{
		forcedParts.Add(part);
	}
	
	
	void RemoveFromForced(tmBatchObject part)
	{
		forcedParts.Remove(part);
	}

	#endregion
}
