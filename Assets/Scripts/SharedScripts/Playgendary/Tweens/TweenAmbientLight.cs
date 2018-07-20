
using UnityEngine;

[AddComponentMenu("Inventain/Tween/TweenAmbientLight")]
public class TweenAmbientLight : Tweener 
{   
    public Color beginColor;
    public Color endColor;      

    override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        RenderSettings.ambientLight = Color.Lerp(beginColor, endColor, factor);
    }       

    static public TweenAmbientLight SetGlobalAmbientColor(GameObject go, Color color, float duration = 1f) 
    {
        var twp = Tweener.InitGO<TweenAmbientLight>(go, duration);
        twp.beginColor = RenderSettings.ambientLight;
        twp.endColor = color;
        twp.Play(true);
        return twp;
    }
}