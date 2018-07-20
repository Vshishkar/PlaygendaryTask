using UnityEngine;

[AddComponentMenu("Inventain/Tween/MaterialTransparency")]
public class TweenMaterialTransparency : Tweener 
{
	#region Variables   
	
	[SerializeField] float endTransparency = 1f;
	[SerializeField] float beginTransparency = 0f;
	[SerializeField] string shaderTransparencyId = "_Transparency";

	Material targetMaterial;
	int transparencyId;
	
	
	public float EndTransparency
	{       
		get { return endTransparency; }
		set { endTransparency = value; }
	}
	
	
	public float BeginTransparency
	{
		get { return beginTransparency; }
		set { beginTransparency = value; }
	}
	
	
	public Material TargetMaterial
	{
		get
		{
			if (targetMaterial == null)
			{
				targetMaterial = GetComponent<Renderer>().material;
			}

			return targetMaterial;
		}
	}
	
	
	public float CurrentTransparency
	{
		get { return TargetMaterial.GetFloat(transparencyId); }
		set { TargetMaterial.SetFloat(transparencyId, value); }
	}


	public string ShaderTransparencyId
	{
		get { return shaderTransparencyId; }
		set { shaderTransparencyId = value; }
	}

	#endregion 


	#region Unity lifecycle
	
	protected override void Awake()
	{
		transparencyId = Shader.PropertyToID(shaderTransparencyId);

		base.Awake();
	}

	#endregion

	
	#region Public methods
	
	public static TweenMaterialTransparency SetTransparency(GameObject go, float transparency, float duration = 1f) 
	{
		TweenMaterialTransparency twt = Tweener.InitGO<TweenMaterialTransparency>(go);
		twt.BeginTransparency = twt.CurrentTransparency;
		twt.EndTransparency = transparency;
		twt.duration = duration;
		twt.Play(true);
		
		return twt;
	}      
	
	#endregion
	
	
	#region Private methods
	
	protected override void TweenUpdateRuntime(float factor, bool isFinished)
	{
		CurrentTransparency = Mathf.Lerp(BeginTransparency, EndTransparency, factor);
	}
	
	
	protected override void TweenUpdateEditor(float factor)
	{
		CurrentTransparency = Mathf.Lerp(BeginTransparency, EndTransparency, factor);
	}
	
	#endregion
}