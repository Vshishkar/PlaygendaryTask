using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class tk2dCollider2DData {
	public Vector2[] points = new Vector2[0];	
}

[System.Serializable]
/// <summary>
/// Advanced collider definitions
/// </summary>
public class tk2dSpriteColliderDefinition {
	// This is used to quickly get a collider of this "type" from the physics manager
	public enum Type {
		Box,
		Circle,
	}

	/// <summary>
	/// Type of collider
	/// </summary>
	public Type type = Type.Box;

	/// <summary>
	/// Origin of the collider relative to the sprite
	/// </summary>
	public Vector3 origin;

	/// <summary>
	/// Rotation angle of the collider. Meaningless on Circle shapes.
	/// </summary>
	public float angle;

	public string name = "";

	public Vector3[] vectors = new Vector3[0];
	public float[] floats = new float[0];

	public tk2dSpriteColliderDefinition( Type type, Vector3 origin, float angle ) {
		this.type = type;
		this.origin = origin;
		this.angle = angle;
	}

	/// <summary>
	/// The radius of a Circle collider
	/// </summary>
	public float Radius {
		get {
			return type == Type.Circle ? floats[0] : 0;
		}
	}

	/// <summary>
	/// The size of the box collider
	/// </summary>
	public Vector3 Size {
		get {
			return type == Type.Box ? vectors[0] : Vector3.zero;
		}
	}
}


[System.Serializable]
/// <summary>
/// Sprite Definition.
/// </summary>
public class tk2dSpriteDefinition
{
	/// <summary>
	/// Collider type.
	/// </summary>
	public enum ColliderType
	{
		/// <summary>
		/// Do not create or destroy anything.
		/// </summary>
		Unset,
		
		/// <summary>
		/// If a collider exists, it will be destroyed. The sprite will be responsible in making sure there are no other colliders attached.
		/// </summary>
		None,
		
		/// <summary>
		/// Create a box collider.
		/// </summary>
		Box,
		
		/// <summary>
		/// Create a mesh collider.
		/// </summary>
		Mesh,

		/// <summary>
		/// Will use a custom physics engine. User is responsible in managing colliders.
		/// </summary>
		Custom,
	}

	/// <summary>
	/// Physics engine.
	/// </summary>
	public enum PhysicsEngine
	{
		Physics3D,
		Physics2D
	}

	/// <summary>
	/// Name
	/// </summary>
	public string name;
	
	public Vector3[] boundsData;
	public Vector3[] untrimmedBoundsData;

	public Vector2 texelSize;
	
	/// <summary>
	/// Array of positions for sprite geometry.
	/// </summary>
    public Vector3[] positions;
	
	/// <summary>
	/// Array of normals for sprite geometry, zero length array if they dont exist
	/// </summary>
	public Vector3[] normals;
	
	/// <summary>
	/// Array of tangents for sprite geometry, zero length array if they dont exist
	/// </summary>
	public Vector4[] tangents;
	
	/// <summary>
	/// Array of UVs for sprite geometry, will match the position count.
	/// </summary>
    public Vector2[] uvs;
	/// <summary>
	/// Array of indices for sprite geometry.
	/// </summary>
    public int[] indices = new int[] { 0, 3, 1, 2, 3, 0 };
	/// <summary>
	/// The material used by this sprite. This is generally the same on all sprites in a colletion, but this is not
	/// true when multi-atlas spanning is enabled.
	/// </summary>
	public Material material;

	[System.NonSerialized]
	public Material materialInst;

	/// <summary>
	/// The material id used by this sprite. This is an index into the materials array and corresponds to the 
	/// material flag above.
	/// </summary>
	public int materialId;
	
	
	/// <summary>
	/// Source texture GUID - this is used by the inspector to find the source image without adding a unity dependency.
	/// </summary>
	public string sourceTextureGUID;
	/// <summary>
	/// Speficies if this texture is extracted from a larger texture source, for instance an atlas. This is used in the inspector.
	/// </summary>
	public bool extractRegion;
	public int regionX, regionY, regionW, regionH;
	
	public enum FlipMode {
		None,
		Tk2d,
		TPackerCW,
	}

	/// <summary>
	/// Specifies if this texture is flipped to its side (rotated) in the atlas
	/// </summary>
	public FlipMode flipped;
	
	/// <summary>
	/// Specifies if this texture has complex geometry
	/// </summary>
	public bool complexGeometry = false;
	
	/// <summary>
	/// Physics engine
	/// </summary>
	public PhysicsEngine physicsEngine = PhysicsEngine.Physics3D;
	
	/// <summary>
	/// Collider type
	/// </summary>
	public ColliderType colliderType = ColliderType.Unset;

	/// <summary>
	/// Advanced custom colliders, set when colliderType == Advanced
	/// </summary>
	public tk2dSpriteColliderDefinition[] customColliders = new tk2dSpriteColliderDefinition[0];
	
	/// <summary>
	/// v0 and v1 are center and size respectively for box colliders when colliderType is Box.
	/// It is an array of vertices, and the geometry defined by indices when colliderType is Mesh.
	/// </summary>
	public Vector3[] colliderVertices; 
	public int[] colliderIndicesFwd;
	public int[] colliderIndicesBack;
	public bool colliderConvex;
	public bool colliderSmoothSphereCollisions;
	public tk2dCollider2DData[] polygonCollider2D = new tk2dCollider2DData[0];
	public tk2dCollider2DData[] edgeCollider2D = new tk2dCollider2DData[0];

	[System.Serializable]
	public class AttachPoint
	{
		public string name = "";
		public Vector3 position = Vector3.zero;
		public float angle = 0;

		public void CopyFrom( AttachPoint src ) {
			name = src.name;
			position = src.position;
			angle = src.angle;
		}

		public bool CompareTo( AttachPoint src ) {
			return (name == src.name && src.position == position && src.angle == angle);
		}
	}

	public AttachPoint[] attachPoints = new AttachPoint[0];
	
	public bool Valid { get { return name.Length != 0; } }

	/// <summary>
	/// Gets the trimmed bounds of the sprite.
	/// </summary>
	/// <returns>
	/// Local space bounds
	/// </returns>
	public Bounds GetBounds()
	{
		return new Bounds(new Vector3(boundsData[0].x, boundsData[0].y, boundsData[0].z),
		                  new Vector3(boundsData[1].x, boundsData[1].y, boundsData[1].z));
	}
	
	/// <summary>
	/// Gets untrimmed bounds of the sprite.
	/// </summary>
	/// <returns>
	/// Local space untrimmed bounds
	/// </returns>
	public Bounds GetUntrimmedBounds()
	{
		return new Bounds(new Vector3(untrimmedBoundsData[0].x, untrimmedBoundsData[0].y, untrimmedBoundsData[0].z),
		                  new Vector3(untrimmedBoundsData[1].x, untrimmedBoundsData[1].y, untrimmedBoundsData[1].z));
	}
}

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollectionData")]
/// <summary>
/// Sprite Collection Data.
/// </summary>
public class tk2dSpriteCollectionData : MonoBehaviour 
{
	public const int CURRENT_VERSION = 3;
	
    //These events should be used only for debug purposes
    public static event System.Action<tk2dSpriteCollectionData> OnSpriteCollectionInitialize;
    public static event System.Action<tk2dSpriteCollectionData> OnSpriteCollectionReset;

	public int version;
	public bool materialIdsValid = false;
	public bool needMaterialInstance = false;
	public bool Transient { get; set; } // this should not get serialized

	/// <summary>
	/// An array of sprite definitions.
	/// </summary>
    public tk2dSpriteDefinition[] spriteDefinitions;
	
	/// <summary>
	/// Dictionary to look up sprite names. This will be initialized on first call to GetSpriteIdByName.
	/// </summary>
	Dictionary<string, int> spriteNameLookupDict = null;
	
	/// <summary>
	/// Whether premultiplied alpha is enabled on this sprite collection. This affects how tint colors are computed.
	/// </summary>
    public bool premultipliedAlpha;
	
	/// <summary>
	/// Only exists for backwards compatibility. Do not use or rely on this.
	/// </summary>
	public Material material;	
	
	/// <summary>
	/// An array of all materials used by this sprite collection.
	/// </summary>
	public Material[] materials;

	[System.NonSerialized]
	public Material[] materialInsts;

	[System.NonSerialized]
	public Texture2D[] textureInsts = new Texture2D[0];


	/// <summary>
	/// An array of all textures used by this sprite collection.
	/// </summary>
	public Texture[] textures;

	/// <summary>
	/// An array of PNG textures used by this sprite collection.
	/// </summary>
	public TextAsset[] pngTextures = new TextAsset[0];

    /// <summary>
    /// An array of streaming PNG textures used by this sprite collection.
    /// </summary>
    public string[] streamingTextures = new string[0];

    /// <summary>
    /// An array of width PNG textures used by this sprite collection.
    /// </summary>
    public int[] widthTextures = new int[0];

    /// <summary>
    /// An array of height PNG textures used by this sprite collection.
    /// </summary>
    public int[] heightTextures = new int[0];

    public int[] materialPngTextureId = new int[0];

	// Used only for PNG textures
	public FilterMode textureFilterMode = FilterMode.Bilinear;
	public bool textureMipMaps = false;
    public bool textureLoadAsync = false;
	
	/// <summary>
	/// Specifies if sprites span multiple atlases.
	/// </summary>
	public bool allowMultipleAtlases;
	
	/// <summary>
	/// The sprite collection GUI.
	/// </summary>
	public string spriteCollectionGUID;
	
	/// <summary>
	/// The name of the sprite collection.
	/// </summary>
	public string spriteCollectionName;

	/// <summary>
	/// Asset Name, used to load the asset
	/// </summary>
	public string assetName = "";	
	
	/// <summary>
	/// Is this asset loadable using tk2dSystem
	/// </summary>
	public bool loadable = false;
	
	/// <summary>
	/// The size of the inv ortho size used to generate the sprite collection.
	/// </summary>
	public float invOrthoSize = 1.0f;
	
	/// <summary>
	/// Target height used to generate the sprite collection.
	/// </summary>
	public float halfTargetHeight = 1.0f;
	
	public int buildKey = 0;
	
	/// <summary>
	/// GUID of this object, used with <see cref="tk2dIndex"/>
	/// </summary>
	public string dataGuid = "";

	/// <summary>
	/// Returns the number of sprite definitions in this sprite collection.
	/// </summary>
    public int Count { get { return inst.spriteDefinitions.Length; } }

	/// <summary>
	/// When true, sprite collection will not be directly selectable
	/// </summary>
    public bool managedSpriteCollection = false;

	/// <summary>
	/// When true, spriteCollectionPlatforms & PlatformGUIDs are expected to have
	/// sensible data.
	/// </summary>
	public bool hasPlatformData = false;

	/// <summary>
	/// Returns an array of platform names.
	/// </summary>
    public string[] spriteCollectionPlatforms = null;

	/// <summary>
	/// Returns an array of GUIDs, each referring to an actual tk2dSpriteCollectionData object
	/// This object contains the actual sprite collection for the platform.
	/// </summary>
    public string[] spriteCollectionPlatformGUIDs = null;

	/// <summary>
	/// Resolves a sprite name and returns a unique id for the sprite.
	/// </summary>
	/// <returns>
	/// Unique Sprite Id. 0 if sprite isn't found.
	/// </returns>
	/// <param name='name'>Case sensitive sprite name, as defined in the sprite collection. This is usually the source filename excluding the extension</param>
	public int GetSpriteIdByName(string name)
	{
		return GetSpriteIdByName(name, 0);
	}
	
	/// <summary>
	/// Resolves a sprite name and returns a unique id for the sprite.
	/// </summary>
	/// <returns>
	/// Unique Sprite Id. defaultValue if sprite isn't found.
	/// </returns>
	/// <param name='name'>Case sensitive sprite name, as defined in the sprite collection. This is usually the source filename excluding the extension</param>
	/// <param name='defaultValue'>The value which is returned when the named sprite can't be found.</param>
	public int GetSpriteIdByName(string name, int defaultValue)
	{
		inst.InitDictionary();
		int returnValue = defaultValue;
		if (!inst.spriteNameLookupDict.TryGetValue(name, out returnValue)) return defaultValue;
		return returnValue; // default to first sprite
	}

	/// <summary>
	/// Resolves a sprite name and returns a reference to a sprite definition
	/// </summary>
	/// <returns>
	/// Unique Sprite Definition. null if sprite isn't found.
	/// </returns>
	/// <param name='name'>Case sensitive sprite name, as defined in the sprite collection. This is usually the source filename excluding the extension</param>
	public tk2dSpriteDefinition GetSpriteDefinition(string name) {
		int id = GetSpriteIdByName(name, -1);
		if (id == -1) {
			return null;
		}
		else {
			return spriteDefinitions[id];
		}
	}


	/// <summary>
	/// </summary>
	/// <returns>The slice borders for srite.</returns>
	/// <param name="spriteID">Sprite I.</param>
	public tk2dSliceBorders GetSliceBordersForSrite(int spriteID)
	{
		return tk2dSliceBordersCollector.Instance.GetBordersForID(spriteCollectionName + "+" + GetNameForSpriteID(spriteID));
	}


#if UNITY_EDITOR
	public void SetSliceBordersForSprite(int spriteID, tk2dSliceBorders newBord)
	{
		tk2dSliceBordersCollector.Instance.SetBordersForID(spriteCollectionName + "+" + GetNameForSpriteID(spriteID), newBord);
	}
#endif


	/// <summary>
	/// </summary>
	/// <returns>The name of sprite with the given ID</returns>
	public string GetNameForSpriteID(int spriteID)
	{
		return inst.spriteDefinitions[spriteID].name;
	}

	
	/// <summary>
	/// Initializes the lookup dictionary
	/// </summary>
	public void InitDictionary()
	{
		if (spriteNameLookupDict == null)
		{
			spriteNameLookupDict = new Dictionary<string, int>(spriteDefinitions.Length);
			for (int i = 0; i < spriteDefinitions.Length; ++i)
			{
				spriteNameLookupDict[spriteDefinitions[i].name] = i;
			}
		}
	}
	
	/// <summary>
	/// Returns the first valid sprite definition
	/// </summary>
	public tk2dSpriteDefinition FirstValidDefinition
	{
		get 
		{
			foreach (var v in inst.spriteDefinitions)
			{
				if (v.Valid)
					return v;
			}
			return null;
		}
	}

	/// <summary>
	/// Returns true if the sprite id is valid for this sprite collection
	/// </summary>
	public bool IsValidSpriteId(int id) {
		if (id < 0 || id >= inst.spriteDefinitions.Length) {
			return false;
		}
		return inst.spriteDefinitions[id].Valid;
	}
	
	/// <summary>
	/// Returns the index of the first valid sprite definition
	/// </summary>
	public int FirstValidDefinitionIndex
	{
		get 
		{
			tk2dSpriteCollectionData data = inst;

			for (int i = 0; i < data.spriteDefinitions.Length; ++i)
				if (data.spriteDefinitions[i].Valid)
					return i;
			return -1;
		}
	}
	
	/// <summary>
	/// Internal function to make sure all material Ids are valid. Used in the tilemap editor
	/// </summary>
	public void InitMaterialIds()
	{
		if (inst.materialIdsValid)
			return;
		
		int firstValidIndex = -1;
		Dictionary<Material, int> materialLookupDict = new Dictionary<Material, int>();
		for (int i = 0; i < inst.materials.Length; ++i)
		{
			if (firstValidIndex == -1 && inst.materials[i] != null)
				firstValidIndex = i;
			materialLookupDict[materials[i]] = i;
		}
		if (firstValidIndex == -1)
		{
			CustomDebug.LogError("Init material ids failed.");
		}
		else
		{
			foreach (var v in inst.spriteDefinitions)			
			{
				if (!materialLookupDict.TryGetValue(v.material, out v.materialId))
					v.materialId = firstValidIndex;
			}
			inst.materialIdsValid = true;
		}
	}

	tk2dSpriteCollectionData platformSpecificData = null;

	// Returns the active instance
	public tk2dSpriteCollectionData inst
	{
		get 
		{
			if (platformSpecificData == null)
			{
				if (hasPlatformData)
				{
					string systemPlatform = tk2dSystem.CurrentPlatform;
					string guid = "";

					for (int i = 0; i < spriteCollectionPlatforms.Length; ++i)
					{
						if (spriteCollectionPlatforms[i] == systemPlatform)
						{
//							CustomDebug.Log("Current paltform: " + systemPlatform);
							guid = spriteCollectionPlatformGUIDs[i];
							break;							
						}
					}
                    if (guid.Length == 0)
                    {
                        CustomDebug.LogWarning("failed to find platform by guid!");
                        guid = spriteCollectionPlatformGUIDs[0]; // failed to find platform, pick the first one
                    }

					platformSpecificData = tk2dSystem.LoadResourceByGUID<tk2dSpriteCollectionData>(guid);
				}
				else
				{
					platformSpecificData = this;
				}
			}
            if (platformSpecificData != null)
            {
			    platformSpecificData.Init(); // awake is never called, so we initialize explicitly
            }
			return platformSpecificData;
		}
	}

	public static readonly string internalResourcePrefix = "tk2dInternal$.";
	
	public void Init()
	{
		// check if already initialized
		if (materialInsts != null)
			return;

		if (spriteDefinitions == null) spriteDefinitions = new tk2dSpriteDefinition[0];
		if (materials == null) materials = new Material[0];

		materialInsts = new Material[materials.Length];
		if (needMaterialInstance)
		{
			if (tk2dSystem.OverrideBuildMaterial) {
				// This is a hack to work around a bug in Unity 4.x
				// Scene serialization will serialize the actively bound texture
				// but not the material during the build, only when [ExecuteInEditMode]
				// is on, eg. on sprites.
				for (int i = 0; i < materials.Length; ++i)
				{
					materialInsts[i] = new Material(Shader.Find("tk2d/BlendVertexColor"));
	#if UNITY_EDITOR
					materialInsts[i].hideFlags = HideFlags.DontSave;
	#endif
				}
			}
			else 
            {
				bool assignTextureInst = false;

				if (pngTextures.Length > 0) 
                {
					assignTextureInst = true;
					textureInsts = new Texture2D[pngTextures.Length];
					for (int i = 0; i < pngTextures.Length; ++i) {
						Texture2D tex = new Texture2D(4, 4, TextureFormat.ARGB32, textureMipMaps);
                        tex.name = string.Format("{0}PNG_{1}_{2}", internalResourcePrefix, name, i);
	#if UNITY_EDITOR
						tex.hideFlags = HideFlags.DontSave;
	#endif
						tex.LoadImage(pngTextures[i].bytes);
						textureInsts[i] = tex;
						tex.filterMode = textureFilterMode;
						tex.Apply(textureMipMaps, true);

    #if !UNITY_EDITOR
                        TextAsset asset = pngTextures[i];
                        pngTextures[i] = null;
                        Resources.UnloadAsset(asset);
    #endif
					}
				}

                if (streamingTextures.Length > 0) 
                {
                    assignTextureInst = true;
                    textureInsts = new Texture2D[streamingTextures.Length];
                    for (int i = 0; i < streamingTextures.Length; ++i)
                    {
                        string path = streamingTextures[i];

                        int width = (widthTextures.Length == streamingTextures.Length) ? (widthTextures[i]) : (4);
                        int height = (heightTextures.Length == streamingTextures.Length) ? (heightTextures[i]) : (4);
                        Texture2D tex = (textureLoadAsync) ? 
                            (TextureManager.LoadTextureAsync(path, textureMipMaps, width, height)) : 
                            (TextureManager.LoadTexture(path, textureMipMaps));
                        tex.name = string.Format("{0}PNG_{1}_{2}", internalResourcePrefix, name, i);
                        tex.filterMode = textureFilterMode;

                        #if UNITY_EDITOR
                        tex.hideFlags = HideFlags.DontSave;
                        #endif

                        textureInsts[i] = tex;
                    }
                }
 
				for (int i = 0; i < materials.Length; ++i) 
				{
					materialInsts[i] = Instantiate(materials[i]) as Material;
	#if UNITY_EDITOR
					materialInsts[i].name = string.Format("{0}Material_{1}_{2}", internalResourcePrefix, name, materials[i].name);
					materialInsts[i].hideFlags = HideFlags.DontSave; 
	#endif
					if (assignTextureInst) {
						int textureId = (materialPngTextureId.Length == 0) ? 0 : materialPngTextureId[i];
						materialInsts[i].mainTexture = textureInsts[ textureId ];
					}
				}
			}
			for (int i = 0; i < spriteDefinitions.Length; ++i)
			{
				tk2dSpriteDefinition def = spriteDefinitions[i];
				def.materialInst = materialInsts[def.materialId];
			}
		}
		else
		{
			for (int i = 0; i < materials.Length; ++i) {
				materialInsts[i] = materials[i];
			}
			for (int i = 0; i < spriteDefinitions.Length; ++i)
			{
				tk2dSpriteDefinition def = spriteDefinitions[i];
				def.materialInst = def.material;
			}
		}


#if UNITY_EDITOR
		// Unity from 4.3 when in 2D mode overrides imported textures with alphaIsTransparency set
		// which naturally breaks our old demo scenes. This happens even when meta files
		// are present :(
		if (materialInsts != null 
				&& materialInsts.Length > 0 
				&& materialInsts[0] != null
				&& materialInsts[0].mainTexture != null
				&& materialInsts[0].shader != null
				&& materialInsts[0].shader.name.Contains("Premul")) { // Detect premultiplied textures
			string path = UnityEditor.AssetDatabase.GetAssetPath( materialInsts[0].mainTexture );
			if (path.Length > 0) {
				UnityEditor.TextureImporter importer = UnityEditor.TextureImporter.GetAtPath(path) as UnityEditor.TextureImporter;
				if (importer != null && 
                    (importer.alphaIsTransparency || 
        #if UNITY_5_5_OR_NEWER
                importer.alphaSource != UnityEditor.TextureImporterAlphaSource.FromInput
        #else
                importer.grayscaleToAlpha
        #endif
                    )) 
                {
					if (UnityEditor.EditorUtility.DisplayDialog(
							"Atlas texture incompatibility", 
							string.Format("Atlas texture '{0}' for sprite collection '{1}' must be reimported to display correctly in Unity 4.3 when in 2D mode.", materialInsts[0].mainTexture.name, name), 
							"Reimport")) {
						List<Texture> textures = new List<Texture>();
						for (int i = 0; i < materialInsts.Length; ++i) {
							if (materialInsts[i] != null 
									&& materialInsts[i].mainTexture != null 
									&& !textures.Contains(materialInsts[i].mainTexture) // only do this once
									&& materialInsts[i].shader != null) {
								path = UnityEditor.AssetDatabase.GetAssetPath( materialInsts[i].mainTexture );
								if (path.Length > 0) {
									importer = UnityEditor.TextureImporter.GetAtPath(path) as UnityEditor.TextureImporter;
									if (importer != null && 
                                        (importer.alphaIsTransparency || 
        #if UNITY_5_5_OR_NEWER
                                    importer.alphaSource != UnityEditor.TextureImporterAlphaSource.FromInput
        #else
                                    importer.grayscaleToAlpha
        #endif
                                        ))
                                    {
										importer.alphaIsTransparency = false;
        #if UNITY_5_5_OR_NEWER
                                        importer.alphaSource = UnityEditor.TextureImporterAlphaSource.FromInput;
        #else
										importer.grayscaleToAlpha = false;
        #endif
										UnityEditor.EditorUtility.SetDirty(importer);
										UnityEditor.AssetDatabase.ImportAsset(path);
									}
								}
								textures.Add( materialInsts[i].mainTexture );
							}
						}
					}
				}
			}
		}
#endif

        if (CustomDebug.Enable)
        {
            if (OnSpriteCollectionInitialize != null)
            {
                OnSpriteCollectionInitialize(this);
            }
        }

		tk2dEditorSpriteDataUnloader.Register(this);
	}

	/// <summary>
	/// Create a sprite collection at runtime from a texture and user specified regions.
	/// Please ensure that names, regions & anchor arrays have same dimension.
	/// Use <see cref="tk2dBaseSprite.CreateFromTexture"/> if you need to create only one sprite from a texture.
	/// </summary>
	public static tk2dSpriteCollectionData CreateFromTexture(Texture texture, tk2dSpriteCollectionSize size, string[] names, Rect[] regions, Vector2[] anchors)
	{
		return tk2dRuntime.SpriteCollectionGenerator.CreateFromTexture(texture, size, names, regions, anchors);
	}

	/// <summary>
	/// Create a sprite collection at runtime from a texturepacker exported file.
	/// Ensure this is exported using the "2D Toolkit" export mode in TexturePacker. 
	/// You can find this exporter in Assets/TK2DROOT/tk2d/Goodies/TexturePacker/Exporter
	/// You can use also use this to load sprite collections at runtime.
	/// </summary>
	public static tk2dSpriteCollectionData CreateFromTexturePacker(tk2dSpriteCollectionSize size, string texturePackerData, Texture texture)
	{
		return tk2dRuntime.SpriteCollectionGenerator.CreateFromTexturePacker(size, texturePackerData, texture);
	}


	public void ResetPlatformData()
	{
        if (CustomDebug.Enable)
        {
            if (OnSpriteCollectionReset != null)
            {
                OnSpriteCollectionReset(this);
            }
        }

		tk2dEditorSpriteDataUnloader.Unregister(this);

		if (platformSpecificData != null) 
        {
			platformSpecificData.DestroyTextureInsts();
            platformSpecificData.DestroyMaterialInsts();
		}
		DestroyTextureInsts();
        DestroyMaterialInsts();

		if (platformSpecificData)
		{
			platformSpecificData = null;
		}
		
		materialInsts = null;
	}


	void DestroyTextureInsts()
    {
        if (textureInsts != null)
        {
            for (int i = 0; i < textureInsts.Length; i++) 
            {
                Texture2D tex = textureInsts[i];
                if (tex != null)
                {
                    TextureManager.UnloadTexture(tex);

                    #if UNITY_EDITOR
                    UnityEngine.Object.DestroyImmediate(tex);
                    #else
                    UnityEngine.Object.Destroy(tex);
                    #endif
                }
                textureInsts[i] = null;
            }
		}
		textureInsts = new Texture2D[0];
	}


    void DestroyMaterialInsts()
    {
        if (materialInsts != null)
        {
            for (int i = 0; i < materialInsts.Length; i++)
            {
                Material mat = materialInsts[i];
                if (mat != null)
                {
	                #if UNITY_EDITOR
                    UnityEngine.Object.DestroyImmediate(mat);
                    #else
	                UnityEngine.Object.Destroy(mat);
                    #endif
                }
                materialInsts[i] = null;
            }

            materialInsts = null;
        }

        if (spriteDefinitions != null)
        {
            for (int i = 0; i < spriteDefinitions.Length; i++)
            {
                tk2dSpriteDefinition def = spriteDefinitions[i];
                if (def != null)
                {
                    def.materialInst = null;
                }
            }
        }
    }


	void OnDestroy()
	{
		ResetPlatformData();
	}


    #region Retain handle

    int retainCount;
    HashSet<MonoBehaviour> renderRefs = new HashSet<MonoBehaviour>();


    public int RetainCount 
    {
        get { return retainCount; }
        private set 
        {
            if (retainCount == 0 && value > 0)
            {
//                CustomDebug.Log("load : " + this.spriteCollectionName);
            }

            if (retainCount > 0 && value == 0)
            {
//                CustomDebug.Log("unload : " + this.spriteCollectionName);
//                this.ResetPlatformData();
            }

            retainCount = value;
        }
    }


    /// <summary>
    /// Add retain this sprite collection.
    /// </summary>
    public void AddRetainCount(MonoBehaviour sender)
    {
        if (renderRefs.Add(sender))
        {
            RetainCount++;
        }
    }


    /// <summary>
    /// Remove retain this sprite collection.
    /// </summary>
    public void RemoveRetainCount(MonoBehaviour sender)
    {
        if (renderRefs.Remove(sender))
        {
            RetainCount--;
        }
    }
           
    #endregion
}


#region Extension for Retain Count

public static class tk2dSpriteCollectionDataRetainCount
{
    /// <summary>
    /// Add retain with check null this sprite collection.
    /// </summary>
    public static void AddRetain(this tk2dSpriteCollectionData platfrom, MonoBehaviour sender)
    {
        if (platfrom)
        {
            platfrom.AddRetainCount(sender);
        }
    }


    /// <summary>
    /// Remove retain with check null this sprite collection.
    /// </summary>
    public static void RemoveRetain(this tk2dSpriteCollectionData platfrom, MonoBehaviour sender)
    {
        if (platfrom)
        {
            platfrom.RemoveRetainCount(sender);
        }
    }
}

#endregion
