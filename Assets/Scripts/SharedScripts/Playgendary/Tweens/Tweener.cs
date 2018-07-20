using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Method 
{
	Linear,
	EaseIn,
	EaseOut,
	EaseInOut
}

public enum Style 
{
	Once,
	Loop,
	PingPong,
}

public delegate void TweenCallback(ITweener tw);
public delegate bool WaitAction();

public interface ITweenerMini 
{
    bool IsBeginStateSet { get; }
    bool IsEndStateSet { get; }
	void SetBeginState(float delay = 0f, TweenCallback del = null);
	void SetEndState(float delay = 0f, TweenCallback del = null);
	void SetBeginStateImmediately();
	void SetEndStateImmediately();
}

public interface ITweener : ITweenerMini 
{	
	void AddOnFinishedDelegate(TweenCallback del);
	void RemoveOnFinishedDelegate(TweenCallback del);
	void SetOnFinishedDelegate(TweenCallback del);
	string TargetName { get; }
	bool IsRun { get; }
}


public struct TimedCallback
{
	public TweenCallback callback;
	public float time;

	public TimedCallback(TweenCallback _callback, float _time) : this()
	{
		this.callback = _callback;
		this.time = _time;
	}
}


public abstract class Tweener : IgnoreTimeScale, ITweener 
{	
    const float TWEEN_FACTOR_EPSILON = 0.001f;

    #region Variables
	public Method method = Method.Linear;
	public Style style = Style.Once;
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public bool useCurve;
	public bool ignoreTimeScale = true;
	public float duration = 1f;
	public float scaleDuration = 1f;
	public bool steeperCurves = true;
	public InvokeData invokeWhenFinished = new InvokeData();
	
    public bool shouldSetBeginStateAtStart = false;

    [SerializeField] string _animationCurveId = "default";
	[SerializeField] bool notUsedForScreenAnimatingCondition;
	float cachedDuration = -1f;
	float cachedScaleDuration = -1f;
	float amountPerDelta = 1f;
	float factor;
	TweenCallback OnFinished;

    List<KeyValuePair<string, TimedCallback>> timedCallbacks = new List<KeyValuePair<string, TimedCallback>>();

	Transform cachedTransform;
	bool started;
	float startTime;
	float delay;
    bool isReversed = false;

	WaitAction WaitAndDoCallback;

	public float AmountPerDelta 
    {
		get 
        {
			if ((cachedDuration != duration) || (cachedScaleDuration != scaleDuration))
            {
				cachedDuration = duration;
				cachedScaleDuration = scaleDuration;
				float realDuration = duration * scaleDuration;
				amountPerDelta = (realDuration > 0f) ? (1f / realDuration) : 10000f;
			}

            return amountPerDelta * (isReversed ? -1 : 1);
		}
	}
	
    #if UNITY_EDITOR
	public string AnimationCurveId 
    {
		get { return _animationCurveId; }
		set
        {
            AnimationCurve newAnimationCurve = TweensCurves.Instance.GetCurve(value);

			if (newAnimationCurve != null)
            {
				animationCurve = newAnimationCurve;
				_animationCurveId = value;
			}
		}
	}
    #endif
	
	public float Factor 
    {
		get { return factor; }
	}

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

    public float Delay 
    {
        set 
        {
            delay = value;
            started = false;
        }
    }

    public virtual string TargetName 
    { 
        get { return gameObject.name; } 
    }


	public bool NotUsedForScreenAnimatingCondition 
	{ 
		get 
		{ 
			return notUsedForScreenAnimatingCondition; 
		}
		set
		{
			notUsedForScreenAnimatingCondition = value;
		}
	}

    #endregion


    #region Unity Lifecycle

    protected virtual void Awake()
    {
        if (shouldSetBeginStateAtStart)
        {
            SetBeginStateImmediately();
        }

        enabled = false;
    }
	
	public virtual void Update() 
    {
		float delta = ignoreTimeScale ? 
            #if UNITY_5_5_OR_NEWER
            Time.unscaledDeltaTime
            #else
            UpdateRealDeltaTime() 
            #endif
            : Time.deltaTime;
		float time = ignoreTimeScale ? 
            #if UNITY_5_5_OR_NEWER
            Time.unscaledTime
            #else
            RealTime 
            #endif
            : Time.time;

		if (!started) 
        {
			started = true;
			startTime = time + delay;
		}

		if (time < startTime) 
        {
			return;
		}

        float oldFactor = factor;
		factor += AmountPerDelta * delta;
	
        if (timedCallbacks.Count > 0)
        {
            for (int i = 0; i < timedCallbacks.Count; i++)
            {
                KeyValuePair<string, TimedCallback> pair = timedCallbacks[i];
                TimedCallback timedCallback = pair.Value;
                if (timedCallback.callback != null && oldFactor < timedCallback.time && factor > timedCallback.time)
                {
                    timedCallback.callback(this);
                }
            }
        }

		if (style == Style.Loop) 
        {
			if (factor > 1f)
            {
				factor -= Mathf.Floor(factor);
			}
			else if (factor < 0f)
			{
				factor = 1f - factor;
			}

		}
        else if (style == Style.PingPong) 
        {
			if (factor > 1f) 
            {
				factor = 1f - (factor - Mathf.Floor(factor));
				amountPerDelta = -AmountPerDelta;
			} 
            else if (factor < 0f)
            {
				factor = -factor;
				factor -= Mathf.Floor(factor);
				amountPerDelta = -AmountPerDelta;
			}
		}

		if ((style == Style.Once) && ((factor > 1f) || (factor < 0f))) 
        {
            factor = Mathf.Clamp01(factor);

			Sample(factor, true);

            enabled = false;

			if (OnFinished != null)
            {
				OnFinished(this);
			}

			invokeWhenFinished.Call();		
		} 
        else
        {
			Sample(factor, false);
		}
	}
	
    #endregion


    #region Public methods
	public void Sample(float value, bool isFinished) 
    {
        value = Mathf.Clamp01(value);

		switch (method)
        {
            case Method.EaseIn:
                value = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - value));

                if (steeperCurves) 
                {
                    value *= value;
                }
                break;

            case Method.EaseOut:
                value = Mathf.Sin(0.5f * Mathf.PI * value);

                if (steeperCurves)
                {
                    value = 1f - value;
                    value = 1f - value * value;
                }

                break;

            case Method.EaseInOut:
                const float pi2 = Mathf.PI * 2f;
                value = value - Mathf.Sin(value * pi2) / pi2;

                if (steeperCurves) 
                {
                    value = value * 2f - 1f;
                    float sign = (value < 0f) ? -1f : 1f;
                    value = 1f - sign * value;
                    value = 1f - value * value;
                    value = sign * value * 0.5f + 0.5f;
                }

                break;
		}
		if (Application.isPlaying) 
        {
			TweenUpdateRuntime(((useCurve && (animationCurve != null)) ? animationCurve.Evaluate(value) : value), isFinished);
		} 
        else 
        {
			TweenUpdateEditor((useCurve && (animationCurve != null)) ? animationCurve.Evaluate(value) : value);
		}
	}
	

	public void SwitchOnCurve() 
    {
		useCurve = true;
	}
	

	public void SwitchOffCurve()
    {
		useCurve = false;
	}
	

	public void Play(bool forward) 
    {
        IsReversed = !forward;
		enabled = true;
	}
	
    public bool IsReversed
    {
        get { return isReversed; }
        set { isReversed = value; }
    }

	public void Reverse()
    {
        isReversed = !isReversed;
	}


	public void ResetScaleDuration() 
    {
		scaleDuration = 1f;
	}

	public void ResetTween() 
    {
		ResetTween(false);
	}


	protected void ResetTween(bool isFinished) 
    {
        factor = IsReversed ? 1f : 0f;
		Sample(factor, isFinished);
	}


    public virtual bool IsBeginStateSet 
    { 
        get { return !enabled && factor < TWEEN_FACTOR_EPSILON; }
    }


    public virtual bool IsEndStateSet 
    { 
        get { return !enabled && factor > 1 - TWEEN_FACTOR_EPSILON; }
    }

    public void SetBeginStateDefault() //signature for animation event
    {
        SetBeginState();
    }

    public virtual void SetBeginState()
    {
        SetBeginState(0);
    }

    public virtual void SetBeginState(float delay)
    {
        SetBeginState(delay, null);
    }

    public virtual void SetBeginState(float delay, TweenCallback del)
    {
        if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
        {
            SetBeginStateImmediately();
            if (del != null)
            {
                del(this);
            }
        }
        else
        {
            Delay = delay;
            SetOnFinishedDelegate(del);
            Play(false);
        }
    }

    public void SetEndStateDefault()//signature for animation event
    {
        SetEndState();
    }

    public virtual void SetEndState(float delay = 0f, TweenCallback del = null)
    {
        if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
        {
            SetEndStateImmediately();
            if (del != null)
            {
                del(this);
            }
        }
        else
        {
            Delay = delay;

            SetOnFinishedDelegate(del);

            Play(true);
        }
    }


    public virtual void SetBeginStateImmediately()
    {
        factor = 0;
        Sample(0, true);
    }


    public virtual void SetEndStateImmediately()
    {
        factor = 1;
        Sample(1, true);
    }


	public void AddTimedCallback(string key, float time, TweenCallback call)
	{
		if(!string.IsNullOrEmpty(key))
		{
			AddTimedCallback(key, new TimedCallback(call, time));
		}
	}


	public void AddTimedCallback(string key, TimedCallback timedCallback)
	{
        if (string.IsNullOrEmpty(key) || timedCallback.callback == null)
        {
            return;
        }

        for (int i = 0; i < timedCallbacks.Count; i++)
        {
            var cb = timedCallbacks[i];
            if (cb.Key.Equals(key))
            {
                return;
            }
        }

        timedCallbacks.Add(new KeyValuePair<string, TimedCallback>(key, timedCallback));
		
	}


	public void RemoveTimedCallback(string key)
	{
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        for (int i = 0; i < timedCallbacks.Count; i++)
        {
            var cb = timedCallbacks[i];
            if (cb.Key.Equals(key))
            {
                timedCallbacks.Remove(cb);
                break;
            }
        }	
	}


	public void ClearAllTimedCallbacks()
	{
        timedCallbacks.Clear();
	}


	// support old version!!!!!
	public void SetTimedCallback(float time, TweenCallback call)
	{
		const string oldVersionDefaultKey = "oldVersionDefaultKey";
		if(call != null)
		{
            for (int i = 0; i < timedCallbacks.Count; i++)
            {
                var cb = timedCallbacks[i];
                if (cb.Key.Equals(oldVersionDefaultKey))
                {
                    timedCallbacks.Add(new KeyValuePair<string, TimedCallback>(oldVersionDefaultKey, new TimedCallback(call, time)));
                    break;
                }
			}
		}
		else 
		{
            for (int i = 0; i < timedCallbacks.Count; i++)
            {
                var cb = timedCallbacks[i];
                if (cb.Key.Equals(oldVersionDefaultKey))
                {
                    timedCallbacks.Remove(cb);
                    break;
                }
            }
		}
	}


	public void AddOnFinishedDelegate(TweenCallback del) { if (del != null) { OnFinished += del; } }
	public void RemoveOnFinishedDelegate(TweenCallback del) { if (del != null) { OnFinished -= del; } }
	public void SetOnFinishedDelegate(TweenCallback del) { OnFinished = del; }

	public bool IsRun 
    {
		get { return enabled; }
	}

	static public T InitGO<T>(GameObject go) where T : Tweener
    {
		T tw = go.GetComponent<T>() ?? go.AddComponent<T>();
		tw.factor = 0f;
		return tw;
	}

	static public T InitGO<T>(GameObject go, float duration) where T : Tweener 
    {
		T tw = go.GetComponent<T>() ?? go.AddComponent<T>();
		tw.duration = duration;
		tw.factor = 0f;
		return tw;
	}
    #endregion


    #region Private methods

    abstract protected void TweenUpdateRuntime(float factor, bool isFinished);

    protected virtual void TweenUpdateEditor(float factor)
    {
        TweenUpdateRuntime(factor, false);
    }


    IEnumerator WaitAndDo(float wait) 
    {
        float start = Time.time;
        while ((Time.time - start) < wait) 
        {
            yield return null;
        }

        if (WaitAndDoCallback != null) 
        {
            WaitAndDoCallback();
        }
    }


    protected void RunWaitTimer(float waitTime, WaitAction todo) 
    {
        WaitAndDoCallback = todo;
        StartCoroutine("WaitAndDo", waitTime);
    }


    protected void ResetWaitTimer() 
    {
        WaitAndDoCallback = null;
        StopCoroutine("WaitAndDo");
    }

    #endregion
}