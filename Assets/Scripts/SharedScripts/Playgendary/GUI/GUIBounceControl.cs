using UnityEngine;
 
[RequireComponent(typeof(TweenScale))]
public class GUIBounceControl : GUIControl 
{
	#region Variables

    TweenScale bounceTween;
    [SerializeField] bool loopBounce;
    [SerializeField] float startDelay;
    [SerializeField] float middleDelay;
    [SerializeField] bool randomMiddleDelay;
    [SerializeField] float randomMiddleDelayPercent;
    [SerializeField] float duration = 0.8f;

    [SerializeField] float bounceSessionTime = -1;
    [SerializeField] float bounceSessionDelay = -1;
    float sessionTime = 0;

    public bool IsActive
    {
        get;
        set;
    }

	#endregion

    #region Unity Lifecicle

    void OnEnable()
    {
        if (IsActive)
        {
            BounceTween.enabled = true;
        }
    }

    void OnDisable()
    {
        BounceTween.enabled = false;
    }


    #endregion
 
	#region Public

    public TweenScale BounceTween
    {
        get
        {
            if (bounceTween == null)
            {
                bounceTween = GetComponent<TweenScale>();
            }

            if (bounceTween == null)
            {
                bounceTween = gameObject.AddComponent<TweenScale>();
            }

            return bounceTween;
        }
    }

    public void Play()
    {
        BounceTween.duration = duration;
        BounceTween.SetBeginStateImmediately();
        BounceTween.SetEndState(startDelay, BounceDone);

        sessionTime = 0;

        IsActive = true;
    }

    public void SetLoop(bool looped = true)
    {
        loopBounce = looped;
    }

    public void Stop(bool immediately = false)
    {
        IsActive = false;

        if (immediately)
        {
            BounceTween.SetOnFinishedDelegate(null);
            BounceTween.SetBeginStateImmediately();
            BounceTween.enabled = false;
        }
    }

	public override void Show(System.Action<GUIControl> onFinished, bool immediately, float delay, bool callInvokes)
    {
		base.Show(onFinished, immediately, delay, callInvokes);

        Play();
    }

	public override void Hide(System.Action<GUIControl> onFinished, bool immediately, float delay, bool callInvokes)
    {
		base.Hide(onFinished, immediately, delay, callInvokes);

        Stop(immediately);
    }

	#endregion
 
 
	#region Private

    void Update()
    {
        if (IsActive)
        {
            sessionTime += Time.deltaTime;
        }
    }

    void BounceDone(ITweener tw) 
    {
        tw.SetOnFinishedDelegate(null);
        if (IsActive) 
        {
            IsActive = loopBounce && gameObject.activeSelf && gameObject.activeInHierarchy;
            if (IsActive) 
            {
                tw.SetBeginStateImmediately();
                float newMiddleDelay = middleDelay + (randomMiddleDelay ? (middleDelay * Random.Range(-randomMiddleDelayPercent, randomMiddleDelayPercent) / 100f) : 0f);

                if (bounceSessionTime > 0 && sessionTime > bounceSessionTime)
                {
                    sessionTime = - bounceSessionDelay;
                    newMiddleDelay += bounceSessionDelay;
                }

                tw.SetEndState(newMiddleDelay, BounceDone);
            }
        }
    }

	#endregion
}