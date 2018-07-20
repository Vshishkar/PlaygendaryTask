using UnityEngine;

public abstract class IgnoreTimeScale : MonoBehaviour {
	
	float realTime;
	float lastRealTime;
	float timeDelta;
	float actual;
	bool timeStarted;
	
	protected float RealTime {
		get { return realTime; }
	}
	
	protected virtual void OnEnable() {
		timeStarted = true;
		timeDelta = 0f;
		lastRealTime = Time.realtimeSinceStartup;
	}

	protected float UpdateRealDeltaTime() {
		realTime = Time.realtimeSinceStartup;
		if (timeStarted) {
			float delta = realTime - lastRealTime;
			actual += (delta < 0f) ? 0f : delta;
			timeDelta = 0.00001f * (int) (actual * 100000f);
			actual -= timeDelta;
			if (timeDelta > 1f) {
				timeDelta = 1f;
			}
		} else {
			timeStarted = true;
			timeDelta = 0f;
		}
		lastRealTime = realTime;
		return timeDelta;
	}
}