using UnityEngine;

public enum ScrollDirection {

	Horizontal = 0,
	Vertical = 1,
	ByZ = 2
}

public class tk2dUIScrollView : MonoBehaviour 
{
	[SerializeField] bool isInverse;
	[SerializeField] bool isMapScroll;
	[SerializeField] float speedCoof;
	public float contentLength = 1f;
	public float leftLimitFactor = 1f;
	public float rightLimitFactor = 1f;
	public Transform contentContainer;
    [SerializeField] protected tk2dUIItem backgroundBtn;
	public ScrollDirection scrollDirection = ScrollDirection.Vertical;
	public bool momentum = true;
	public bool snap = false;
	public bool snapFirstLast = false;
    public int pageSize = -1;
	public float scrollSpeed = 40f;
	public float smoothTime = 0.1f;
	public bool limitedMaxVelocity = false;
	public float maxVelocity = 50f;
	
	public float SWIPE_THRESHOLD = 0.02f;
	public float INPUT_CHANGE_APPLY_FACTOR = 0.5f;
	public float SWIPE_VELOCITY_FACTOR = 1f;
	public float TIME_FACTOR_FOR_MOVE_CONTENT_INSIDE_LIMITS = 5f;
	public float TIME_FACTOR_FOR_MOVE_CONTENT_OUTSIDE_LIMITS = 30f;
	public float VELOCITY_FOR_END_MOVE_CONTENT_INSIDE_LIMITS = 1.0f;
	public float VELOCITY_FOR_END_MOVE_CONTENT_OUTSIDE_LIMITS = 1.0f;
	public float VELOCITY_FOR_END_MOVE_CONTENT_TO_ONE_OF_LIMITS = 5.0f;

	public System.Action OnContentPositionChange;
	public System.Action<Transform> OnStartSnap;
	public System.Action<Transform> OnFinishedSnap;

	protected bool isDown, resetChild, isInProgress, isSnap;

    protected float swipeVelocity = 0f;
    protected float snapToLimitsVelocity = 0f;

    protected float startTouchValue = 0f;
    protected float previousTouchWorldValue = 0f;
    protected float cachedTouchWorldValue = 0f;
    protected float beginContentOffsetValue = 0f;
    protected float downOffsetValue = 0f;

    protected float fromSnapValue, toSnapValue;
    protected float startSnapVelocity, currentSnapVelocity;
    protected Transform snapItem = null;

	Transform cachedTransform;
	Transform CachedTransform 
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

    public bool BackgroundBtnEnabled
    {
        get
        {
            return backgroundBtn;
        }
        set
        {
            backgroundBtn.enabled = value;

            BoxCollider collider = backgroundBtn.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = backgroundBtn.enabled;
            }
        }
    }

    public bool IsInverse
    {
        get { return isInverse; }
    }

	public void ScrollTo(Transform scrollToItem, float velocity = 20f) 
    {
		snapItem = scrollToItem;
		velocity = Abs(velocity);
		if (velocity < 1f) 
        {
			velocity = 1;
		}
		startSnapVelocity = currentSnapVelocity = velocity;
		fromSnapValue = ContentOffsetValue;
		toSnapValue = -scrollToItem.localPosition[AxisZ];
		swipeVelocity = 0f;
		snapToLimitsVelocity = 0f;
		isDown = isInProgress = false;
		isSnap = true;
		if (OnStartSnap != null) 
        {
			OnStartSnap(snapItem);
		}
	}

	public void ResetContentPosition() 
    {
		ContentOffsetValue = 0f;
	}

    public void ContentPositionChangedManually()
    {
        FireOnContentPositionChangedEvent();
    }

	public int Axis 
    {
		get { return (int) scrollDirection; }
	}

	public int AxisZ
	{
		get
		{
			return scrollDirection == ScrollDirection.ByZ ? 1 : Axis;
		}
	}

	protected virtual void OnEnable() 
    {
		if (backgroundBtn != null) 
        {
			backgroundBtn.OnDownUIItem += Down;
			backgroundBtn.OnUpUIItem += Up;
		}
		if (tk2dUIManager.Instance != null) {
			tk2dUIManager.Instance.OnInputUpdate += OverUpdate;
		}
	}

    protected virtual void OnDisable() 
    {
		if (backgroundBtn != null) {
			backgroundBtn.OnDownUIItem -= Down;
            backgroundBtn.OnUpUIItem -= Up;
		}
        if (tk2dUIManager.Instance__NoCreate != null) 
        {
            tk2dUIManager.Instance.OnInputUpdate -= OverUpdate;
		}
		isDown = false;
		isInProgress = false;
		swipeVelocity = 0f;
		snapToLimitsVelocity = 0f;
	}

    protected virtual void Down(tk2dUIItem btn) 
    {
        if (contentLength > float.Epsilon)
        {
            startTouchValue = btn.Touch.position[AxisZ];
            cachedTouchWorldValue = previousTouchWorldValue = CurrentTouchWorldPosition(btn)[AxisZ];
            downOffsetValue = beginContentOffsetValue = ContentOffsetValue;
            swipeVelocity = 0f;
            snapToLimitsVelocity = 0f;
            resetChild = isInProgress = false;
            isDown = true;
        }
	}

    protected virtual void Up(tk2dUIItem btn)
    {
        if (!isDown)
        {
            return;
        }

		isDown = false;
		isInProgress = true;
		swipeVelocity *= SWIPE_VELOCITY_FACTOR;
		float swipeDistance = btn.Touch.position[AxisZ] - startTouchValue;

        if (Abs(swipeDistance) > SwipeThreshold())
        {
            if (snap)
            {
                float min = (scrollDirection == ScrollDirection.Horizontal) ? -contentLength : 0f;
                float max = (scrollDirection == ScrollDirection.Horizontal) ? 0f : contentLength;
                float destination = CalculateDestinationValue(swipeVelocity, min, max);

                snapItem = GetChildForSnap(-destination);

                if (!OutsideLimits(min, max, destination) || snapFirstLast)
                {
                    swipeVelocity = Abs(swipeVelocity);
                    if (swipeVelocity < 1f)
                    {
                        swipeVelocity = 1;
                    }
                    startSnapVelocity = currentSnapVelocity = swipeVelocity;
                    fromSnapValue = ContentOffsetValue;
                    toSnapValue = -snapItem.localPosition[AxisZ];
                    swipeVelocity = 0f;
                    snapToLimitsVelocity = 0f;
                    isInProgress = false;
                    isSnap = true;

                    if (OutsideLimits(min, max, destination) && snapFirstLast)
                    {
                        if (destination < min)
                        {
                            startSnapVelocity = currentSnapVelocity = Mathf.Abs(destination - min);
                        }
                        else if (destination > max)
                        {
                            startSnapVelocity = currentSnapVelocity = Mathf.Abs(destination - max);
                        }
                    }

                }
                if (OnStartSnap != null)
                {
                    OnStartSnap(snapItem);
                }
            }
        }
        else
        {
            snapItem = GetChildForSnap(0);

			if (OnFinishedSnap != null) 
			{
				OnFinishedSnap (null);
			}
        }
	}

	// call after Down or Up
    protected virtual void OverUpdate()
    {
		float destination = ContentOffsetValue;
		bool change = false;
		float min = (scrollDirection == ScrollDirection.Horizontal) ? -contentLength : 0f;
		float max = (scrollDirection == ScrollDirection.Horizontal) ? 0f : contentLength;
        if (isDown)
        {
            destination = AddChangeInputValue(destination);
            change = isInProgress;
            if (isInProgress)
            {
                if (OutsideLimits(min, destination, destination))
                {
                    destination -= ((destination - min) / leftLimitFactor) / 2f;
                    change = true;
                    swipeVelocity = 0;
                    if (destination >= min)
                    {
                        destination = min;
                    }
                }
                else if (OutsideLimits(destination, max, destination))
                {
                    destination -= ((destination - max) / rightLimitFactor) / 2f;
                    change = true;
                    swipeVelocity = 0;
                }
            }
        }
        else if (isSnap)
        {
            currentSnapVelocity = LerpToZero(currentSnapVelocity, (tk2dUITime.deltaTime * TIME_FACTOR_FOR_MOVE_CONTENT_INSIDE_LIMITS));
            if (Abs(currentSnapVelocity) < VELOCITY_FOR_END_MOVE_CONTENT_INSIDE_LIMITS)
            {
                currentSnapVelocity = 0f;
            }
            float factor = 1f - currentSnapVelocity / startSnapVelocity;
            destination = Lerp(fromSnapValue, toSnapValue, factor);
            change = true;
            if (factor >= 1f)
            {
                isSnap = false;
                swipeVelocity = 0f;
                isInProgress = false;
                if (OnFinishedSnap != null)
                {
                    OnFinishedSnap(snapItem);
                }
            }
        }
        else if (isInProgress)
        {
            if (!OutsideLimits(min, max, destination))
            {
                // step 1: move content inside content length
                destination += swipeVelocity * tk2dUITime.deltaTime * scrollSpeed;
                change = true;
                swipeVelocity = LerpToZero(swipeVelocity, (tk2dUITime.deltaTime * TIME_FACTOR_FOR_MOVE_CONTENT_INSIDE_LIMITS));

                if (Abs(swipeVelocity) < VELOCITY_FOR_END_MOVE_CONTENT_INSIDE_LIMITS)
                {
                    swipeVelocity = 0f;
                    isInProgress = false;
                }
            }
            else if (Abs(swipeVelocity) > 0f)
            {
                // step 2: fast move content outside content length until swipeCurrentVelocity != 0
                destination += swipeVelocity * tk2dUITime.deltaTime * scrollSpeed;
                change = true;
                swipeVelocity = LerpToZero(swipeVelocity, (tk2dUITime.deltaTime * TIME_FACTOR_FOR_MOVE_CONTENT_OUTSIDE_LIMITS));
                if (Abs(swipeVelocity) < VELOCITY_FOR_END_MOVE_CONTENT_OUTSIDE_LIMITS)
                {
                    swipeVelocity = 0f;
                }
            }
            else
            {
                // step 3: fast move content to min/max limit
                float approvedDestination = (destination < min) ? min : max;
                destination = Mathf.SmoothDamp(destination, approvedDestination, ref snapToLimitsVelocity, smoothTime, Mathf.Infinity, tk2dUITime.deltaTime);
                change = true;
                if (Abs(snapToLimitsVelocity) < VELOCITY_FOR_END_MOVE_CONTENT_TO_ONE_OF_LIMITS)
                {
                    destination = approvedDestination;
                    snapToLimitsVelocity = 0f;
                    isInProgress = false;
                    if (snap && !snapFirstLast)
                    {
                        if (OnFinishedSnap != null)
                        {
                            OnFinishedSnap(snapItem);
                        }
                    }
                }
            }
        }
       
		if (change) 
        {
			ContentOffsetValue = destination;
		}
	}

    protected Vector3 CurrentTouchWorldPosition(tk2dUIItem btn) 
    {
		Vector3 touchPosition = btn.Touch.position;
		var uiCamera = tk2dUIManager.Instance.GetUICameraForControl(gameObject);
		touchPosition.z = btn.transform.position.z - uiCamera.transform.position.z;
		var worldPosition = uiCamera.ScreenToWorldPoint(touchPosition);
		worldPosition.z = btn.transform.position.z;
		return CachedTransform.InverseTransformPoint(worldPosition);
	}

    protected bool OutsideLimits(float min, float max, float value) 
    {
		return (value < min) || (value > max);
	}

	float Clamp01(float value) 
    {
		return (value < 0f) ? 0f : ((value > 1f) ? 1f : value);
	}

	float Abs(float value) 
    {
		return (value < 0f) ? -value : value;
	}

	float LerpToZero(float from, float factor) 
    {
		return from * (1f - Clamp01(factor));
	}

	float Lerp(float from, float to, float factor) 
    {
		factor = Clamp01(factor);
		return from * (1f - factor) + to * factor;
	}

	float AddChangeInputValue(float source) 
    {
		if (momentum) 
        {
			float currentTouchWorldValue = CurrentTouchWorldPosition(backgroundBtn)[AxisZ];
			swipeVelocity = currentTouchWorldValue - previousTouchWorldValue;
			if (swipeVelocity == 0f) 
            {
				swipeVelocity = currentTouchWorldValue - cachedTouchWorldValue;
			}

			if (limitedMaxVelocity && (Abs(swipeVelocity) > maxVelocity))
            {
				swipeVelocity = (swipeVelocity < 0f) ? -maxVelocity : maxVelocity;
			}

			if (previousTouchWorldValue != currentTouchWorldValue) 
            {
				cachedTouchWorldValue = previousTouchWorldValue;
			}

			previousTouchWorldValue = currentTouchWorldValue;
		}

		float swipeDistance = backgroundBtn.Touch.position[AxisZ] - startTouchValue;

        if (!resetChild && (Abs(swipeDistance) > SwipeThreshold()))
        {
			tk2dUIManager.Instance.OverrideClearAllChildrenPresses(backgroundBtn);
			resetChild = true;
		}

		swipeDistance = isMapScroll ? swipeDistance * speedCoof : swipeDistance;

		if (!isInProgress && (Abs(swipeDistance) > SWIPE_THRESHOLD)) 
        {
			isInProgress = true;
			beginContentOffsetValue -= swipeDistance;
		}

		if (isInProgress)
        {
			source = Lerp(source, (beginContentOffsetValue + swipeDistance), INPUT_CHANGE_APPLY_FACTOR);
		}

		return source;
	}

    protected float CalculateDestinationValue(float velocity, float min, float max) 
    {
		float destination = ContentOffsetValue;
		float deltaTime = tk2dUITime.deltaTime;
		while (true) 
        {
			bool isDestinationOutside = OutsideLimits(min, max, destination);
			destination += velocity * deltaTime * scrollSpeed;
			velocity = LerpToZero(velocity, (deltaTime * (isDestinationOutside ? TIME_FACTOR_FOR_MOVE_CONTENT_OUTSIDE_LIMITS : TIME_FACTOR_FOR_MOVE_CONTENT_INSIDE_LIMITS)));
			if (Abs(velocity) < (isDestinationOutside ? VELOCITY_FOR_END_MOVE_CONTENT_OUTSIDE_LIMITS : VELOCITY_FOR_END_MOVE_CONTENT_INSIDE_LIMITS)) 
            {
				break;
			}

            if (pageSize > 0 && (Mathf.Abs(destination - downOffsetValue) > pageSize || isDestinationOutside))
            {
                destination -= velocity * deltaTime * scrollSpeed;
                break;
            }
		}
		return destination;
	}


    protected virtual float SwipeThreshold()
    {
        return tk2dUIManager.Instance.SwipeThreshold;
    }


	protected virtual Transform GetChildForSnap(float destination) 
    {
		Transform result = null;
		float distanceToResult = 0f;
		foreach (Transform child in contentContainer) 
        {
			if (child.gameObject.activeSelf)
            {
				float distanceToChild = Abs(child.localPosition[AxisZ] - destination);
				if ((result == null) || (distanceToResult > distanceToChild)) 
                {
					distanceToResult = distanceToChild;
					result = child;
				}
			}
		}
		return result;
	}


    protected virtual void FireOnContentPositionChangedEvent()
    {
        if (OnContentPositionChange != null) 
        {
            OnContentPositionChange();
        }
    }


    public void Stop()
    {
        isInProgress = false;
        isDown = false;
        isSnap = false;
    }

    protected float ContentOffsetValue 
    {
		get 
		{ 
			if(isInverse)
			{
				return -contentContainer.localPosition[Axis]; 
			}

			return contentContainer.localPosition[Axis]; 
		}
		set
        {
			var newPosition = contentContainer.localPosition;

			newPosition[Axis] = isInverse ?  -value : value;

			contentContainer.localPosition = newPosition;

            FireOnContentPositionChangedEvent();
		}
	}
}
