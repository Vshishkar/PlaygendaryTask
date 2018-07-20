using UnityEngine; 
using System.Collections;
using System;

[RequireComponent(typeof(Camera))]
public class TweenCameraSize : Tweener
{
    public float beginSize = 0;
    public float endSize = 0;   

    Camera targetCamera;

    protected override void Awake()
    {
        targetCamera = GetComponent<Camera>();

        base.Awake();
    }

    override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        targetCamera.orthographicSize = beginSize * (1f - factor) + endSize * factor;
    }   

}