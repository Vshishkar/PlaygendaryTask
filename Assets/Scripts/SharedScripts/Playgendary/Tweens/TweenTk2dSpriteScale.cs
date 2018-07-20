using UnityEngine;
 
[RequireComponent(typeof(tk2dSprite))]
public class TweenTk2dSpriteScale : Tweener 
{
	#region Variables   

    tk2dSprite sprite;

    [SerializeField] Vector3 endScale;
    [SerializeField] Vector3 beginScale;

    public Vector3 EndScale
    {       
        get { return endScale; }
        set { endScale = value; }
    }

    public Vector3 BeginScale
    {
        get { return beginScale; }
        set { beginScale = value; }
    }

    tk2dSprite Sprite
    {
        get
        {
            if (sprite == null)
            {
                sprite = GetComponent<tk2dSprite>();
            }
            return sprite;
        }
    }

	#endregion 
     
	#region Private   

    protected override void TweenUpdateRuntime(float factor, bool isFinished)
    {
        Sprite.scale = BeginScale + (EndScale - BeginScale) * factor;
    }

    protected override void TweenUpdateEditor(float factor)
    {
        Sprite.scale = BeginScale + (EndScale - BeginScale) * factor;
    }

	#endregion
}