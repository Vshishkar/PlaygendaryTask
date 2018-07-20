using UnityEngine;
using System.Collections;


[RequireComponent(typeof(tmBatchObject))]
public class tmTextureRender : tmTextureRenderBase 
{
	#region Variables
	tmBatchObject batchObject;
	#endregion


	#region Gettets && Setters
	public tmBatchObject BatchObject 
	{
		get 
		{
			if(batchObject == null)
			{
				batchObject = GetComponent(typeof(tmBatchObject)) as tmBatchObject;
				if(batchObject == null)
				{
					batchObject = gameObject.AddComponent<tmBatchObject>();
				}
			}
			return batchObject; 
		}
	}
	#endregion


	#region Unity

	protected override void OnEnable()
	{
		base.OnEnable();

		#if UNITY_EDITOR
		if(Application.isPlaying)
		#endif
		{
			MainTexCollection += this;
			LightmapCollection += this;
			ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
		}
	}


	protected override void OnDisable()
	{
		BatchObject.Unbatch();

		#if UNITY_EDITOR
		if(Application.isPlaying)
		#endif
		{
			MainTexCollection -= this;
			LightmapCollection -= this;
		}

		base.OnDisable();
	}
	#endregion



	#region Private

	public override void Rebuild()
	{
		base.Rebuild();
		BatchObject.Rebuild();
	}

	#endregion
}
