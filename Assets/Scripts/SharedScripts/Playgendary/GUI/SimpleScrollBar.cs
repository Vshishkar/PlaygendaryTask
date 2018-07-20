using UnityEngine;
using System.Collections;

public class SimpleScrollBar : MonoBehaviour, ILayoutCellHandler
{
    [SerializeField] tk2dUIScrollView scrollView;
	
    [SerializeField] Transform beginTransform;
    [SerializeField] Transform endTransform;

    [SerializeField] tk2dBaseSprite barSprite;

    [SerializeField] bool isInversed = false;

    Transform cachedTransform;
    Transform CachedTransform
    {
        get
        {
            if (cachedTransform == null)
            {
                cachedTransform = transform;
            }

            return cachedTransform;
        }
    }

    Vector3 directionVector = Vector3.right;
    float scrollLength;
    float barHalfSize;

    #region ILayoutCellHandler implementation

    public void RepositionForCell(LayoutCellInfo info)
    {
        directionVector = endTransform.position - beginTransform.position;
        scrollLength = directionVector.magnitude - barSprite.GetBounds().size.x * barSprite.transform.lossyScale.x;
        directionVector.Normalize();

        barHalfSize = barSprite.GetBounds().size.x * 0.5f * barSprite.transform.lossyScale.x;
        ScrollView_OnContentPositionChanged();
    }

    #endregion

    void OnEnable()
    {
        scrollView.OnContentPositionChange += ScrollView_OnContentPositionChanged;
    }

    void OnDisable()
    {
        scrollView.OnContentPositionChange -= ScrollView_OnContentPositionChanged;
    }


    void ScrollView_OnContentPositionChanged()
    {
        float offset = 0;

        if (scrollView.scrollDirection == ScrollDirection.Horizontal)
        {
            offset = -scrollView.contentContainer.localPosition.x * (isInversed ? -1 : 1);
        }
        else if (scrollView.scrollDirection == ScrollDirection.Vertical)
        {
            offset = -scrollView.contentContainer.localPosition.y * (isInversed ? -1 : 1);
        }

        float factor = Mathf.Clamp01(offset / scrollView.contentLength);

        CachedTransform.position = beginTransform.position + directionVector * (barHalfSize + factor * scrollLength);
    }
}
