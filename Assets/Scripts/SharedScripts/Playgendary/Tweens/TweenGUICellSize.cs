using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TweenGUICellSize : Tweener 
{
    #region Variables

    public float beginSize;
    public float endSize;

    [SerializeField] GUILayoutCell target;
    [SerializeField] GUILayouter rootLayouter;
    [SerializeField] GUILayouter targetCellLayouter;

    #endregion


    #region Properties

    public GUILayoutCell Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            targetCellLayouter = null;
            rootLayouter = null;

            InitializeLayouters();
        }
    }

    #endregion


    #region Unity lifecycle

    protected override void Awake()
    {
        base.Awake();

        InitializeLayouters();
    }

    #endregion


    #region Protected methods

    protected override void TweenUpdateRuntime(float factor, bool isFinished)
    {
        if (target != null)
        {
            target.SizeValue = beginSize + (endSize - beginSize) * factor;
        }

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (rootLayouter != null)
            {
                rootLayouter.UpdateLayoutDebug(new Vector3(ScreenDimentions.Width, ScreenDimentions.Height));
            }
        }
        else
        #endif
        {
            if (targetCellLayouter != null)
            {
                targetCellLayouter.UpdateLayout();
            }
        }
    }

    #endregion


    #region Private methods

    void InitializeLayouters()
    {
        if (target != null)
        {
            Transform targetParent = target.CachedTransform.parent;
            if (targetParent != null)
            {
                if (targetCellLayouter == null)
                {
                    targetCellLayouter = targetParent.GetComponent<GUILayouter>();
                }

                if (rootLayouter == null)
                {
                    GUILayouter[] layouters = target.GetComponentsInParent<GUILayouter>();
                    for (int i = 0; i < layouters.Length; i++)
                    {
                        if (layouters[i].IsRootLayouter)
                        {
                            rootLayouter = layouters[i];
                            break;
                        }
                    }
                }
            }
        }
    }

    #endregion
}
