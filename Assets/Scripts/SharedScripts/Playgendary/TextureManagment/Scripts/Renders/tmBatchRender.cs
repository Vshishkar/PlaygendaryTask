using UnityEngine;
using System.Collections.Generic;


public class tmBatchRender : tmTextureRenderBase 
{
	public override void Rebuild()
	{
		UpdateMaterial();

		#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
		#endif

		ModifiedFlag = 0;
	}



	static Dictionary<Mesh, int> meshRetainCount = new Dictionary<Mesh, int>();
	static void RetainMesh(Mesh mesh)
	{
		if(!meshRetainCount.ContainsKey(mesh))
		{
			meshRetainCount.Add(mesh, 0);
		}

		meshRetainCount[mesh]++;
	}


	static void ReleaseMesh(Mesh mesh)
	{
		if(meshRetainCount.ContainsKey(mesh) && --meshRetainCount[mesh] == 0)
		{
//			Debug.Log("Unloading : " + mesh.name);
			Resources.UnloadAsset(mesh);
		}
	}

	#region Unity

	protected override void OnEnable()
	{
		base.OnEnable();

		#if UNITY_EDITOR
		if(Application.isPlaying)
		#endif
		{
			Mesh mesh = RenderSharedMesh;
			if(mesh != null && !mesh.isReadable)
			{
				RetainMesh(mesh);
			}
			MainTexCollection += this;
			LightmapCollection += this;
			ModifiedFlag |= ModifiedFlags.ModifiedMaterial;
		}
	}


	protected override void OnDisable()
	{
		#if UNITY_EDITOR
		if(Application.isPlaying)
		#endif
		{
			Mesh mesh = RenderSharedMesh;
			if(mesh != null && !mesh.isReadable)
			{
				ReleaseMesh(mesh);
			}
			MainTexCollection -= this;
			LightmapCollection -= this;
		}

		base.OnDisable();
	}

	#endregion
}
