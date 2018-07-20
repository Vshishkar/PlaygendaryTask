using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class tmTextureRenderBase : MonoBehaviour, tmIRebuildable
{
	[System.Flags]
	public enum ModifiedFlags
	{
		ModifiedMesh 		= 	1 << 0,
		ModifiedMaterial	= 	1 << 1,
		ModifiedUV1			= 	1 << 2,
		ModifiedUV2			= 	1 << 3
	}


	#region Variables

	[SerializeField] Mesh mesh;
	[SerializeField] Material material;

	[SerializeField] string mainTexCollectionGUID;
	[SerializeField] string mainTextureID;
	[SerializeField] string lightmapCollectionGUID;
	[SerializeField] string lightmapTextureID;

	tmTextureCollectionPlatform mainTexCollection;
	tmTextureCollectionPlatform lightmapCollection;

	protected Mesh sharedMesh;
	protected Material sharedMaterial;
	protected ModifiedFlags modifiedFlag;

    MeshFilter cachedMeshFilter;
    SkinnedMeshRenderer cachedSkinnedMeshRenderer;
	Renderer cachedRenderer;

	#endregion



	#region shared properties
	public Mesh SharedMesh 
	{
		get 
		{ 
			if(sharedMesh == null)
			{
				modifiedFlag |= ModifiedFlags.ModifiedMesh;
				BuildMesh();
			}
			return sharedMesh; 
		}
	}


	public Material SharedMaterial 
	{
		get
		{ 
			if(sharedMaterial == null)
			{
				modifiedFlag |= ModifiedFlags.ModifiedMaterial;
				UpdateMaterial();
			}
			return sharedMaterial; 
		}
	}


	public Mesh RenderSharedMesh 
	{
		get 
		{ 
			if(cachedMeshFilter != null)
			{
				return cachedMeshFilter.sharedMesh;
			}

			if(cachedSkinnedMeshRenderer != null)
			{
				return cachedSkinnedMeshRenderer.sharedMesh;
			}

			return null; 
		}
		set
		{
			if(cachedMeshFilter != null)
			{
				cachedMeshFilter.sharedMesh = value;
			}
			else if(cachedSkinnedMeshRenderer != null)
			{
				cachedSkinnedMeshRenderer.sharedMesh = value;
			}
		}
	}


	public Renderer CachedRenderer 
	{
		get 
		{ 
			if(cachedRenderer == null)
			{
				cachedRenderer = GetComponent<Renderer>();

				if(cachedRenderer == null)
				{
					if(sharedMesh != null && sharedMesh.boneWeights.Length > 0)
					{
						cachedSkinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
						cachedRenderer = cachedSkinnedMeshRenderer;
					}
					else
					{
						cachedRenderer = gameObject.AddComponent<MeshRenderer>();
					}
				}
			}

			return cachedRenderer; 
		}
	}

	#endregion



	#region Gettets && Setters

	public ModifiedFlags ModifiedFlag 
	{
		get { return modifiedFlag; }
		set { SetModifiedFlag(value); }
	}


	public Mesh Mesh 
	{
		get
		{
			#if UNITY_EDITOR
			tmUtility.ValidateMesh(mesh);
			#endif

			return mesh; 
		}
		set{ SetMesh(value); }
	}


	public Material Material 
	{
		get { return material; }
		set { SetMaterial(value); }
	}


	public string MainTexCollectionGUID 
	{
		get 
		{
			return mainTexCollectionGUID;
		}
		set 
		{
			if(mainTexCollectionGUID != value)
			{
				ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
				mainTexCollectionGUID = value;
				mainTexCollection = tmManager.Instance.GetMainTexPlatformCollection(mainTexCollectionGUID);
			}
		}
	}


	public tmTextureCollectionPlatform MainTexCollection 
	{
		get 
		{
			if(mainTexCollection == null)
			{
				if(!string.IsNullOrEmpty(MainTexCollectionGUID))
				{
					mainTexCollection = tmManager.Instance.GetMainTexPlatformCollection(MainTexCollectionGUID);
				}				
				if(mainTexCollection != null)
				{
					ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
				}
			}
			return mainTexCollection;
		}
		set
		{
			if(mainTexCollection != value)
			{
				mainTexCollection = value;
				if(mainTexCollection != null)
				{
					mainTexCollectionGUID = tmUtility.PlatformlessPath(mainTexCollection.collectionGuid);
				}
				ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
			}
		}
	}


	public string MainTextureID 
	{
		get { return mainTextureID; }
		set { SetMainTextureID(value); }
	}


	public string MainTextureName 
	{
		get
		{ 
			return MainTexCollection.GetTextureDefenitionByID(MainTextureID).textureName;; 
		}
		set 
		{ 
			MainTextureID = MainTexCollection.GetTextureDefenitionByName(value).textureGuid;
		}
	}


	public tmTextureDefenition MainTextureDefenition
	{
		get 
		{
			if(MainTexCollection != null)
			{
				return MainTexCollection.GetTextureDefenitionByID(MainTextureID);
			}
			return null; 
		}
	}


	public string LightmapCollectionGUID 
	{
		get 
		{
			return lightmapCollectionGUID;
		}
		set 
		{
			if(lightmapCollectionGUID != value)
			{
				ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
				lightmapCollectionGUID = value;
				lightmapCollection = tmManager.Instance.GetLightmapPlatformCollection(lightmapCollectionGUID);
			}
		}
	}


	public tmTextureCollectionPlatform LightmapCollection 
	{
		get 
		{
			if(lightmapCollection == null)
			{
				if(!string.IsNullOrEmpty(LightmapCollectionGUID))
				{
					lightmapCollection = tmManager.Instance.GetLightmapPlatformCollection(LightmapCollectionGUID);
				}
				if(lightmapCollection != null)
				{
					ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
				}
			}
			return lightmapCollection;
		}
		set
		{
			if(lightmapCollection != value)
			{
				lightmapCollection = value;
				if(lightmapCollection != null)
				{
					lightmapCollectionGUID = tmUtility.PlatformlessPath(lightmapCollection.collectionGuid);
				}
				ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
			}
		}
	}


	public string LightmapTextureID 
	{
		get { return lightmapTextureID; }
		set { SetLightmapTextureID(value); }
	}


	public tmTextureDefenition LightmapTextureDefenition
	{
		get 
		{
			if(LightmapCollection != null)
			{
				return LightmapCollection.GetTextureDefenitionByID(LightmapTextureID);
			}
			return null; 
		}
	}


	protected virtual void SetModifiedFlag(ModifiedFlags value)
	{
		if(modifiedFlag == 0 && value != 0)
		{
			tmManager.Instance.RegisterModifiedRender(this);
		}

		modifiedFlag = value;
	}


	protected virtual void SetMesh(Mesh value)
	{
		if(mesh != value)
		{
			ModifiedFlag |= ModifiedFlags.ModifiedMesh;
			mesh = value;

			#if UNITY_EDITOR
			if(mesh != null && (GetComponent<MeshFilter>() == null) && (GetComponent<SkinnedMeshRenderer>() == null))
			{
				if(mesh.bindposes.Length == 0)
				{
					cachedMeshFilter = gameObject.AddComponent<MeshFilter>();
				}
				else
				{
					cachedSkinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				}
			}
			tmUtility.ValidateMesh(mesh);
			#endif
		}
	}


	protected virtual void SetMaterial(Material value)
	{
		if(material != value)
		{
			modifiedFlag |= ModifiedFlags.ModifiedMaterial;
			material = value;
			UpdateMaterial();
		}
	}


	protected virtual void SetMainTextureID(string value)
	{
		if(mainTextureID != value)
		{
			ModifiedFlag |= (ModifiedFlags.ModifiedUV1 | ModifiedFlags.ModifiedMaterial);
			mainTextureID = value;
		}
	}


	protected virtual void SetLightmapTextureID(string value)
	{
		if(lightmapTextureID != value)
		{
			ModifiedFlag |= (ModifiedFlags.ModifiedUV2 | ModifiedFlags.ModifiedMaterial);
			lightmapTextureID = value;
		}
	}

	#endregion



	#region tmIRebuildable implementation

	public virtual void Rebuild()
	{
		UpdateMaterial();
		BuildMesh();

		#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
		#endif
	}

	#endregion



	#region Unity

	void Reset()
	{
		if(Mesh == null)
		{
			MeshFilter mf = GetComponent<MeshFilter>();
			if(mf != null)
			{
				Mesh = mf.sharedMesh;
			}

			SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
			if(smr != null)
			{
				Mesh = smr.sharedMesh;
			}
		}
	}


	protected virtual void Awake()
	{
        cachedMeshFilter = GetComponent<MeshFilter>();
        cachedSkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

		ModifiedFlag =
			ModifiedFlags.ModifiedMesh |
			ModifiedFlags.ModifiedMaterial |
			ModifiedFlags.ModifiedUV1 |
			ModifiedFlags.ModifiedUV2;
	}


	protected virtual void OnEnable()
	{

	}


	protected virtual void OnDisable()
	{

	}


	protected virtual void OnDestroy()
	{

	}

	#endregion



	#region Private

	protected virtual void BuildMesh()
	{
		if(Mesh != null && !string.IsNullOrEmpty(MainTextureID) && MainTexCollection != null)
		{
			tmTextureDefenition mainTextureDefenition = MainTextureDefenition;

			if(mainTextureDefenition != null)
			{
				tmTextureDefenition lightmapTextureDefenition = LightmapTextureDefenition;

				if ((ModifiedFlag & (ModifiedFlags.ModifiedMesh | ModifiedFlags.ModifiedUV1 | ModifiedFlags.ModifiedUV2)) != 0)
				{
					tmManager.Instance.MeshInstance(Mesh, out sharedMesh, mainTextureDefenition, lightmapTextureDefenition, false);
					RenderSharedMesh = sharedMesh;
				}
			}
		}

		ModifiedFlag &= ~(ModifiedFlags.ModifiedMesh | ModifiedFlags.ModifiedUV1 | ModifiedFlags.ModifiedUV2);
	}


	protected virtual void UpdateMaterial()
	{
		if((ModifiedFlag & ModifiedFlags.ModifiedMaterial) != 0)
		{
			if(Material != null)
			{
				this.Material.mainTexture = null;
				this.Material.SetLightmapTexture(null);

				sharedMaterial = MaterialInstance(
					Material,
					MainTexCollection, 
					LightmapCollection
				);
				
                CachedRenderer.sharedMaterial = sharedMaterial;
			}

			ModifiedFlag &= ~ModifiedFlags.ModifiedMaterial;
		}
	}



	protected virtual Material MaterialInstance(Material original, tmTextureCollectionPlatform mainCollection, tmTextureCollectionPlatform lightmapCollection)
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

		Material copy;
		if(tmManager.Instance.GetSharedMaterial(original, mainCollection, lightmapCollection, hashKey, out copy))
		{
			copy.name = original.name + "_" + materialUniqueName;
			copy.hideFlags = HideFlags.HideAndDontSave;
		}

		return copy;
	}
	#endregion
}
