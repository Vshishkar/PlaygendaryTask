using UnityEngine;
using System.Collections.Generic;

public class CustomGUI : SingletonMonoBehaviour<CustomGUI> 
{
    public static event System.Action OnDebugGUI
    {
        add 
        { 
            if (CustomGUI.Instance)
            {
                onDebugGUI += value; 
            }
        }
        remove { onDebugGUI -= value; }
    }

	#region Variables

    static bool needShowDebugGUI;
    static bool needShowDebugSwitch;
    static System.Action onDebugGUI;

    GUIDebug guiDebug;

	#endregion


    #region Properties

    static bool NeedShowDebugGUI
    {
        get { return (CustomDebug.Enable && needShowDebugGUI); }
    }   

    #endregion


	#region Unity lifecycle

    protected override void Awake()
    {
        base.Awake();
        guiDebug = gameObject.AddComponent<GUIDebug>();
        guiDebug.enabled = needShowDebugGUI;
    }


    void Update()
    {
        if (CustomDebug.Enable)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 4)
                {
                    if (!needShowDebugSwitch)
                    {
                        needShowDebugSwitch = true;
                        needShowDebugGUI = !needShowDebugGUI;
                    }
                }
                else
                {
                    if (needShowDebugSwitch)
                    {
                        needShowDebugSwitch = false;
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (!needShowDebugSwitch)
                    {
                        needShowDebugSwitch = true;
                        needShowDebugGUI = !needShowDebugGUI;
                    }
                }
                else
                {
                    if (needShowDebugSwitch)
                    {
                        needShowDebugSwitch = false;
                    }
                }
            }

            if (guiDebug)
            {
                guiDebug.enabled = NeedShowDebugGUI;
            }
        }
    }
	
	#endregion


    #region OnGUI

    class GUIDebug : MonoBehaviour
    {
        
        void OnGUI ()
        {
            if (NeedShowDebugGUI)
            {
                if (onDebugGUI != null)
                {
                    onDebugGUI();
                }
            }
        }
    }        

    #endregion
}