using UnityEngine;

[AddComponentMenu("Inventain/Tween/Position")]
public class TweenPosition : Tweener 
{	
	public Vector3 beginPosition = Vector3.zero;
	public Vector3 endPosition = Vector3.one;	

    public bool useAbsolutePosValues = false;
    public bool needRecalculatePositions = false;

    protected override void Awake()
    {
        if (!useAbsolutePosValues)
        {
            if (needRecalculatePositions)
            {
                beginPosition = SizeHelper.RecalculatePosition(beginPosition, SizeFactor.TwoFactor);
                endPosition = SizeHelper.RecalculatePosition(endPosition, SizeFactor.TwoFactor);
            }
            else if (tk2dSystem.IsRetina)
            {
                beginPosition = new Vector3(beginPosition.x * 2, beginPosition.y * 2, beginPosition.z);
                endPosition = new Vector3(endPosition.x * 2, endPosition.y * 2, endPosition.z);
            }
        }
        base.Awake();
    }

	override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        CachedTransform.localPosition = beginPosition * (1f - factor) + endPosition * factor;
	}		

    static public TweenPosition SetPosition(GameObject go, Vector3 position, float duration = 1f) 
    {
		var twp = Tweener.InitGO<TweenPosition>(go, duration);
        twp.beginPosition = twp.CachedTransform.localPosition;
        twp.endPosition = position;
        twp.Play(true);
		return twp;
	}
}