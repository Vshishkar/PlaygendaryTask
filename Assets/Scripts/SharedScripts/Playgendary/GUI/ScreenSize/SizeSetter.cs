using UnityEngine;

public abstract class SizeSetter : MonoBehaviour {

	public bool destroyAfterInit;
	public RoundFloatEnum roundFloatPreference = RoundFloatEnum.DontRoundFloat;

    bool start = false;

    void Awake() {
		if (!start) {
			start = true;
			UpdateSize();
			if (destroyAfterInit) {
				Destroy(this);
			}
		}
	}

	public void Init() {
        Awake();
	}

	public void ForceInit() {
		UpdateSize();
		start = true;
	}

	protected abstract void UpdateSize();
}
