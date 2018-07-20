using UnityEngine;

[RequireComponent(typeof(tk2dUIItem))]
public class GUIButtonStateSwitcher : MonoBehaviour, ILayoutCellHandler
{
    #region Variables

	public enum ButtonState
	{
		Released,
		Pressed
	}

    [SerializeField] tk2dBaseSprite buttonSprite;

    [SerializeField] string normalSpriteName;
    [SerializeField] string pressedSpriteName;
	[SerializeField] string blinkSpriteName;
	[SerializeField] float blinkSpeed = 0.5f;
    [SerializeField] Transform contentRoot;
    [SerializeField] float pressedOffset;

	Vector2 initialDimensions;
	Vector3 initialLocalPosition = Vector3.zero;
    float initialY;

	ButtonState CurrentState = ButtonState.Released;
	float currentBlinkTime = 0;

    tk2dUIItem uiitem;
    tk2dUIItem UIItem 
    {
        get
        {
            if (uiitem == null)
            {
                uiitem = GetComponent<tk2dUIItem>();
            }

            return uiitem;
        }
    }

    public string NormalSpriteName
    {
        get
        {
            return normalSpriteName;
        }
        set
        {
            normalSpriteName = value;
            buttonSprite.SetSprite(normalSpriteName);
        }
    }

    public string PressedSpriteName
    {
        get
        {
            return pressedSpriteName;
        }
        set
        {
            pressedSpriteName = value;
        }
    }

	bool IsSlicedSprite
	{
		get
		{
			return buttonSprite is tk2dSlicedSprite;
		}
	}

	tk2dSlicedSprite SlicedSprite
	{
		get
		{
			return buttonSprite as tk2dSlicedSprite;
		}
	}

    #endregion

    #region Unity Lifecycle

	void Awake()
	{
		if(tk2dSystem.IsRetina)
		{
			pressedOffset = pressedOffset * 2;
		}
	}

	void Update()
	{
		if(currentBlinkTime >= blinkSpeed)
		{
			if(CurrentState != ButtonState.Pressed && !string.IsNullOrEmpty(blinkSpriteName))
			{
				buttonSprite.SetSprite(buttonSprite.CurrentSprite.name.Equals(normalSpriteName) ? blinkSpriteName : normalSpriteName);
				currentBlinkTime = 0;
			}
		}
		else
		{
			currentBlinkTime += Time.deltaTime;
		}
	}

    void OnEnable()
    {
        UIItem.OnDown += UIItem_OnDown;
        UIItem.OnRelease += UIItem_OnRelease;

        buttonSprite.SetSprite(normalSpriteName);		

        if (contentRoot != null)
        {
            initialY = contentRoot.localPosition.y;
        }
    }

    void OnDisable()
    {
        UIItem.OnDown -= UIItem_OnDown;
        UIItem.OnRelease -= UIItem_OnRelease;

        if (contentRoot != null)
        {            
            contentRoot.SetLocalPositionY(initialY);
        }

		if(IsSlicedSprite)
		{
			SlicedSprite.dimensions = initialDimensions;

			SlicedSprite.transform.localPosition = initialLocalPosition;
		}
        CurrentState = ButtonState.Released;
    }

    #endregion

    #region Private Methods

    #region ILayoutCellHandler implementation

    public void RepositionForCell(LayoutCellInfo info)
    {
        if (IsSlicedSprite)
        {
            initialDimensions = SlicedSprite.dimensions;
        }
    }

    #endregion

    void UIItem_OnRelease ()
    {        
        buttonSprite.SetSprite(normalSpriteName);
		CurrentState = ButtonState.Released;

        if (contentRoot != null)
        {            
			contentRoot.SetLocalPositionY(initialY);
        }

		if(IsSlicedSprite)
		{
			SlicedSprite.dimensions = initialDimensions;
			
			SlicedSprite.transform.localPosition = initialLocalPosition;
		}

    }

    void UIItem_OnDown ()
    {
        buttonSprite.SetSprite(pressedSpriteName);
		CurrentState = ButtonState.Pressed;

		if (contentRoot != null)
		{
			initialY = contentRoot.localPosition.y;
			
			contentRoot.SetLocalPositionY(contentRoot.localPosition.y + (IsSlicedSprite ? Mathf.FloorToInt(pressedOffset/2) + Mathf.FloorToInt(pressedOffset/4) : pressedOffset));
		}

		if(IsSlicedSprite)
		{
			initialDimensions = SlicedSprite.dimensions;
			
			SlicedSprite.dimensions = new Vector2(SlicedSprite.dimensions.x, SlicedSprite.dimensions.y + pressedOffset);
			
			SlicedSprite.transform.SetLocalPositionY(SlicedSprite.transform.localPosition.y + Mathf.FloorToInt(pressedOffset/2));
		}
    }

    #endregion

}
