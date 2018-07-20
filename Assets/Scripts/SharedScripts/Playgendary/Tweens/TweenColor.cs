using UnityEngine;

public interface ITweenColor
{
	Color Color
	{
		get;
		set;
	}
}


[AddComponentMenu("Inventain/Tween/Color")]
public class TweenColor : Tweener
{
    public System.Action OnColorChanged;

	public bool[] useChanelMask = { true, true, true, true };
	public Color beginColor = new Color(1f, 1f, 1f, 0f);
	public Color endColor = new Color(1f, 1f, 1f, 1f);
	
	[SerializeField]
	GameObject target;
	tk2dBaseSprite tk2dSprite;
	tk2dTextMesh tk2dLabel;
	ColorQuad quad;
    DrawableMesh drawableMesh;
    MeshRenderer meshRenderer;
	ITweenColor objectColor;
    bool needHandleMeshRenderer;
	
    void Start()
    {        
        needHandleMeshRenderer = GetComponent<tmBatchObject>() == null;
    }

	protected override void Awake()
	{
		base.Awake ();
		InitReference (false);
	}

    void InitReference(bool force) 
    {
        if (force || ((tk2dSprite == null) && (tk2dLabel == null) && (quad == null) && (drawableMesh == null))) 
        {
			tk2dSprite = Target.GetComponent<tk2dBaseSprite>();
			tk2dLabel = Target.GetComponent<tk2dTextMesh>();
			quad = Target.GetComponent<ColorQuad>();
            drawableMesh = Target.GetComponent<DrawableMesh>();
			objectColor = Target.GetComponent<ITweenColor>();
            if ((tk2dSprite == null) && 
				(tk2dLabel == null) && 
				(quad == null) && 
				(drawableMesh == null) &&
				(objectColor == null))
            {
                meshRenderer = Target.GetComponent<MeshRenderer>();
                if (meshRenderer != null && (meshRenderer.sharedMaterial == null || !meshRenderer.sharedMaterial.HasProperty("_Color")))
                {
                    CustomDebug.LogWarning("Wrong material!");
                    meshRenderer = null;
                }
            }
		}
	}

	Color ApplyChanelMask(Color value, Color source) 
    {
		if (!useChanelMask[0]) 
        {
			value.r = source.r;
		}

		if (!useChanelMask[1]) 
        {
			value.g = source.g;
		}

		if (!useChanelMask[2]) 
        {
			value.b = source.b;
		}

		if (!useChanelMask[3]) 
        {
			value.a = source.a;
		}
		return value;
	}

	
	override protected void TweenUpdateRuntime(float factor, bool isFinished) 
    {
        CurrentColor = Color.Lerp(beginColor, endColor, factor);
	}
		
	
	override public string TargetName {
		get { return Target.name; }
	}
	
	public GameObject Target {
		get {
			if (target == null) {
				target = gameObject;
			}
			return target;
		}
		set {
			target = value;
			if (target == null) {
				target = Target;
			}
			InitReference(true);
		}
	}

    public bool IsReferenceSetUp
    {
        get
        {
            if (tk2dSprite != null) 
            {
                return true;
            }

            if (tk2dLabel != null) 
            {
                return true;
            }

            if (quad != null) 
            {
                return true;
            }

            if (drawableMesh != null)
            {
                return true;
            }

            if (meshRenderer != null)
            {
                return true;
            }

			if (objectColor != null)
			{
				return true;
			}

            return false;
        }
    }
        

	public Color CurrentColor 
    {
		get 
        {
			InitReference(false);
			if (tk2dSprite != null) 
            {
				return tk2dSprite.color;
			}

			if (tk2dLabel != null) 
            {
				return tk2dLabel.color;
			}

			if (quad != null) {
				return quad.TintColor;
			}

            if (drawableMesh != null)
            {
                return drawableMesh.Color;
            }

            if (meshRenderer != null)
            {
                return meshRenderer.sharedMaterial.color;
            }

			if (objectColor != null)
			{
				return objectColor.Color;
			}

			return Color.black;
		}
		set 
        {
			InitReference(false);
            if (tk2dSprite != null)
            {
                tk2dSprite.color = ApplyChanelMask(value, tk2dSprite.color);
                if (needHandleMeshRenderer)
                {
                    tk2dSprite.GetComponent<Renderer>().enabled = tk2dSprite.color.a > 0.01f;
                }
            }
            else if (tk2dLabel != null)
            {
                var currentColor = tk2dLabel.color;
                var newColor = ApplyChanelMask(value, currentColor);

                if (currentColor != newColor)
                {
                    tk2dLabel.color = newColor;
                }

                if (needHandleMeshRenderer)
                {
                    tk2dLabel.GetComponent<Renderer>().enabled = tk2dLabel.color.a > 0.01f;
                }
            }
            else if (quad != null)
            {
                quad.TintColor = ApplyChanelMask(value, quad.TintColor);
            }
            else if (drawableMesh != null)
            {
                drawableMesh.Color = ApplyChanelMask(value, drawableMesh.Color);
            }
            else if (meshRenderer != null)
            {
                meshRenderer.sharedMaterial.color = ApplyChanelMask(value, meshRenderer.sharedMaterial.color);
            }
			else if (objectColor != null)
			{
				objectColor.Color = ApplyChanelMask(value,objectColor.Color);
			}

            if (OnColorChanged != null)
            {
                OnColorChanged();
            }
		}
	}
	

	public bool IsOnlyAlphaTween {
		get { return !useChanelMask[0] && !useChanelMask[1] && !useChanelMask[2] && useChanelMask[3]; }
	}
			
	
    static public TweenColor SetColor(GameObject go, Color color, float duration = 1f) 
    {
		var twc = Tweener.InitGO<TweenColor>(go, duration);
        twc.InitReference(true);
        twc.beginColor = twc.CurrentColor;
        twc.endColor = color;		
        twc.Play(true);
		return twc;
	}
}