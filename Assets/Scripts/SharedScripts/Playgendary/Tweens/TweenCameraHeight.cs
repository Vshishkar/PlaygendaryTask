using UnityEngine; 
using System.Collections;
using System;

[RequireComponent(typeof(Camera))]
public class TweenCameraHeight : Tweener
{
    public float beginHeight = 0;
    public float endHeight = 0;   

    Camera targetCamera;

    protected override void Awake()
    {
        targetCamera = GetComponent<Camera>();

        base.Awake();
    }

    override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {    
        float currHeight = beginHeight * (1f - factor) + endHeight * factor;

        targetCamera.rect = new Rect(0, (1 - currHeight) * 0.5f, 1, currHeight);
    }   

}