using UnityEngine;

public static class GameObjectAnimationExtension {

	public static void SpeedFX(this GameObject go, float speed) {
		var animations = go.GetComponentsInChildren<Animation>();
		foreach (var animation in animations) {
			foreach (AnimationState state in animation) {
                state.speed = speed;
            }
        }
		var particleSystems = go.GetComponentsInChildren<ParticleSystem>();
		foreach (var particleSystem in particleSystems) {
            #if UNITY_5_5_OR_NEWER
            var main = particleSystem.main;
            main.simulationSpeed = speed;
            #else
            particleSystem.playbackSpeed = speed;
            #endif
		}
    }

	public static void ResetAnimationToStart(this GameObject go) {
		var animations = go.GetComponentsInChildren<Animation>();
		foreach (Animation animation in animations) {
			animation.Play();
			animation.Rewind();
			animation.Sample();
			animation.Stop();
		}
	}

	public static void ResetFX(this GameObject go) {
		var particleSystems = go.GetComponentsInChildren<ParticleSystem>();
		foreach (var particleSystem in particleSystems) {
			particleSystem.Clear();
			particleSystem.Play();
		}
		var animations = go.GetComponentsInChildren<Animation>();
		foreach (var animation in animations) {
			animation.Stop();
			animation.Rewind();
			animation.Play();
		}
	}

	public static void StopFX(this GameObject go, bool cleanup) {
		var particleSystems = go.GetComponentsInChildren<ParticleSystem>();
		foreach (var particleSystem in particleSystems) {
			particleSystem.Stop();
			if (cleanup) {
				particleSystem.Clear(true);
			}
		}
		var animations = go.GetComponentsInChildren<Animation>();
		foreach (var animation in animations) {
			animation.Stop();
		}
	}

	public static void PlayFX(this GameObject go) {
		var particleSystems = go.GetComponentsInChildren<ParticleSystem>();
		foreach (var particleSystem in particleSystems) {
			particleSystem.Play();
		}
		var animations = go.GetComponentsInChildren<Animation>();
		foreach (var animation in animations) {
			animation.Play();
		}
	}
}
