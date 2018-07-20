using UnityEngine;
using System.Collections;



public abstract class tk2dBaseMesh : MonoBehaviour 
{
	#region Cached properties

	Mesh cachedMesh;
	Mesh CachedMesh
	{
		get
		{
			if (cachedMesh == null)
			{
				cachedMesh = new Mesh();
				cachedMesh.hideFlags = HideFlags.DontSave;
				CachedMeshFilter.mesh = cachedMesh;
			}
			
			return cachedMesh;
		}
	}

	
	MeshFilter cachedMeshFilter;
	MeshFilter CachedMeshFilter
	{
		get
		{
			if (cachedMeshFilter == null)
			{
				cachedMeshFilter = GetComponent<MeshFilter>();
			}
			
			return cachedMeshFilter;
		}
	}


	protected virtual void OnDestroy()
	{
        if (cachedMesh != null)
		{
            if (cachedMeshFilter != null && cachedMeshFilter.sharedMesh == cachedMesh)
			{
                cachedMeshFilter.mesh = null;
			}

#if UNITY_EDITOR
            DestroyImmediate(cachedMesh, true);
#else
            Destroy(cachedMesh);
#endif
		}
	}


	Transform cachedTransform;
    public Transform CachedTransform
	{
		get
		{
			if (cachedTransform == null)
			{
				cachedTransform = transform;
			}
			
			return transform;
		}
	}


	Renderer cachedRenderer;
    public Renderer CachedRenderer 
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
        
	#endregion


	#region Batch object interface

    // Get tmBatch object to support batching
    // using bool instead of null check because many objects really have null
    bool batchObjectInitialized = false;
    tmBatchObject batchObject;
    tmBatchObject BatchObject
    {
        get
        {
            if (!batchObjectInitialized)
            {
                batchObject = GetComponent<tmBatchObject>();

                if (batchObject != null)
                {
                    batchObject.ForcedBatching = true;
                }

                batchObjectInitialized = true;
            }

            return batchObject;
        }
    }
        

	bool BatchObjectIsActive
	{
		get { return BatchObject != null && BatchObject.BatchingType != tmBatchingType.None; }
		set
		{
			if (BatchObjectIsActive != value && BatchObject != null)
			{
				if (value)
				{
					BatchObject.BatchingType = tmBatchingType.Skinning;
				}
				else
				{
					BatchObject.BatchingType = tmBatchingType.None;
				}
			}
		}
	}


	// returns whether change occurs
	bool UpdateBatchObjectIsActive()
	{
		bool oldValue = BatchObjectIsActive;
		BatchObjectIsActive = (cachedMesh != null && CurrentMaterial != null);
		return oldValue != BatchObjectIsActive;
	}


	void MarkMeshModified()
	{
		bool changed = UpdateBatchObjectIsActive();

		// if activity not changed and currently is active -> mark modified
		if (!changed && BatchObjectIsActive)
		{
			BatchObject.MarkMeshModified();
		}
	}


	void MarkMaterialModified()
	{
		bool changed = UpdateBatchObjectIsActive();

		// if activity not changed and currently is active -> rebuild
		if (!changed && BatchObjectIsActive)
		{
			BatchObject.Rebuild();
		}
	}

	#endregion


	#region Round Shift

    public static bool globalDisableRoundShift = true;

	public bool isRoundShiftDisabled = true;
	Vector3 totalShift = Vector3.zero;
	protected Vector3 TotalShift
	{
		get { return totalShift; }
	}


	protected virtual void Update()
	{
        if (!globalDisableRoundShift && !isRoundShiftDisabled && CachedTransform.hasChanged)
		{
			CachedTransform.hasChanged = false;

			Vector3 curShift = CachedTransform.RoundShift();
			if (curShift != totalShift)
			{
				totalShift = curShift;
				RoundShiftChanged();
			}
		}
	}


	protected abstract void RoundShiftChanged();

	#endregion


	#region Mesh interface

	protected void MeshClear()
	{
		CachedMesh.Clear();
		MarkMeshModified();
	}


	protected Vector3[] MeshVertices
	{
		get { return CachedMesh.vertices; }
		set { CachedMesh.vertices = value; MarkMeshModified(); }
	}


	protected Vector3[] MeshNormals
	{
		get { return CachedMesh.normals; }
		set { CachedMesh.normals = value; MarkMeshModified(); }
	}


	protected Vector4[] MeshTangents
	{
		get { return CachedMesh.tangents; }
		set { CachedMesh.tangents = value; MarkMeshModified(); }
	}


	protected Color[] MeshColors
	{
		get { return CachedMesh.colors; }
		set { CachedMesh.colors = value; MarkMeshModified(); }
	}


	protected Color32[] MeshColors32
	{
		get { return CachedMesh.colors32; }
		set { CachedMesh.colors32 = value; MarkMeshModified(); }
	}


	protected Vector2[] MeshUV
	{
		get { return CachedMesh.uv; }
		set { CachedMesh.uv = value; MarkMeshModified(); }
	}


	protected Vector2[] MeshUV2
	{
		get { return CachedMesh.uv2; }
		set { CachedMesh.uv2 = value; MarkMeshModified(); }
	}


	protected int[] MeshTriangles
	{
		get { return CachedMesh.triangles; }
		set { CachedMesh.triangles = value; MarkMeshModified(); }
	}


	protected void MeshRecalculateBounds()
	{
		CachedMesh.RecalculateBounds();
	}


	protected Bounds MeshBounds
	{
		get { return CachedMesh.bounds; }
		set { CachedMesh.bounds = value; MarkMeshModified(); }
	}

	#endregion



	#region Material interface

	protected Material CurrentMaterial
	{
		get { return CachedRenderer.sharedMaterial; }
		set
		{
			if (CurrentMaterial != value)
			{
				CachedRenderer.material = value;
				MarkMaterialModified();
			}
		}
	}

	#endregion
}


