using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Inventain/Tween/Transform")]
public class TweenTransform : Tweener 
{	
    public bool ignoreZ;
    public bool ignoreRotation;
	public Transform beginTransform;
    public Transform endTransform;		
	
	override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        if (ignoreZ)
        {            
            float oldZ = CachedTransform.position.z;
            CachedTransform.position = beginTransform.position * (1f - factor) + endTransform.position * factor;
            CachedTransform.SetGlobalPositionZ(oldZ);
        }
        else
        {
            CachedTransform.position = beginTransform.position * (1f - factor) + endTransform.position * factor;
        }

        if (ignoreRotation)
        {
            CachedTransform.rotation = beginTransform.rotation;

        }
        else
        {
            CachedTransform.rotation = Quaternion.Euler(beginTransform.rotation.eulerAngles * (1f - factor) + endTransform.rotation.eulerAngles * factor);
        }
	}		

    static public TweenTransform SetTransform(GameObject go, Transform transform, float duration = 1f) 
    {
        TweenTransform twt = Tweener.InitGO<TweenTransform>(go, duration);
        twt.beginTransform = twt.CachedTransform;
        twt.endTransform = transform;
        twt.Play(true);
		return twt;
	}
}