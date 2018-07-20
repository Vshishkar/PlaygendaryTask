using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class SpriteRenderInitializer : MonoBehaviour, tk2dRuntime.ISpriteCollectionForceBuild
{
    #region Variables

	static Hashtable spritesCache = new Hashtable();


    RectTransform cachedRectTransform;
    Transform cachedTransfrom;


    public RectTransform CachedRectTransform
    {
        get
        {
            if (cachedRectTransform == null)
            {
                cachedRectTransform = GetComponent<RectTransform>();
            }

            return cachedRectTransform;
        }
    }

    public Transform CachedTransfrom
    {
        get
        {
            if (cachedTransfrom == null)
            {
                cachedTransfrom = GetComponent<Transform>();
            }
            return cachedTransfrom;
        }
    }

    SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return spriteRenderer;
        }
    }

    /*
    UnityEngine.UI.Image uiImage;

    public UnityEngine.UI.Image UIImage
    {
        get
        {
            if (uiImage == null)
            {
                uiImage = GetComponent<UnityEngine.UI.Image>();
            }
            return uiImage;
        }
    }
    */

    [SerializeField] protected int _spriteId = 0;
    public int SpriteID 
    { 
        get { return _spriteId; } 
        set 
        {
            if (value != _spriteId)
            {
                _spriteId = Mathf.Clamp(value, 0, CollectionInst.spriteDefinitions.Length - 1);
            }
        } 
    }


	public string SpriteName
	{
		get
		{
			return CollectionInst.GetNameForSpriteID(SpriteID);
		}
	}

    [SerializeField] tk2dSpriteCollectionData collection;
    public tk2dSpriteCollectionData Collection 
    { 
        get { return collection; } 
        set { collection = value; collectionInst = collection.inst; } 
    }

    private tk2dSpriteCollectionData collectionInst;
    protected tk2dSpriteCollectionData CollectionInst
    {
        get
        {
            if (collectionInst == null && collection != null)
            {
                collectionInst = collection.inst;
            }

            return collectionInst;
        }
    }
	
    public tk2dSpriteDefinition CurrentSprite 
    {
        get 
        {
            return (CollectionInst == null) ? null : CollectionInst.spriteDefinitions[_spriteId];
        }
    }

	[SerializeField] tk2dBaseSprite.Anchor anchorPoint = tk2dBaseSprite.Anchor.MiddleCenter;

	public tk2dBaseSprite.Anchor AnchorPoint
	{
		get { return anchorPoint; }
		set { anchorPoint = value; }
	}

	public Vector2 AnchorPointVect2
	{
		get 
		{
			Vector2 result = Vector2.zero;

			switch (anchorPoint)
			{
			case tk2dBaseSprite.Anchor.LowerCenter:
				result = new Vector2(0.5f, 0);
				break;

			case tk2dBaseSprite.Anchor.LowerLeft:
				result = new Vector2(0, 0);
				break;

			case tk2dBaseSprite.Anchor.LowerRight:
				result = new Vector2(1f, 0);
				break;

			case tk2dBaseSprite.Anchor.MiddleCenter:
				result = new Vector2(0.5f, 0.5f);
				break;

			case tk2dBaseSprite.Anchor.MiddleLeft:
				result = new Vector2(0, 0.5f);
				break;

			case tk2dBaseSprite.Anchor.MiddleRight:
				result = new Vector2(1, 0.5f);
				break;

			case tk2dBaseSprite.Anchor.UpperCenter:
				result = new Vector2(0.5f, 1);
				break;

			case tk2dBaseSprite.Anchor.UpperLeft:
				result = new Vector2(0, 1);
				break;

			case tk2dBaseSprite.Anchor.UpperRight:
				result = new Vector2(1, 1);
				break;
			}

			return result;
		} 
	}

	[SerializeField] int targetPixelsPerMeter = 100;

	public int TargetPixelsPerMeter
	{
		get { return targetPixelsPerMeter; }
		set { targetPixelsPerMeter = value; }
	}


	public int PixelsPerMeter
	{
		get
		{
			int result = TargetPixelsPerMeter;

			if (tk2dSystem.IsRetina)
			{
				result *= 2;
			}

			return result;
		}
	}

    #endregion

    #region Private Methods

    void UpdateSpriteRenderer()
    {
        if (CurrentSprite != null && CurrentSprite.uvs.Length > 0 && CurrentSprite.boundsData.Length > 1)
        {      
			Sprite newSprite = null;
			if ( spritesCache.ContainsKey(CurrentSprite) && (spritesCache[CurrentSprite] != null) && ((Sprite)spritesCache[CurrentSprite]) && Application.isPlaying)
			{
				newSprite = spritesCache[CurrentSprite] as Sprite;
			}
			else
			{
	            Texture2D mainTexture = CurrentSprite.materialInst.mainTexture as Texture2D;

	            Rect spriteRect = new Rect(mainTexture.width * CurrentSprite.uvs[0].x ,
	                                  mainTexture.height * CurrentSprite.uvs[0].y ,
	                                  CurrentSprite.boundsData[1].x,
	                                  CurrentSprite.boundsData[1].y
	                              );

				Bounds ub = CurrentSprite.GetUntrimmedBounds();
				Bounds tb = CurrentSprite.GetBounds();
				float shiftX = ub.size.x * 0.5f - ub.center.x + tb.center.x - tb.size.x * 0.5f;
				float shiftY = ub.size.y * 0.5f - ub.center.y + tb.center.y - tb.size.y * 0.5f;
				Vector2 shift = new Vector2(shiftX, shiftY);
				Vector2 anchorInPixels = new Vector2(AnchorPointVect2.x * ub.size.x, AnchorPointVect2.y * ub.size.y) - shift;
				Vector2 newAnchor = new Vector2(anchorInPixels.x / tb.size.x , anchorInPixels.y / tb.size.y);

				newSprite = Sprite.Create (mainTexture, spriteRect, newAnchor, PixelsPerMeter, 0, SpriteMeshType.FullRect, Vector4.zero);
				spritesCache.Remove(CurrentSprite);
				spritesCache.Add(CurrentSprite, newSprite);
			}

			if (SpriteRenderer != null)
			{
				SpriteRenderer.sprite = newSprite;
			}
            /*
			if (UIImage != null)
			{
				UIImage.sprite = newSprite;
			}
           */         
        }
        else
        {
            SpriteRenderer.sprite = null;
        }
    }

    #region ISpriteCollectionForceBuild implementation
    // tk2dRuntime.ISpriteCollectionEditor
    public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
    {
		return Collection == spriteCollection;
	}
	
	public virtual void ForceBuild()
    {
        if (Collection == null) 
        {
            return;
        }      

        collectionInst = null;

        UpdateSpriteRenderer();
    }
    #endregion

    #endregion


	#region Unity lifecycle

	void Awake()
	{
		//base.Awake();
		
		// This will not be set when instantiating in code
		// In that case, Build will need to be called
		if (Collection)
		{
			// reset spriteId if outside bounds
			// this is when the sprite collection data is corrupt
			if (_spriteId < 0 || _spriteId >= Collection.Count)
				_spriteId = 0;
			
			ForceBuild();
		}
	}

	#endregion


    #region Public Methods

	public void SetSprite(tk2dSpriteCollectionData newCollection, int newSpriteId, tk2dBaseSprite.Anchor anchor) 
	{
        if (Collection != newCollection) 
        {
            Collection = newCollection;
            _spriteId = -1; // force an update, but only when the collection has changed
        }

        SpriteID = newSpriteId;

		AnchorPoint = anchor;

		UpdateSpriteRenderer();
    }

	public void SetSprite(tk2dSpriteCollectionData newCollection, int newSpriteId) 
	{
		SetSprite(newCollection, newSpriteId, AnchorPoint);
	}

	public bool SetSprite(tk2dSpriteCollectionData newCollection, string spriteName, tk2dBaseSprite.Anchor anchor) 
	{
        int spriteId = newCollection.GetSpriteIdByName(spriteName, -1);
        if (spriteId != -1) { 
            SetSprite(newCollection, spriteId, anchor);
        }
        else {
            CustomDebug.LogError("SetSprite - Sprite not found in collection: " + spriteName);
        }
        return spriteId != -1;
    }

	public bool SetSprite(tk2dSpriteCollectionData newCollection, string spriteName) 
	{
		return SetSprite(newCollection, spriteName, AnchorPoint);
	}
		
    #endregion
}
