using UnityEngine;

[AddComponentMenu("Inventain/Tween/Rotation")]
public class TweenRotation : Tweener 
{
    #region Variables   

    Transform targetTransform;

    [SerializeField] Vector3 endRotation = Vector3.one;
    [SerializeField] Vector3 beginRotation = Vector3.zero;

    public Vector3 EndRotation
    {       
        get { return endRotation; }
        set { endRotation = value; }
    }

    public Vector3 BeginRotation
    {
        get { return beginRotation; }
        set { beginRotation = value; }
    }

    public Transform TargetTransform
    {
        get
        {
            if (targetTransform == null)
            {
                targetTransform = transform;
            }
            return targetTransform;
        }
    }

    public Vector3 CurrentRotation
    {
        get { return TargetTransform.localRotation.eulerAngles; }
        set
        {
            TargetTransform.localRotation = Quaternion.Euler(value);
        }
    }
    #endregion 


    #region Private   

    protected override void TweenUpdateRuntime(float factor, bool isFinished)
    {
        CurrentRotation = BeginRotation + (EndRotation - BeginRotation) * factor;
    }

    protected override void TweenUpdateEditor(float factor)
    {
        CurrentRotation = BeginRotation + (EndRotation - BeginRotation) * factor;
    }

    static public TweenRotation SetRatation(GameObject go, Vector3 rotation, float duration = 1f) 
    {
        TweenRotation tws = Tweener.InitGO<TweenRotation>(go);
        tws.BeginRotation = tws.CurrentRotation;
        tws.EndRotation = rotation;
        tws.duration = duration;
        tws.Play(true);
        return tws;
    }       

    #endregion


}
