using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor (typeof(TextAsset))]
public class TextAssetCustomInspector : Editor
{
	const int TEXT_MAX_LENGTH = (1 << 14) - 2;


	AssetImporter importer;
	AssetImporter Importer
	{
		get
		{
			if(importer == null)
			{
				importer = AssetImporter.GetAtPath (AssetDatabase.GetAssetPath((target as TextAsset)));
			}
			return importer;
		}
	}


	public override void OnInspectorGUI ()
	{
		GUILayout.Label("Description");
		GUILayout.BeginHorizontal(EditorStyles.label);
		{
			GUILayout.TextArea(Importer.userData);
		}
		GUILayout.EndHorizontal();


		GUILayout.Label("File Content");
		GUILayout.BeginHorizontal(EditorStyles.miniLabel);
		{
			string text = (target as TextAsset).text;
			int length = Mathf.Min (text.Length, TEXT_MAX_LENGTH);
			text = text.Substring(0, length);

			GUILayout.TextArea (text);
		}
		GUILayout.EndHorizontal();


		GUILayout.FlexibleSpace ();
	}
}