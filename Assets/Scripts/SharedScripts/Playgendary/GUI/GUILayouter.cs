using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GUILayouterType
{
    Horizontal,
    Vertical
}

public enum GUILayouterRotationType
{
    Both,
    Portrait,
    Landscape
}

public class GUILayouter : MonoBehaviour, ILayoutCellHandler 
{
    bool isDirty;

    public static float DEFAULT_OFFSET
    {
        get 
        {
            int platformMultiplier = (tk2dSystem.IsRetina ? 2 : 1);

            return 20 * platformMultiplier;
        }
    }
     

	#region Variables

    [HideInInspector][SerializeField] GUILayouterRotationType rotationType = GUILayouterRotationType.Both;
    public GUILayouterRotationType RotationType
    {
        get
        {
            return rotationType;
        }
        set
        {
            if (!Application.isPlaying)
            {
                rotationType = value;
            }
        }
    }

    [SerializeField] GUILayouterType type = GUILayouterType.Horizontal;
    public GUILayouterType Type
    {
        get
        {
            return type;
        }

		set 
		{
			type = value;
		}
    }
    GUILayoutCell[] cells;   

    [SerializeField] bool isRootLayouter = true;

    public bool IsRootLayouter
    {
        get
        {
            return isRootLayouter;
        }
        set
        {
            isRootLayouter = value;
        }
    }


    [SerializeField] bool isInversed;

    Rect? occupiedPixels = null;
     

    Transform cachedTransform;

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

    public Rect? OccupiedPixels
    {
        get
        {
            return occupiedPixels;
        }
        set
        {
            occupiedPixels = value;

            if (cells == null || cells.Length == 0)
            {
                cells = GetComponentsInChildren<GUILayoutCell>(true);

                List<GUILayoutCell> myCells = new List<GUILayoutCell>();

                foreach (var cell in cells)
                {
                    if (cell.transform.parent == CachedTransform)
                    {
                        myCells.Add(cell);
                    }
                }

                cells = myCells.ToArray();
            }

            UpdateLayout();
        }
    }




	#endregion

	#region Unity Lifecycle

    protected virtual void Awake()
    {
        if (IsRootLayouter)
        {
            GUIOrientationManger.OnDeviceOrientationChanged += GUIOrientationManger_OnDeviceOrientationChanged;
        }

		ResetLayouter ();
    }


    protected virtual void Start()
    {
		if ((rotationType != GUILayouterRotationType.Both) && IsRootLayouter)
		{
			gameObject.SetActive(rotationType == GUIOrientationManger.Instance.CurrentRotationType);
		}

		Initialize();
    }


	protected virtual void OnDestroy()
	{
		if (IsRootLayouter)
		{
			GUIOrientationManger.OnDeviceOrientationChanged -= GUIOrientationManger_OnDeviceOrientationChanged;
		}
	}

    protected virtual void OnEnable()
    {
        if (isDirty)
        {
            StartCoroutine(CoroutineInitialize());
            isDirty = false;
        }
    }

	#endregion

    #region Event Handlers

    void GUIOrientationManger_OnDeviceOrientationChanged (GUILayouterRotationType rt)
    {
        if (this.IsNull())
        {
            GUIOrientationManger.OnDeviceOrientationChanged -= GUIOrientationManger_OnDeviceOrientationChanged;
            return;
        }

        if ((rotationType != GUILayouterRotationType.Both))
		{
            bool isActive = (rotationType == rt);

            gameObject.SetActive(isActive);
		}

        ResetLayouter();

        if (isActiveAndEnabled)
        {
            StartCoroutine(CoroutineInitialize());
        }
        else
        {
            isDirty = true;
        }
    }


    IEnumerator CoroutineInitialize()
    {
        yield return new WaitForEndOfFrame();
        Initialize();
    }

    #endregion

    #region ILayoutCellHandler implementation

    public void RepositionForCell(LayoutCellInfo info)
    {        
        if (info.type == GUILayouterType.Horizontal)
        {
            OccupiedPixels = info.cellRect;
        }
        else if (info.type == GUILayouterType.Vertical)
        {
            OccupiedPixels = info.cellRect;
        }

        if (info.anchor == AnchorType.Left)
        {
            CachedTransform.localPosition = new Vector3(-info.cellRect.width * 0.5f, 0, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Right)
        {
            CachedTransform.localPosition = new Vector3(info.cellRect.width * 0.5f, 0, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Top)
        {
            CachedTransform.localPosition = new Vector3(0, info.cellRect.height * 0.5f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Bottom)
        {
            CachedTransform.localPosition = new Vector3(0, -info.cellRect.height * 0.5f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Center)
        {
            CachedTransform.localPosition = new Vector3(0, 0, CachedTransform.localPosition.z);;
        }
    }

    #endregion

	#region Public methods

	public void ResetLayouter()
	{
		occupiedPixels = null;
		cells = null;
	}

	public void Initialize()
	{
		if (occupiedPixels == null)
		{            
            int screenWidth = ScreenDimentions.Width;
            int screenHeight = ScreenDimentions.Height;

            int width = screenWidth;
            int height = screenHeight;
            bool isLandscapeOrientation;

            if (rotationType == GUILayouterRotationType.Both)
            {
                #if UNITY_EDITOR
                isLandscapeOrientation = (width > height);
                #else
                if ((Screen.autorotateToLandscapeLeft) ||
                    (Screen.autorotateToLandscapeRight) ||
                    (Screen.autorotateToPortrait) ||
                    (Screen.autorotateToPortraitUpsideDown))
                {
                    DeviceOrientation deviceOrientation = Input.deviceOrientation;

                    if ((deviceOrientation == DeviceOrientation.LandscapeLeft && Screen.autorotateToLandscapeLeft) ||
                        (deviceOrientation == DeviceOrientation.LandscapeRight && Screen.autorotateToLandscapeRight))
                    {
                        isLandscapeOrientation = true;
                    }
                    else if ((deviceOrientation == DeviceOrientation.Portrait && Screen.autorotateToPortrait) || 
                             (deviceOrientation == DeviceOrientation.PortraitUpsideDown && Screen.autorotateToPortraitUpsideDown))
                    {
                        isLandscapeOrientation = false;
                    }
                    else
                    {
                        isLandscapeOrientation = (width > height);
                    }
                }
                else
                {
                    ScreenOrientation screenOrientation = Screen.orientation;

                    if (screenOrientation == ScreenOrientation.LandscapeLeft ||
                        screenOrientation == ScreenOrientation.LandscapeRight ||
                        screenOrientation == ScreenOrientation.Landscape)
                    {
                        isLandscapeOrientation = true;
                    }
                    else if (screenOrientation == ScreenOrientation.Portrait ||
                             screenOrientation == ScreenOrientation.PortraitUpsideDown)
                    {
                        isLandscapeOrientation = false;
                    }
                    else
                    {
                        isLandscapeOrientation = (width > height);
                    }
                }
                #endif
            }
            else
            {
                isLandscapeOrientation = (rotationType == GUILayouterRotationType.Landscape);
            }

            width = isLandscapeOrientation ? Mathf.Max(screenWidth, screenHeight) : Mathf.Min(screenWidth, screenHeight);
            height = isLandscapeOrientation ? Mathf.Min(screenWidth, screenHeight) : Mathf.Max(screenWidth, screenHeight);

            OccupiedPixels = new Rect?(new Rect(0, 0, width, height));
		}
	}

    #if UNITY_EDITOR
    public void UpdateLayoutDebug(Vector3 debugScreenSize)
    {
        occupiedPixels = new Rect?(new Rect(0, 0, debugScreenSize.x, debugScreenSize.y));

        cells = GetComponentsInChildren<GUILayoutCell>(true);

        List<GUILayoutCell> myCells = new List<GUILayoutCell>();

        foreach (var cell in cells)
        {
            if (cell.transform.parent == CachedTransform)
            {
                myCells.Add(cell);
            }
        }

        cells = myCells.ToArray();

        UpdateLayout();
    }
    #endif

    public void UpdateLayout()
    {
        if(cells == null || cells.Length == 0 || cells[0] == null)
		{
			return;
		}

        int occupiedFixedSize = 0;
        float flexibleAreasTotalWeight = 0;

        bool hasFlexibleAreas = false;

        for (int i = 0; i < cells.Length; i++)
        {
            GUILayoutCell cell = cells[i];
            if (cell.Type == GUILayoutCellType.FixedSize)
            {
                occupiedFixedSize += (int)cell.SizeValue;
            }
            else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
            {
                occupiedFixedSize += (int)(cell.SizeValue * (type == GUILayouterType.Horizontal ? occupiedPixels.GetValueOrDefault().width : occupiedPixels.GetValueOrDefault().height));
            }
            else if (cell.Type == GUILayoutCellType.Flexible)
            {
                hasFlexibleAreas = true;
                flexibleAreasTotalWeight += cell.SizeValue;
            }
        }

        int availiblePixels = (int)(type == GUILayouterType.Horizontal ? occupiedPixels.GetValueOrDefault().width : occupiedPixels.GetValueOrDefault().height);

        int unoccupiedArea = availiblePixels - occupiedFixedSize;

        if (unoccupiedArea < 0)
        {
//            CustomDebug.LogWarning(gameObject.name + ": Can't fit " + occupiedFixedSize + " pixels in " + occupiedPixels + " pixels");
        }

        if (unoccupiedArea > 0 && flexibleAreasTotalWeight < float.Epsilon && hasFlexibleAreas)
        {
            CustomDebug.LogError(gameObject.name + ": Weights for flexible cells is 0!");
        }


        //setting cells sizes
        int currentPosition = isInversed ? availiblePixels : 0;

        for (int i = 0; i < cells.Length; i++)
        {
            GUILayoutCell cell = cells[i];
            Rect cellRect = new Rect();
            if (type == GUILayouterType.Horizontal)
            {
                if (cell.Type == GUILayoutCellType.FixedSize)
                {
                    cellRect = new Rect(0, 0, cell.SizeValue, occupiedPixels.GetValueOrDefault().height);
                }
                else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
                {
                    cellRect = new Rect(0, 0, cell.SizeValue * (type == GUILayouterType.Horizontal ? ScreenDimentions.Width : ScreenDimentions.Height), occupiedPixels.GetValueOrDefault().height);
                }
                else if (cell.Type == GUILayoutCellType.Flexible)
                {
                    cellRect = new Rect(0, 0, cell.SizeValue * (float)unoccupiedArea / (float)flexibleAreasTotalWeight, occupiedPixels.GetValueOrDefault().height);
                }
                if (isInversed)
                {
                    cellRect.center = new Vector3(currentPosition - cellRect.width * 0.5f, 0, 0);
                    currentPosition -= (int)cellRect.width;
                }
                else
                {
                    cellRect.center = new Vector3(currentPosition + cellRect.width * 0.5f, 0, 0);
                    currentPosition += (int)cellRect.width;
                }
            }
            else if (type == GUILayouterType.Vertical)
            {
                if (cell.Type == GUILayoutCellType.FixedSize)
                {
                    cellRect = new Rect(0, cell.SizeValue * 0.5f, occupiedPixels.GetValueOrDefault().width, cell.SizeValue);
                }
                else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
                {
                    cellRect = new Rect(0, 0, occupiedPixels.GetValueOrDefault().width, cell.SizeValue * (type == GUILayouterType.Horizontal ? ScreenDimentions.Width : ScreenDimentions.Height));
                }
                else if (cell.Type == GUILayoutCellType.Flexible)
                {
                    cellRect = new Rect(0, 0, occupiedPixels.GetValueOrDefault().width, cell.SizeValue * (float)unoccupiedArea / (float)flexibleAreasTotalWeight);
                }
                if (isInversed)
                {
                    cellRect.center = new Vector3(0, currentPosition - cellRect.height * 0.5f, 0);
                    currentPosition -= (int)cellRect.height;
                }
                else
                {
                    cellRect.center = new Vector3(0, currentPosition + cellRect.height * 0.5f, 0);
                    currentPosition += (int)cellRect.height;
                }
            }
            cell.Reposition(type, cellRect);
        }
    }


	#endregion

}
