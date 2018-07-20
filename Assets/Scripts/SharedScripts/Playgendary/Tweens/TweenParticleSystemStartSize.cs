using UnityEngine;

[AddComponentMenu("Inventain/Tween/ParticleSystemStartSize")]
public class TweenParticleSystemStartSize : Tweener 
{
	#region Variables   

	[SerializeField] float endSize = 1f;
	[SerializeField] float beginSize = 0f;
	ParticleSystem targetParticleSystem;


	public float EndSize
	{       
		get { return endSize; }
		set { endSize = value; }
	}


	public float BeginSize
	{
		get { return beginSize; }
		set { beginSize = value; }
	}


	public ParticleSystem TargetParticleSystem
	{
		get
		{
			if (targetParticleSystem == null)
			{
				targetParticleSystem = GetComponent<ParticleSystem>();
			}
			return targetParticleSystem;
		}
	}


	public float CurrentSize
	{
		get 
        {
            #if UNITY_5_5_OR_NEWER
            return TargetParticleSystem.main.startSizeMultiplier;
            #else
            return TargetParticleSystem.startSize; 
            #endif
        }
		set 
        { 
            #if UNITY_5_5_OR_NEWER
            var main = TargetParticleSystem.main;
            main.startSizeMultiplier = value;
            #else
            TargetParticleSystem.startSize = value; 
            #endif
        }
	}

	#endregion 


	#region Public methods

	public static TweenParticleSystemStartSize SetSize(GameObject go, float size, float duration = 1f) 
	{
		TweenParticleSystemStartSize twps = Tweener.InitGO<TweenParticleSystemStartSize>(go);
		twps.BeginSize = twps.CurrentSize;
		twps.EndSize = size;
		twps.duration = duration;
		twps.Play(true);

		return twps;
	}      

	#endregion

	
	#region Private methods
	
	protected override void TweenUpdateRuntime(float factor, bool isFinished)
	{
		CurrentSize = BeginSize + (EndSize - BeginSize) * factor;
	}


	protected override void TweenUpdateEditor(float factor)
	{
		CurrentSize = BeginSize + (EndSize - BeginSize) * factor;
	}
	
	#endregion
}