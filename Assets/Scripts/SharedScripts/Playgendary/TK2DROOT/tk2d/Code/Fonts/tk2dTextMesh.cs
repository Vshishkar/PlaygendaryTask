using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("2D Toolkit/Text/tk2dTextMesh")]
/// <summary>
/// Text mesh
/// </summary>
public class tk2dTextMesh : tk2dBaseMesh, tk2dRuntime.ISpriteCollectionForceBuild, ILayoutCellHandler
{
	// Holding the data in this struct for the next version
    [SerializeField] tk2dTextMeshData data = new tk2dTextMeshData();

    public tk2dTextMeshData Data
    {
        get
        {
            return data;
        }
    }

	// Batcher needs to grab this
	public tk2dColoredText FormattedText 
	{
		get { return data.FormattedText; }
	}


	/* currentNumberOfCharacters is the actual number of characters allocated in mesh
	 * can be used to determine whether mesh should be expanded
	 * */
	int currentNumberOfCharacters = 0;
	Vector3[] vertices = new Vector3[0];
	Vector2[] uvs = new Vector2[0];
	Vector2[] uv2 = new Vector2[0];
	Color32[] colors = new Color32[0];
	Color32[] untintedColors = new Color32[0];
	int[] triangles = new int[0];


    tk2dFontData defaultFontData;


    #region ILayoutCellHandler implementation

    public void RepositionForCell(LayoutCellInfo info)
    {
        this.MaxTextLength = info.cellRect.width;
        this.MaxTextHeight = info.cellRect.height;

        if (info.anchor == AnchorType.Top)
        {
            this.anchor = TextAnchor.UpperCenter;
        }
        else if (info.anchor == AnchorType.Bottom)
        {
            this.anchor = TextAnchor.LowerCenter;
        }
        else if (info.anchor == AnchorType.Center)
        {
            this.anchor = TextAnchor.MiddleCenter;
        }
        else if (info.anchor == AnchorType.Left)
        {
            this.anchor = TextAnchor.MiddleLeft;
        }
        else if (info.anchor == AnchorType.Right)
        {
            this.anchor = TextAnchor.MiddleRight;
        }
       
        if (info.anchor == AnchorType.Center)
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Left)
        {
            transform.localPosition = new Vector3(-info.cellRect.width * 0.5f, transform.localPosition.y, transform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Right)
        {
            transform.localPosition = new Vector3(info.cellRect.width * 0.5f, transform.localPosition.y, transform.localPosition.z);
        }

        if (info.anchor == AnchorType.Center)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Bottom)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, -info.cellRect.height * 0.5f, transform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Top)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, info.cellRect.height * 0.5f, transform.localPosition.z);
        }    

    }

    #endregion



	[System.FlagsAttribute]
	enum UpdateFlags
	{
		UpdateNone		= 0,
		UpdateAll 		= 1 << 0,
	};
	UpdateFlags updateFlags = UpdateFlags.UpdateNone;


	void SetNeedUpdate(UpdateFlags uf)
	{
		if (updateFlags == UpdateFlags.UpdateNone) 
		{
			updateFlags |= uf;
			tk2dUpdateManager.QueueCommit(this);
		}
		else 
		{
			// Already queued
			updateFlags |= uf;
		}
	}


	// accessors
	/// <summary>Gets or sets the font. Call <see cref="Commit"/> to commit changes.</summary>
	public tk2dFontData font 
	{ 
		get { return data.Font; } 
		set 
		{
			data.Font = value; 
			SetNeedUpdate( UpdateFlags.UpdateAll );

			UpdateMaterial();
		} 
	}


	public tk2dFontData FontInst
	{
		get
		{
			return data.FontInst;
		}
	}


	/// <summary>Gets or sets the text. Call <see cref="Commit"/> to commit changes.</summary>
	public string text 
	{ 
		get { return data.Text; } 
		set 
		{
			data.Text = value;
			SetNeedUpdate(UpdateFlags.UpdateAll);
		}
	}

	/// <summary>Gets or sets the color. Call <see cref="Commit"/> to commit changes.</summary>
	public Color color { get { return data.color; } set { data.color = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Gets or sets the secondary color (used in the gradient). Call <see cref="Commit"/> to commit changes.</summary>
	public Color color2 { get { return data.color2; } set { data.color2 = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Gets or sets the styling color (not used in the gradient). Call <see cref="Commit"/> to commit changes.</summary>
	public Color colorS { get { return data.colorS; } set { data.colorS = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Use vertex vertical gradient. Call <see cref="Commit"/> to commit changes.</summary>
	public bool useGradient { get { return data.useGradient; } set { data.useGradient = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Gets or sets the text anchor. Call <see cref="Commit"/> to commit changes.</summary>
	public TextAnchor anchor { get { return data.anchor; } set { data.anchor = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Gets or sets the scale. Call <see cref="Commit"/> to commit changes.</summary>
	public Vector3 scale { get { return data.UserScale; } set { data.UserScale = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Gets or sets kerning state. Call <see cref="Commit"/> to commit changes.</summary>
	public bool kerning { get { return data.kerning; } set { data.kerning = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Gets or sets the default texture gradient. 
	/// You can also change texture gradient inline by using ^1 - ^9 sequences within your text.
	/// Call <see cref="Commit"/> to commit changes.</summary>
	public int textureGradient { get { return data.textureGradient; } set { data.textureGradient = value; SetNeedUpdate(UpdateFlags.UpdateAll); } }
	/// <summary>Additional spacing between characters. 
	/// This can be negative to bring characters closer together.
	/// Call <see cref="Commit"/> to commit changes.</summary>
	public float Spacing { get { return data.Spacing; } set { if (data.Spacing != value) { data.Spacing = value; SetNeedUpdate(UpdateFlags.UpdateAll); } } }
	/// <summary>Additional line spacing for multieline text. 
	/// This can be negative to bring lines closer together.
	/// Call <see cref="Commit"/> to commit changes.</summary>
	public float LineSpacing { get { return data.LineSpacing; } set { if (data.LineSpacing != value) { data.LineSpacing = value; SetNeedUpdate(UpdateFlags.UpdateAll); } } }

    public float MaxTextLength
    {
        get
        {
            return data.MaxTextLength;
        }
        set
        {
			if (data.MaxTextLength != value)
            {
				data.MaxTextLength = value;
				SetNeedUpdate(UpdateFlags.UpdateAll);
			}
		}
	}

    public float MaxTextHeight
    {
        get
        {
            return data.MaxTextHeight;
        }
        set
        {
            if (data.MaxTextHeight != value)
            {
                data.MaxTextHeight = value;
                SetNeedUpdate(UpdateFlags.UpdateAll);
            }
        }
    }

    public tk2dTextMeshData.LineBreakMode BreakMode
    {
        get
        {
			return data.BreakMode;
        }
        set
        {
			if (data.BreakMode != value)
            {
				data.BreakMode = value;
				SetNeedUpdate(UpdateFlags.UpdateAll);
			}
		}
	}

    public bool ShouldScaleInCellSize
    {
        get
        {
            return data.ShouldScaleInCellSize;
        }
        set
        {
            if (data.ShouldScaleInCellSize != value)
            {
                data.ShouldScaleInCellSize = value;
                SetNeedUpdate(UpdateFlags.UpdateAll);
            }
        }
    }

    public bool DisableNumberGrouping
    {
        get
        {
			return data.DisableNumberGrouping;
        }
        set
        {
			if (DisableNumberGrouping != value)
            {
				data.DisableNumberGrouping = value;
				SetNeedUpdate(UpdateFlags.UpdateAll);
			}
		}
	}

    public bool UseShortNumbers
    {
        get
        {
            return data.UseShortNumbers;
        }

        set
        {
            if (UseShortNumbers != value)
            {
                data.UseShortNumbers = value;
                SetNeedUpdate(UpdateFlags.UpdateAll);
            }
        }
    }

	/// <summary>
	/// Gets or sets the sorting order
	/// The sorting order lets you override draw order for sprites which are at the same z position
	/// It is similar to offsetting in z - the sprite stays at the original position
	/// This corresponds to the renderer.sortingOrder property in Unity 4.3
	/// </summary>
	public int SortingOrder 
	{ 
		get 
		{
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
			return data.renderLayer; 
#else
			return CachedRenderer.sortingOrder;
#endif
		}
		set 
		{
#if (UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
			if (data.renderLayer != value) 
			{
				data.renderLayer = value; SetNeedUpdate(UpdateFlags.UpdateText);
			} 
#else
			if (CachedRenderer.sortingOrder != value) 
			{
				data.renderLayer = value; // for awake
				CachedRenderer.sortingOrder = value;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(CachedRenderer);
#endif
			}
#endif
		} 
	}


    public tk2dFontData DefaultFontData
    {
        get
        {
            return defaultFontData;
        }
    }


	void RebuildAll()
	{
		UpdateMaterial();

		updateFlags = UpdateFlags.UpdateNone;
		SetNeedUpdate(UpdateFlags.UpdateAll);
	}


	// Use this for initialization
	void Awake() 
	{
        if (font != null)
        {
            defaultFontData = font;
        }

		RebuildAll();
	}

#if UNITY_EDITOR
	private void OnEnable() 
	{
		if (CachedRenderer != null 
		    && data != null 
		    && data.Font != null
		    && data.FontInst != null 
		    && CachedRenderer.sharedMaterial == null 
		    && data.FontInst.needMaterialInstance) 
		{
			RebuildAll();
		}
	}
#endif


	public Bounds GetEstimatedMeshBoundsForString(string str)
	{
		return data.GetEstimatedMeshBoundsForString(str);
	}


    public Bounds CurrentMeshBounds
    {
		get
		{
	        return data.EstimatedMeshBounds;
		}
    }



	protected override void RoundShiftChanged()
	{
		SetNeedUpdate(UpdateFlags.UpdateAll);
	}


	// this function is static to ensure that no fields are affected by this call
	private static void SetVertices(Vector3[] curVertices, 
	                                Vector2[] curUVs,
	                                Vector2[] curUV2s, 
	                                Color32[] untintedColors,
	                                Vector3 meshCustomOffset,
	                                bool isRoundShiftDisabled,
	                                Vector3 totalShift,
	                                tk2dTextGeomGen.GeomData curGeomData)
	{
		curGeomData.SetTextMeshGeom(curVertices, curUVs, curUV2s, untintedColors, 0);
		
		//set custom font offset
		for (int curVert = 0; curVert < curVertices.Length; curVert += 1)
		{
			if (!isRoundShiftDisabled)
			{
				curVertices[curVert] = (curVertices[curVert].FloorVector() - totalShift);
			}

			if (Mathf.Abs(meshCustomOffset.x) > float.Epsilon || Mathf.Abs(meshCustomOffset.y) > float.Epsilon)
			{
				curVertices[curVert] += meshCustomOffset;
			}
		}
	}
	
	
	private void SetColors(Color32[] dest)
	{
		Color32 topColor = data.color;
		Color32 bottomColor = data.useGradient ? data.color2 : data.color;
		for (int i = 0; i < dest.Length; ++i) 
		{
			Color32 c = ((i % 4) < 2) ? topColor : bottomColor;
			byte red = (byte)(((int)untintedColors[i].r * (int)c.r) / 255);
			byte green = (byte)(((int)untintedColors[i].g * (int)c.g) / 255);
			byte blue = (byte)(((int)untintedColors[i].b * (int)c.b) / 255);
			byte alpha = (byte)(((int)untintedColors[i].a * (int)c.a) / 255);
			if (FontInst.premultipliedAlpha) 
			{
				red = (byte)(((int)red * (int)alpha) / 255);
				green = (byte)(((int)green * (int)alpha) / 255);
				blue = (byte)(((int)blue * (int)alpha) / 255);
			}
			
			// why every time call new Color32(...) if Color32 is struct?
			//dest[i] = new Color32(red, green, blue, alpha);
			
			dest[i].r = red;
			dest[i].g = green;
			dest[i].b = blue;
			dest[i].a = alpha;
		}
	}

	// Do not call this, its meant fo internal use
	public bool DoNotUse__CommitInternal()
	{
		// early return
		if (FontInst == null) return false;


		bool didReallocate = false;
		var geomData = new tk2dTextGeomGen.GeomData(data, currentNumberOfCharacters);
		if (geomData.ReallocRequired)
		{
			// volatile data
			int numVertices;
			int numIndices;
			geomData.GetTextMeshGeomDesc(out numVertices, out numIndices);

			if(vertices.Length != numVertices)
			{
				vertices = new Vector3[numVertices];
				uvs = new Vector2[numVertices];
				colors = new Color32[numVertices];
				untintedColors = new Color32[numVertices];
				if (FontInst.textureGradients)
				{
					uv2 = new Vector2[numVertices];
				}
				triangles = new int[numIndices];
			}

			currentNumberOfCharacters = geomData.RequiredAllocatedCharacters;
			geomData = new tk2dTextGeomGen.GeomData(data, currentNumberOfCharacters);
			
			
			geomData.SetTextMeshIndices(triangles, 0, 0);

			MeshClear();

			didReallocate = true;
		}
		
		
		if ( (updateFlags & UpdateFlags.UpdateAll) != 0 || didReallocate )
		{
			SetVertices(vertices, 
			            uvs, 
			            uv2,
			            untintedColors, 
			            Application.isPlaying ? FontInst.resultMeshCustomOffset : data.Font.resultMeshCustomOffset,
			            isRoundShiftDisabled, 
			            TotalShift, 
			            geomData);

			// update colors since untinted colors are changed
			if (!FontInst.isPacked)
			{
				SetColors(colors);
			}
			else
			{
				colors = untintedColors;
			}


			MeshVertices = vertices;
			MeshUV = uvs;
			if (font.textureGradients)
			{
				MeshUV2 = uv2;
			}
			MeshColors32 = colors;

			if (didReallocate)
			{
				MeshTriangles = triangles;
			}


			MeshRecalculateBounds();
			MeshBounds = tk2dBaseSprite.AdjustedMeshBounds( MeshBounds, data.renderLayer );
		}

		updateFlags = UpdateFlags.UpdateNone;
		return true;
	}

    public Vector3[] LastLetterVertices()
    {
        Vector3[] vert = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            vert[i] = MeshVertices[data.RequiredCharactersCount * 4 - 4 + i];
        }

        return vert;
    }

	/// <summary>
	/// Makes the text mesh pixel perfect to the active camera.
	/// Automatically detects <see cref="tk2dCamera"/> if present
	/// Otherwise uses Camera.main
	/// </summary>
	public void MakePixelPerfect()
	{
		float s = 1.0f;
		tk2dCamera cam = tk2dCamera.CameraForLayer(gameObject.layer);
		if (cam != null)
		{
			if (FontInst.version < 1)
			{
				CustomDebug.LogError("Need to rebuild font.");
			}

			float zdist = (transform.position.z - cam.transform.position.z);
			float textMeshSize = (FontInst.invOrthoSize * FontInst.halfTargetHeight);
			s = cam.GetSizeAtDistance(zdist) * textMeshSize;
		}
		else if (Camera.main)
		{
            #if UNITY_5_6_OR_NEWER
			if (Camera.main.orthographic)
            #else
			if (Camera.main.isOrthoGraphic)
            #endif

			{
				s = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = (transform.position.z - Camera.main.transform.position.z);
				s = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, zdist);
			}
			s *= FontInst.invOrthoSize;
		}
		scale = new Vector3(Mathf.Sign(scale.x) * s, Mathf.Sign(scale.y) * s, Mathf.Sign(scale.z) * s);
	}	
	
	// tk2dRuntime.ISpriteCollectionEditor
	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		if (data.Font != null && data.Font.spriteCollection != null)
			return data.Font.spriteCollection == spriteCollection;
		
		// No easy way to identify this at this stage
		return true;
	}
	
	void UpdateMaterial()
	{
		if (FontInst != null)
		{
			CurrentMaterial = FontInst.materialInst;
		}
	}
	
	public void ForceBuild()
	{
		RebuildAll();
	}
	

#if UNITY_EDITOR
	void OnDrawGizmos() 
	{
		Bounds b = MeshBounds;
		Gizmos.color = Color.clear;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(b.center, b.extents * 2);
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.white;
	}
#endif
}
