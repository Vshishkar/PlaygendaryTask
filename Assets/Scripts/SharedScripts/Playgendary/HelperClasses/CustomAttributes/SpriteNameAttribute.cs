using UnityEngine;
using System.Collections;

public class SpriteNameAttribute : PropertyAttribute
{
    protected string targetTK2DSprite;

    public string TargetTK2DSprite
    {
        get
        {
            return targetTK2DSprite;
        }
    }

    // or spritecollection reference
    public SpriteNameAttribute(string TK2DBaseSpriteReference)
    {
        targetTK2DSprite = TK2DBaseSpriteReference;
    }
}
