using UnityEngine;
using System.Collections;

public static class ScreenDimentions
{
    public const int iPhone6Height = 1334;//--> 640x1136
    public const int iPhone6PlusHeight = 1920;//--> 1242x2208
    public const int iPadProHeight = 2732;//--> 1536x2048
    public const int iPhoneXHeight = 2436;//--> 1242x2690


    #if UNITY_ANDROID
    public static Vector2 androidReferenceScreen = new Vector2(2208, 1242);
    public const float androidDownScaleRatio = 1.9f;
    public const float androidDownScaleMultiplier = 1.1f;
    #endif

    public static int Height
    {
        get
        {
            int h = Screen.height;           

            #if UNITY_EDITOR

            float width;
            float height;
            float aspect;

            tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

            h = (int)height;       

            #endif

            #if UNITY_IOS

            if (Screen.height == iPhone6PlusHeight)
            {
                h = 2208;
            }
            else if (Screen.width == iPhone6PlusHeight)
            {
                h = 1242;
            }
            else if (Screen.height == iPadProHeight)
            {
                h = 2048;
            }
            else if (Screen.width == iPadProHeight)
            {
                h = 1536;
            }
            else if (Screen.height == iPhone6Height)
            {
                h = 1136;
            }
            else if (Screen.width == iPhone6Height)
            {
                h = 640;
            }
            else if (Screen.height == iPhoneXHeight)
            {
                h = 2690;
            }
            else if (Screen.width == iPhoneXHeight)
            {
                h = 1242;
            }

            #elif UNITY_ANDROID

            float retinaMultiplier = (tk2dSystem.IsRetina) ? (1f) : (0.5f);
            h = (int)((float)h * AndroidScreenMultiplier * retinaMultiplier);

            #endif

            return h;
        }
    }

    public static int Width
    {
        get
        {
            int w = Screen.width;

            #if UNITY_EDITOR

            float width;
            float height;
            float aspect;

            tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

            w = (int)width;

            #endif

            #if UNITY_IOS

            if (Screen.height == iPhone6PlusHeight)
            {
                w = 1242;
            }
            else if (Screen.width == iPhone6PlusHeight)
            {
                w = 2208;
            }
            else if (Screen.height == iPadProHeight)
            {
                w = 1536;
            }
            else if (Screen.width == iPadProHeight)
            {
                w = 2048;
            }
            else if (Screen.height == iPhone6Height)
            {
                w = 640;
            }
            else if (Screen.width == iPhone6Height)
            {
                w = 1136;
            }
            else if (Screen.height == iPhoneXHeight)
            {
                w = 1242;
            }
            else if (Screen.width == iPhoneXHeight)
            {
                w = 2690;
            }

            #elif UNITY_ANDROID

            float retinaMultiplier = (tk2dSystem.IsRetina) ? (1f) : (0.5f);
            w  = (int)((float)w * AndroidScreenMultiplier * retinaMultiplier);

            #endif

            return w;
        }
    }


    #if UNITY_ANDROID

    public static float AndroidScreenMultiplier
    {
        get
        {
            float minScreenSize = Mathf.Min(Screen.width, Screen.height);
            float maxScreenSize = Mathf.Max(Screen.width, Screen.height);
            float maxReferenceSize = Mathf.Max(androidReferenceScreen.x, androidReferenceScreen.y);    
            float diff = maxReferenceSize/maxScreenSize;
            if (AndroidIsTallDevice())
            {
                diff *= androidDownScaleMultiplier;
            }
            return diff;
        }
    }


    public static bool AndroidIsTallDevice()
    {
        float minScreenSize = Mathf.Min(Screen.width, Screen.height);
        float maxScreenSize = Mathf.Max(Screen.width, Screen.height);
        if (maxScreenSize / minScreenSize >= androidDownScaleRatio)
        {
            return true;
        }
        return false;
    }

    #endif
}
