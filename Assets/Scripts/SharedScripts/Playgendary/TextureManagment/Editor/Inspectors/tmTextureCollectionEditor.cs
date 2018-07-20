using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(tmTextureCollection))]
public class tmTextureCollectionEditor : Editor {

	public override void OnInspectorGUI() 
	{
		base.OnInspectorGUI();

		if(GUILayout.Button("Build"))
		{
			tmCollectionBuilder.BuildCollection(target as tmTextureCollection);
		}
	}
}
