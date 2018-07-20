using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(tmBatchObject))]
public class tmBatchObjectEditor : Editor 
{
	tmBatchingType[] toggleTypes = {tmBatchingType.None, tmBatchingType.Dynamic, tmBatchingType.Static, tmBatchingType.Skinning};


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();


		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel("Vertex Count");
			int vertexCount = 0;
			foreach(tmBatchObject subTarget in targets)
			{
				MeshFilter filter = subTarget.GetComponent<MeshFilter>();
				if(filter != null)
				{
					if(filter.sharedMesh != null)
						vertexCount += filter.sharedMesh.vertexCount;
				}
				else
				{
					SkinnedMeshRenderer smr = subTarget.GetComponent<SkinnedMeshRenderer>();
					if(smr != null && smr.sharedMesh != null)
					{
						vertexCount += smr.sharedMesh.vertexCount;
					}
				}
			}
			EditorGUILayout.LabelField("" + vertexCount);
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel ("Batching Type");

			int index = 0;
			foreach(tmBatchingType toggle in toggleTypes)
			{
				bool[] toggles = new bool[toggleTypes.Length];

				foreach(tmBatchObject subTarget in targets)
				{
					toggles[index] |= (subTarget.BatchingType == toggle);
				}

				bool lastToggle = toggles[index];
				toggles[index] = GUILayout.Toggle(toggles[index], toggle.ToString(), "Button");

				if(lastToggle ^ toggles[index])
				{
					foreach(tmBatchObject subTarget in targets)
					{
						subTarget.BatchingType = toggle;
					}
				}

				index++;
			}
		}
		EditorGUILayout.EndHorizontal();


//		EditorGUILayout.BeginHorizontal();
//		{
//			EditorGUILayout.PrefixLabel ("Root");
//
//			Transform lastRoot = (target as tmBatchObject).Root;
//			Transform root = EditorGUILayout.ObjectField(lastRoot, typeof(Transform), true) as Transform;
//
//			if(lastRoot != root)
//			{
//				foreach(tmBatchObject subTarget in targets)
//				{
//					subTarget.Root = root;
//				}
//			}
//		}


//		if(GUILayout.Button("auto"))
//		{
//			foreach(tmBatchObject subTarget in targets)
//			{
//				subTarget.SetUp();
//			}
//		}
	}
}
