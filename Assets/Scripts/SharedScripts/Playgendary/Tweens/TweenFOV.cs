using UnityEngine; 
using System.Collections;
using System;

[RequireComponent(typeof(Camera))]
public class TweenFOV : Tweener
{
    public event Action OnFovUpdate;

    public float beginFOV = 0;
    public float endFOV = 0;   

    Camera targetCamera;

    protected override void Awake()
    {
        targetCamera = GetComponent<Camera>();

        base.Awake();
    }

    override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        targetCamera.fieldOfView = beginFOV * (1f - factor) + endFOV * factor;
    }       

    public override void Update()
    {
        if (OnFovUpdate != null)
        {
            OnFovUpdate();
        }

        base.Update();
    }
}