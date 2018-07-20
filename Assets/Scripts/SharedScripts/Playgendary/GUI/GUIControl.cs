using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GUIControl : MonoBehaviour 
{
    [System.Serializable]
    public abstract class ShowableObject
    {
        public float showDelay;
        public float showDelayVariance;
        public float hideDelay;
        public float hideDelayVariance;
    }

    [System.Serializable]
    public class TweenData : ShowableObject
    {
        public Tweener tween;
        public float showDurationScale = 1;
        public float hideDurationScale = 1;
    }

    [System.Serializable]
    public class ControlData : ShowableObject
    {
        public GUIControl control;
    }

	public struct ActionWithInvokes
	{
		public System.Action<GUIControl> onFinished;
		public bool callInvokes;

		public ActionWithInvokes(System.Action<GUIControl> action, bool call)
		{
			onFinished = action;
			callInvokes = call;
		}
	}

	#region Variables

    [HideInInspector][SerializeField] List<TweenData> tweensData = new List<TweenData>();
    [HideInInspector][SerializeField] List<ControlData> controlsData = new List<ControlData>();
	[SerializeField] bool notUsedForScreenAnimatingCondition;
    [SerializeField] bool isAnimatingIgnoreTimeScale;
    [SerializeField] GameObject[] deactivateOnHideObjects;

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


	public bool NotUsedForScreenAnimatingCondition 
	{ 
		get { return notUsedForScreenAnimatingCondition; }
		set { notUsedForScreenAnimatingCondition = value; }
	}
		

    public List<TweenData> TweensData
    {
        get { return tweensData; }
    }


    public List<ControlData> ControlsData
    {
        get { return controlsData; }
    }


    public bool IsShown
    {
        get 
        {
            for (int i = 0; i < tweensData.Count; i++)
            {
                TweenData t = tweensData[i];

				if ((t.tween != null) &&
					(!t.tween.NotUsedForScreenAnimatingCondition) && 
					(!t.tween.IsEndStateSet || t.tween.enabled))
                {
                    return false;
                }
            }

            for (int i = 0; i < controlsData.Count; i++)
            {
                ControlData c = controlsData[i];

				if ((c.control != null) && 
					(!c.control.NotUsedForScreenAnimatingCondition) && 
					(!c.control.IsShown))
                {
                    return false;
                }
            }                
           
            return true; 
        }
    }

    public bool IsHidden
    {
        get 
        {
			for (int i = 0, n = tweensData.Count; i < n; i++)
            {
                TweenData t = tweensData[i];

				if ((t.tween != null) && 
					(!t.tween.NotUsedForScreenAnimatingCondition) && 
					(!t.tween.IsBeginStateSet || t.tween.enabled))
                {
                    return false;
                }
            }

			for (int i = 0, n = controlsData.Count; i < n; i++)
            {
                ControlData c = controlsData[i];

				if ((c.control != null) && 
					(!c.control.NotUsedForScreenAnimatingCondition) && 
					(!c.control.IsHidden))
                {
                    return false;
                }
            }                

            return true; 
        }
    }

    public bool IsAnimating
    {
        get { return !IsHidden && !IsShown; }
    }

    public float ShowDuration
    {
        get
        {
            float maxDuration = 0;

			for (int i = 0, n = tweensData.Count; i < n; i++)
			{
				TweenData t = tweensData[i];

				if (t != null)
				{
					if ((t.showDelay + t.tween.duration) * t.showDurationScale > maxDuration)
					{
						maxDuration = t.showDelay + t.tween.duration * t.showDurationScale;
					}
				}
			}

			for (int i = 0, n = controlsData.Count; i < n; i++)
			{
				ControlData c = controlsData[i];

				if (c != null)
				{
					if ((c.showDelay + c.control.ShowDuration) > maxDuration)
					{
						maxDuration = c.showDelay + c.control.ShowDuration;
					}
				}
			}

            return maxDuration;
        }
    }

    public float HideDuration
    {
        get
        {
            float maxDuration = 0;

			for (int i = 0, n = tweensData.Count; i < n; i++)
			{
				TweenData t = tweensData[i];

				if (t != null)
				{
					if ((t.hideDelay + t.tween.duration) * t.showDurationScale > maxDuration)
					{
						maxDuration = t.hideDelay + t.tween.duration * t.showDurationScale;
					}
				}
			}

			for (int i = 0, n = controlsData.Count; i < n; i++)
			{
				ControlData c = controlsData[i];

				if (c != null)
				{
					if (c.control != null && (c.hideDelay + c.control.HideDuration) > maxDuration)
					{
						maxDuration = c.hideDelay + c.control.HideDuration;
					}
				}
			}

            return maxDuration;
        }
    }

    public void ActivateControl()
    {
        gameObject.SetActive(true);

		for (int i = 0, n = tweensData.Count; i < n; i++)
		{
			TweenData td = tweensData[i];

			if (td != null)
			{
				td.tween.gameObject.SetActive(true);
			}
        }

		for (int i = 0, n = controlsData.Count; i < n; i++)
		{
			ControlData cd = controlsData[i];

			if (cd != null)
			{
				cd.control.ActivateControl();
			}
        }
    }

    public void DeactivateControl()
    {
        gameObject.SetActive(false);

		for (int i = 0, n = tweensData.Count; i < n; i++)
		{
			TweenData td = tweensData[i];

			if (td != null)
			{
				td.tween.gameObject.SetActive(false);
			}
        }

		for (int i = 0, n = controlsData.Count; i < n; i++)
		{
			ControlData cd = controlsData[i];

			if (cd != null)
			{
				cd.control.DeactivateControl();
			}
        }
    }

	#endregion
 

	#region Public

    public virtual void Show()
    {
        Show(null);
    }

    public virtual void ShowImmediately()
    {
        Show(null, true);
    }

    public virtual void Show(System.Action<GUIControl> onFinished)
    {
        Show(onFinished, false);
    }

	public virtual void Show(System.Action<GUIControl> onFinished, bool immediately, bool callInvokes = true)
    {
        Show(onFinished, immediately, 0f, callInvokes);
    }

	public virtual void Show(System.Action<GUIControl> onFinished, bool immediately, float delay, bool callInvokes)
    {
        gameObject.SetActive(true);

		for (int i = 0, n = deactivateOnHideObjects.Length; i < n; i++)
		{
			GameObject go = deactivateOnHideObjects[i];

			if (go != null)
			{
				go.SetActive(true);
			}
		}

        if (!gameObject.activeInHierarchy && !immediately)
        {
			Show(onFinished, true, delay, callInvokes);
            return;
        }

		ShowTweenData(immediately, delay);

		for (int i = 0, n = controlsData.Count; i < n; i++)
		{
			ControlData cd = controlsData[i];

			if (cd != null)
			{
				cd.control.Show(null, immediately, cd.showDelay + Random.Range(-cd.showDelayVariance * 0.5f, cd.showDelayVariance * 0.5f) + delay, callInvokes);
			}
        }

		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(StartAnimation(new ActionWithInvokes(onFinished, callInvokes)));
		}
    }

    public virtual void Hide()
    {
        Hide(null);
    }

    public virtual void HideImmediately()
    {
        Hide(null, true);
    }

    public virtual void Hide(System.Action<GUIControl> onFinished)
    {
        Hide(onFinished, false);
    }

	public virtual void Hide(System.Action<GUIControl> onFinished, bool immediately, bool callInvokes = true)
    {
        Hide(onFinished, immediately, 0f, callInvokes);
    }

	public virtual void Hide(System.Action<GUIControl> onFinished, bool immediately, float delay, bool callInvokes)
    {
        if (!gameObject.activeInHierarchy && !immediately)
        {
			Hide(onFinished, true, delay,callInvokes);
            return;
        }

		HideControls(immediately, delay, callInvokes);
       
        if (immediately)
        {
            DeactivateObjectsToHide();
            
            if (onFinished != null)
            {
                onFinished(this);
            }
        }
        else
        {          
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(StartAnimation(new ActionWithInvokes(onFinished, callInvokes)));
			}
        }
    }


	public virtual void HideControls()
	{
		HideControls(false, 0f, true);
	}


	public virtual void HideControls(bool immediately, float delay, bool callInvokes)
	{
		HideTweenData(immediately, delay);

		for (int i = 0, n = controlsData.Count; i < n; i++)
		{
			ControlData c = controlsData[i];

			if (c != null)
			{
				if (c.control == null)
				{
					CustomDebug.LogError(gameObject.name + ": Child control on GUIControl component not set!");
				}
				else
				{
					c.control.Hide(null, immediately, c.hideDelay + Random.Range(-c.showDelayVariance * 0.5f, c.showDelayVariance * 0.5f) + delay, callInvokes);
				}
			}
		}
	}

    void DeactivateObjectsToHide()
    {

		for (int i = 0, n = deactivateOnHideObjects.Length; i < n; i++)
		{
			GameObject go = deactivateOnHideObjects[i];

			if (go != null)
			{
				go.SetActive(false);
			}
		}
    }

	protected virtual IEnumerator StartAnimation(ActionWithInvokes actionWithInvokes)
    {
        while (IsAnimating)
        {
            if (!isAnimatingIgnoreTimeScale)
            {
                yield return new WaitForSeconds(0.1f);           
            }
            else
            {
                #if UNITY_5_5_OR_NEWER
                yield return new WaitForSecondsRealtime(0.1f);
                #else
                yield return StartCoroutine(WaitForRealTimeSeconds(0.1f));
                #endif
            }
        }

		if (actionWithInvokes.onFinished != null)
        {
			actionWithInvokes.onFinished(this);
        }        

        if (IsHidden)   
        {
            DeactivateObjectsToHide();

			if ((CachedTransform.localScale.x < 0.01f) || (CachedTransform.localScale.y < 0.01f))
            {
                gameObject.SetActive(false);
            }
        }	
    }


    protected virtual IEnumerator WaitForRealTimeSeconds(float time)
    {
        while (true)
        {
            float endTime = Time.realtimeSinceStartup + time;
            yield return new WaitWhile(() => { return (Time.realtimeSinceStartup < endTime); });
            yield break;
        }
    }
    #endregion


	#region Private methods

	protected virtual void ShowTweenData(bool immediately, float delay)
	{
		for (int i = 0, n = tweensData.Count; i < n; i++)
		{
			TweenData t = tweensData[i];

			if (t != null)
			{
				if (immediately)
				{
					t.tween.SetEndStateImmediately();
					t.tween.enabled = false;
				}
				else
				{
					t.tween.scaleDuration = t.showDurationScale;
					t.tween.SetBeginStateImmediately();
					t.tween.SetEndState(t.showDelay + Random.Range(-t.showDelayVariance * 0.5f, t.showDelayVariance * 0.5f) + delay);
				}
			}
		}
	}


	protected virtual void HideTweenData(bool immediately, float delay)
	{
		for (int i = 0, n = tweensData.Count; i < n; i++)
		{
			TweenData t = tweensData[i];

			if (t != null)
			{
				if (t.tween == null)
				{
					CustomDebug.LogError(gameObject.name + ": Tween on GUIControl component not set!", this);
				}
				else
				{
					if (immediately)
					{
						t.tween.SetBeginStateImmediately();
						t.tween.enabled = false;
					}
					else
					{
						t.tween.scaleDuration = t.hideDurationScale;
						t.tween.SetBeginState(t.hideDelay + Random.Range(-t.showDelayVariance * 0.5f, t.showDelayVariance * 0.5f) + delay);
					}
				}
			}
		}
	}

	#endregion
}