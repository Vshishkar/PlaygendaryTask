using UnityEngine;

[AddComponentMenu("Inventain/Tween/TweenTextureOffset")]
public class TweenTextureOffset : Tweener 
{	
	public Vector2 beginOffset = Vector2.zero;
	public Vector2 endOffset = Vector2.one;
	
	
	ColorQuad colorQuad;
	GradientQuad gadientQuad;	

	void InitReference(bool force) 
    {
		if (force || ((colorQuad == null) && (gadientQuad == null))) 
        {
            colorQuad = gameObject.GetComponent<ColorQuad>();
            gadientQuad = gameObject.GetComponent<GradientQuad>();
		}
	}

	override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        CurrentOffset = Vector2.Lerp(beginOffset, endOffset, factor);
    }

	public Vector2 CurrentOffset 
    {
		get 
        {
			InitReference(false);
			if (colorQuad != null) 
            {
				return colorQuad.Offset;
			}
			if (gadientQuad != null) 
            {
				return gadientQuad.Offset;
			}

			return -1f * Vector2.one;
		}
		set {
			InitReference(false);
			if (colorQuad != null) 
            {
				colorQuad.Offset = value;
			} 
            else if (gadientQuad != null) 
            {
				gadientQuad.Offset = value;
			} 
		}
	}
	
	
	static public TweenTextureOffset SetOffset(GameObject go, float duration, Vector2 offset) {
		var twto = Tweener.InitGO<TweenTextureOffset>(go, duration);
        twto.beginOffset = twto.CurrentOffset;
        twto.endOffset = offset;
		return twto;
	}
}