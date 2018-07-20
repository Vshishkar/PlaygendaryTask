using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(GUILayouter))]
public class GUILayoutUpdater : MonoBehaviour 
{
    #if UNITY_EDITOR
    GUILayouter targetLayouter;

    int frameCount;
    int fps = 30;

    public GUILayouter TargetLayouter
    {
        get
        {
            if (targetLayouter == null)
            {
                targetLayouter = GetComponent<GUILayouter>();
            }

            return targetLayouter;
        }
    }    


    void Update()
    {
        if ((TargetLayouter != null) && (TargetLayouter.IsRootLayouter))
        {
            bool isDirty = true;
            if (EditorApplication.isPlaying)
            {
                frameCount++;
                if (frameCount > fps)
                {
                    frameCount = 0;
                }
                else
                {
                    isDirty = false;
                }
            }

            if (isDirty)
            {
                ResetLayouters();

                float width;
                float height;
                float aspect;
                tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);
                TargetLayouter.UpdateLayoutDebug(new Vector2(ScreenDimentions.Width, ScreenDimentions.Height));
            }
        }
    }


    void ResetLayouters()
    {
        if ((TargetLayouter != null) && (TargetLayouter.IsRootLayouter))
        {
            GUILayouter[] childLayouters = GetComponentsInChildren<GUILayouter>();

            foreach (var l in childLayouters)
            {
                l.ResetLayouter();
            }

            TargetLayouter.ResetLayouter();
        }
    }

    [ButtonAttribute][SerializeField] string reset = "Reset";
    void Reset()
    {
        if (!string.IsNullOrEmpty(reset))
        {
            if ((TargetLayouter != null) && (TargetLayouter.IsRootLayouter))
            {
                GUILayouter[] childLayouters = GetComponentsInChildren<GUILayouter>();
                foreach (var l in childLayouters)
                {
                    l.ResetLayouter();
                }
                TargetLayouter.ResetLayouter();

                GUILayoutCell[] cells = GetComponentsInChildren<GUILayoutCell>();
                foreach (var c in cells)
                {
                    c.ResetLayoutHandlers();
                }
            }
        }
    }

    #endif
}
