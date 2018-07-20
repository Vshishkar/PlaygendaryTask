using UnityEngine;
using System.Collections;

public class tk2dClippedSpriteProgressBar : MonoBehaviour 
{
    #region Variables

    public enum ProgressDirection
    {
        Vertical,
        Horizontal
    }

    [SerializeField] tk2dClippedSprite clippedSprite;

    [SerializeField] bool isInversed;

    [SerializeField] ProgressDirection direction = ProgressDirection.Horizontal;

    [SerializeField] float fillAmount;
    public float FillAmount
    {
        get
        {
            return fillAmount;
        }

        set
        {
            fillAmount = Mathf.Clamp01(value);
            UpdateState();
        }
    }


    #endregion

    #region Private Methods

    void UpdateState()
    {
        if (clippedSprite != null)
        {
            if (direction == ProgressDirection.Horizontal)
            {
                if (isInversed)
                {
                    clippedSprite.ClipRect = new Rect(1, 0, -1 * fillAmount, 1);
                }
                else
                {
                    clippedSprite.ClipRect = new Rect(0, 0, 1 * fillAmount, 1);
                }
            }
            else if (direction == ProgressDirection.Vertical)
            {
                if (isInversed)
                {
                    clippedSprite.ClipRect = new Rect(0, 1, 1, -1 * fillAmount);
                }
                else
                {
                    clippedSprite.ClipRect = new Rect(0, 0, 1, 1 * fillAmount);
                }
            }
        }
    }

    #endregion


    #if UNITY_EDITOR

    void OnValidate()
    {
        UpdateState();
    }

    #endif
	
}
