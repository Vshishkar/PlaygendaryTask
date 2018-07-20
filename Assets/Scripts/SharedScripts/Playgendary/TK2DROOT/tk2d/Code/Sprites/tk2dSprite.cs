using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/Sprite/tk2dSprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
/// <summary>
/// Sprite implementation which maintains its own Unity Mesh. Leverages dynamic batching.
/// </summary>
public class tk2dSprite : tk2dBaseSprite, ILayoutCellHandler
{
	Vector3[] meshVertices;
	Vector3[] meshNormals = null;
	Vector4[] meshTangents = null;
	Color32[] meshColors;

    #region ILayoutCellHandler implementation  

    public void RepositionForCell(LayoutCellInfo info)
    {
        Vector3 untrimmedBounds = GetUntrimmedBounds().size;

        if (isLayoutSizeLocked)
        {
            if (info.anchor == AnchorType.Center)
            {
                CachedTransform.localPosition = new Vector3(0, CachedTransform.localPosition.y, CachedTransform.localPosition.z);
            }
            else if (info.anchor == AnchorType.Left)
            {
                CachedTransform.localPosition = new Vector3((-info.cellRect.width + untrimmedBounds.x) * 0.5f, CachedTransform.localPosition.y, CachedTransform.localPosition.z);
            }
            else if (info.anchor == AnchorType.Right)
            {
                CachedTransform.localPosition = new Vector3((info.cellRect.width - untrimmedBounds.x) * 0.5f, CachedTransform.localPosition.y, CachedTransform.localPosition.z);
            }
            else if (info.anchor == AnchorType.Center)
            {
                CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, 0, CachedTransform.localPosition.z);
            }
            else if (info.anchor == AnchorType.Bottom)
            {
                CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, (-info.cellRect.height + untrimmedBounds.y) * 0.5f, CachedTransform.localPosition.z);
            }
            else if (info.anchor == AnchorType.Top)
            {
                CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, (info.cellRect.height - untrimmedBounds.y) * 0.5f, CachedTransform.localPosition.z);
            }
        }
        else
        {
            this.scale = Vector3.one;
            untrimmedBounds = GetUntrimmedBounds().size;

            this.scale = new Vector3(_scale.x * info.cellRect.width / untrimmedBounds.x, _scale.y * info.cellRect.height / untrimmedBounds.y, _scale.z);

            CachedTransform.localPosition = new Vector3(0,0,CachedTransform.localPosition.z);
        }   


    }

    #endregion
	
	new void Awake()
	{
		base.Awake();
		
		// This will not be set when instantiating in code
		// In that case, Build will need to be called
		if (Collection)
		{
			// reset spriteId if outside bounds
			// this is when the sprite collection data is corrupt
			if (_spriteId < 0 || _spriteId >= Collection.Count)
				_spriteId = 0;
			
			Build();
		}
	}



	/// <summary>
	/// Adds a tk2dSprite as a component to the gameObject passed in, setting up necessary parameters and building geometry.
	/// Convenience alias of tk2dBaseSprite.AddComponent<tk2dSprite>(...).
	/// </summary>
	public static tk2dSprite AddComponent(GameObject go, tk2dSpriteCollectionData spriteCollection, int spriteId)
	{
		return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteId);
	}
	
	/// <summary>
	/// Adds a tk2dSprite as a component to the gameObject passed in, setting up necessary parameters and building geometry.
	/// Convenience alias of tk2dBaseSprite.AddComponent<tk2dSprite>(...).
	/// </summary>
	public static tk2dSprite AddComponent(GameObject go, tk2dSpriteCollectionData spriteCollection, string spriteName)
	{
		return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteName);
	}
	
	/// <summary>
	/// Create a sprite (and gameObject) displaying the region of the texture specified.
	/// Use <see cref="tk2dSpriteCollectionData.CreateFromTexture"/> if you need to create a sprite collection
	/// with multiple sprites. It is your responsibility to destroy the collection when you
	/// destroy this sprite game object. You can get to it by using sprite.Collection.
	/// Convenience alias of tk2dBaseSprite.CreateFromTexture<tk2dSprite>(...)
	/// </summary>
	public static GameObject CreateFromTexture(Texture texture, tk2dSpriteCollectionSize size, Rect region, Vector2 anchor)
	{
		return tk2dBaseSprite.CreateFromTexture<tk2dSprite>(texture, size, region, anchor);
	}
	
    public static void PlaySpriteToFaceAnimation(tk2dSprite sprite)
    {
        tk2dSprite.PlaySpriteToFaceAnimation(sprite, Color.white);
    }

    public static void PlaySpriteToFaceAnimation(tk2dSprite sprite, Color c)
    {
        tk2dSprite spriteCopy = GameObject.Instantiate(sprite) as tk2dSprite;

        spriteCopy.transform.parent = sprite.transform.parent;
        spriteCopy.transform.localPosition = new Vector3(0,0,-5);

        spriteCopy.transform.localScale = Vector3.one;
        spriteCopy.color = c;

        TweenScale iconTweenScale = TweenScale.SetScale(spriteCopy.gameObject, Vector3.one * 5f, 0.3f);

        iconTweenScale.BeginScale = Vector3.one;

        TweenColor iconTweenColor = TweenColor.SetColor(spriteCopy.gameObject, Color.clear, 0.3f);
        iconTweenColor.beginColor = Color.white;
        iconTweenColor.useChanelMask[0] = iconTweenColor.useChanelMask[1] = iconTweenColor.useChanelMask[2] = false;
        iconTweenColor.SetOnFinishedDelegate(delegate 
        {
            Destroy(spriteCopy.gameObject);
        });
    }

	protected override void UpdateGeometry() { UpdateGeometryImpl(); }
	protected override void UpdateColors() { UpdateColorsImpl(); }
	protected override void UpdateVertices() { UpdateVerticesImpl(); }
	

	public override void Build()
	{
		var sprite = CurrentSprite;

		meshVertices = new Vector3[sprite.positions.Length];
        meshColors = new Color32[sprite.positions.Length];
		
		meshNormals = new Vector3[0];
		meshTangents = new Vector4[0];
		
		if (sprite.normals != null && sprite.normals.Length > 0)
		{
			meshNormals = new Vector3[sprite.normals.Length];
		}
		if (sprite.tangents != null && sprite.tangents.Length > 0)
		{
			meshTangents = new Vector4[sprite.tangents.Length];
		}


		MeshClear();

		/// update mesh
		UpdateVerticesImpl();
		UpdateColorsImpl();

		MeshTriangles = sprite.indices;

		UpdateMaterial();
		CreateCollider();
	}
	
	
	protected void UpdateColorsImpl()
	{
		// This can happen with prefabs in the inspector
		if (meshColors == null || meshColors.Length == 0)
			return;

		SetColors(meshColors);
		MeshColors32 = meshColors;
	}
	
	protected void UpdateVerticesImpl()
	{	
		// This can happen with prefabs in the inspector
		if (meshVertices == null || meshVertices.Length == 0 || CollectionInst == null)
			return;
	
		var sprite = CurrentSprite;

		// Clear out normals and tangents when switching from a sprite with them to one without
		if (sprite.normals.Length != meshNormals.Length)
		{
			meshNormals = (sprite.normals != null && sprite.normals.Length > 0)?(new Vector3[sprite.normals.Length]):(new Vector3[0]);
		}
		if (sprite.tangents.Length != meshTangents.Length)
		{
			meshTangents = (sprite.tangents != null && sprite.tangents.Length > 0)?(new Vector4[sprite.tangents.Length]):(new Vector4[0]);
		}
		
		SetPositions(meshVertices, meshNormals, meshTangents);

		MeshVertices = meshVertices;
		MeshNormals = meshNormals;
		MeshTangents = meshTangents;
		MeshUV = sprite.uvs;
		MeshTriangles = sprite.indices;
		MeshBounds = AdjustedMeshBounds( GetBounds(), renderLayer );
	}

	protected void UpdateGeometryImpl()
	{
		// This can happen with prefabs in the inspector
		if (CollectionInst == null)
			return;


		var sprite = CurrentSprite;
		if (meshVertices == null || meshVertices.Length != sprite.positions.Length)
		{
			meshVertices = new Vector3[sprite.positions.Length];
			meshNormals = (sprite.normals != null && sprite.normals.Length > 0)?(new Vector3[sprite.normals.Length]):(new Vector3[0]);
			meshTangents = (sprite.tangents != null && sprite.tangents.Length > 0)?(new Vector4[sprite.tangents.Length]):(new Vector4[0]);
			meshColors = new Color32[sprite.positions.Length];
		}

		MeshClear();

		UpdateVerticesImpl();
		UpdateColorsImpl();
	}
	
	protected override void UpdateMaterial()
	{
		CurrentMaterial = CurrentSprite.materialInst;
	}
	
	protected override int GetCurrentVertexCount()
	{
		if (meshVertices == null)
			return 0;
		// Really nasty bug here found by Andrew Welch.
		return meshVertices.Length;
	}

#if UNITY_EDITOR
	void OnDrawGizmos() 
	{
		if (CollectionInst != null && spriteId >= 0 && spriteId < CollectionInst.Count) 
		{
			var sprite = CurrentSprite;
			Gizmos.color = Color.clear;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube(Vector3.Scale(sprite.untrimmedBoundsData[0], _scale), Vector3.Scale(sprite.untrimmedBoundsData[1], _scale));
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = Color.white;
		}
	}
#endif


	public override void ReshapeBounds(Vector3 dMin, Vector3 dMax) {
		float minSizeClampTexelScale = 0.1f; // Can't shrink sprite smaller than this many texels
		// Irrespective of transform
		var sprite = CurrentSprite;
		Vector3 oldAbsScale = new Vector3(Mathf.Abs(_scale.x), Mathf.Abs(_scale.y), Mathf.Abs(_scale.z));
		Vector3 oldMin = Vector3.Scale(sprite.untrimmedBoundsData[0], _scale) - 0.5f * Vector3.Scale(sprite.untrimmedBoundsData[1], oldAbsScale);
		Vector3 oldSize = Vector3.Scale(sprite.untrimmedBoundsData[1], oldAbsScale);
		Vector3 newAbsScale = oldSize + dMax - dMin;
		newAbsScale.x /= sprite.untrimmedBoundsData[1].x;
		newAbsScale.y /= sprite.untrimmedBoundsData[1].y;
		// Clamp the minimum size to avoid having the pivot move when we scale from near-zero
		if (sprite.untrimmedBoundsData[1].x * newAbsScale.x < sprite.texelSize.x * minSizeClampTexelScale && newAbsScale.x < oldAbsScale.x) {
			dMin.x = 0;
			newAbsScale.x = oldAbsScale.x;
		}
		if (sprite.untrimmedBoundsData[1].y * newAbsScale.y < sprite.texelSize.y * minSizeClampTexelScale && newAbsScale.y < oldAbsScale.y) {
			dMin.y = 0;
			newAbsScale.y = oldAbsScale.y;
		}
		// Add our wanted local dMin offset, while negating the positional offset caused by scaling
		Vector2 scaleFactor = new Vector3(Mathf.Approximately(oldAbsScale.x, 0) ? 0 : (newAbsScale.x / oldAbsScale.x),
			Mathf.Approximately(oldAbsScale.y, 0) ? 0 : (newAbsScale.y / oldAbsScale.y));
		Vector3 scaledMin = new Vector3(oldMin.x * scaleFactor.x, oldMin.y * scaleFactor.y);
		Vector3 offset = dMin + oldMin - scaledMin;
		offset.z = 0;
		transform.position = transform.TransformPoint(offset);
		scale = new Vector3(_scale.x * scaleFactor.x, _scale.y * scaleFactor.y, _scale.z);
	}
}
