using UnityEngine;
using UnityEditor;
using System.Collections;
[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectPool))]
public class PoolPreinstantiate : Editor {
	
		public override void OnInspectorGUI() {
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(0, 0);
        #endif
			base.OnInspectorGUI();


			GUILayout.BeginHorizontal("Box");
			{
				if(GUILayout.Button("PreInstantiate"))
				Preinstantiate();
			}
			GUILayout.EndHorizontal();
		}

	void Preinstantiate ()
	{
		foreach(ObjectPool p in targets)
		{
			for (int i = 0; i < p.preInstantiateCount; i++) {
				GameObject go = Instantiate(p.prefab) as GameObject;
				go.transform.parent = p.transform;
			}
		}
	}
}
