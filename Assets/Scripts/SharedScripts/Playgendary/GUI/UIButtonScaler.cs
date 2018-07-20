using UnityEngine;
using System.Collections;

public class UIButtonScaler : MonoBehaviour {

	public tk2dUIItem myItem;
	public float downDuration = 0.1f;
	public float upDuration = 0.5f;
	public Vector3 downScale = new Vector3(0.9f, 0.9f, 0.9f);
	public Vector3 upScale = Vector3.one;
	public bool cachedUpScaleOnAwake = true;
	public Method downMethod = Method.Linear;
	public bool useDownCurve = true;
	public AnimationCurve downCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public Method upMethod = Method.Linear;
    public bool useUpCurve = true;
	public AnimationCurve upCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	public bool useOnReleaseInsteadOfOnUp;
    public bool isIgnoringTimeScale = false;
    public bool isScaleBoxCollider = false;
	
	Transform cachedTransform;
    BoxCollider cachedBoxCollider;
    Vector3 cachedBoxColliderOriginSize;
	Vector3 toScale = Vector3.one;
	Vector3 fromScale = Vector3.one;
	float elapsedTime;

	void Awake() {
		if (cachedUpScaleOnAwake) {
			var setter = GetComponent<ScaleSetter>();
			if (setter != null) {
				setter.Init();
			}
            upScale = LocalScale;
		}
    }

	void OnEnable() {
		if (MyItem != null) {
			MyItem.OnDown += ButtonDown;
			if (useOnReleaseInsteadOfOnUp) {
				MyItem.OnRelease += ButtonUp;
			} else {
				MyItem.OnUp += ButtonUp;
            }
            
        }
		elapsedTime = 0f;
        LocalScale = upScale;
    }

    void OnDisable() {
		if (MyItem != null) {
			MyItem.OnDown -= ButtonDown;
			if (useOnReleaseInsteadOfOnUp) {
				MyItem.OnRelease -= ButtonUp;
			} else {
				MyItem.OnUp -= ButtonUp;
            }
        }
    }

	void ButtonDown() {
		StopCoroutine("UpScaleTween");
		if (downDuration > 0f) {
			toScale = downScale;
            fromScale = LocalScale;
			StartCoroutine("DownScaleTween");
		} else {
            LocalScale = downScale;
		}
    }

	void ButtonUp() {
		StopCoroutine("DownScaleTween");
		if (upDuration > 0f) {
			toScale = upScale;
            fromScale = LocalScale;
			StartCoroutine("UpScaleTween");
		} else {
            LocalScale = upScale;
		}
	}

	float ApplyMethod(float value, Method method) {
		if (value < 0f) {
			value = 0f;
		}
		if (value > 1f) {
			value = 1f;
		}
		switch (method) {
		case Method.EaseIn:
			value = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - value));
			value *= value;
			break;
		case Method.EaseOut:
			value = Mathf.Sin(0.5f * Mathf.PI * value);
			value = 1f - value;
			value = 1f - value * value;
			break;
		case Method.EaseInOut:
			const float pi2 = Mathf.PI * 2f;
			value = value - Mathf.Sin(value * pi2) / pi2;
			value = value * 2f - 1f;
			float sign = (value < 0f) ? -1f : 1f;
			value = 1f - sign * value;
			value = 1f - value * value;
			value = sign * value * 0.5f + 0.5f;
			break;
		}
		return value;
	}

	IEnumerator DownScaleTween() {
		elapsedTime = 0f;
		while (elapsedTime < downDuration) {
			float percent = elapsedTime / downDuration;
			percent = (percent > 1f) ? 1f : percent;
			percent = ApplyMethod(percent, downMethod);
			percent = (useDownCurve && (downCurve != null)) ? downCurve.Evaluate(percent) : percent;
            LocalScale = fromScale * (1f - percent) + toScale * percent;
			yield return null;
            float deltaTime = isIgnoringTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += deltaTime;

		}
        LocalScale = toScale;
	}

	IEnumerator UpScaleTween() {
		elapsedTime = 0f;
		while (elapsedTime < upDuration) {
			float percent = elapsedTime / upDuration;
			percent = (percent > 1f) ? 1f : percent;
			percent = ApplyMethod(percent, upMethod);
			percent = (useUpCurve && (upCurve != null)) ? upCurve.Evaluate(percent) : percent;
            LocalScale = fromScale * (1f - percent) + toScale * percent;
            yield return null;
            float deltaTime = isIgnoringTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += deltaTime;
        }
        LocalScale = toScale;
    }
	

	public bool UseOnReleaseInsteadOfOnUp {
		get { return useOnReleaseInsteadOfOnUp; }
	}


    public bool IsIgnoringTimeScale {
        get { return isIgnoringTimeScale; }
    }


	public Transform CachedTransform {
		get {
			if (cachedTransform == null) {
				cachedTransform = transform;
			}
			return cachedTransform;
		}
	}

	public tk2dUIItem MyItem {
		get {
			if (myItem == null) {
				myItem = GetComponent<tk2dUIItem>();
			}
			return myItem;
		}
	}


    public BoxCollider CachedBoxCollider
    {
        get
        {
            if (cachedBoxCollider == null)
            {
                cachedBoxCollider = GetComponent<BoxCollider>();

                if (cachedBoxCollider != null)
                {
                    cachedBoxColliderOriginSize = cachedBoxCollider.size;

                    if (tk2dSystem.IsRetina)
                    {
                        cachedBoxColliderOriginSize *= 2;
                    }
                }
            }

            return cachedBoxCollider;
        }
    }


    public Vector3 LocalScale
    {
        get
        {
            return CachedTransform.localScale;
        }
        set
        {
            CachedTransform.localScale = value;

            if (!isScaleBoxCollider && CachedBoxCollider != null)
            {
                float coeff = (value.x == 0f) ? (1f) : (Mathf.Abs(value.x));
                CachedBoxCollider.size = cachedBoxColliderOriginSize / coeff;
            }
        }
    }


	void Reset()
	{
		downDuration = 0.15f;
		upDuration = 0.7f;
		downScale = new Vector3(0.85f, 0.85f, 1f);
		upScale = Vector3.one;
		cachedUpScaleOnAwake = false;
		downMethod = Method.EaseOut;
		useDownCurve = false;
		downCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
		upMethod = Method.EaseOut;
		useUpCurve = true;

        #if UNITY_EDITOR
        upCurve = TweensCurves.Instance.GetCurve("OnUPBounceForButtonScaler");
        #else
        upCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        #endif

		useOnReleaseInsteadOfOnUp = false;
	}
}
