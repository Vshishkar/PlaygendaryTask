using UnityEngine;
using UnityEditor;
using System.IO;


[CanEditMultipleObjects, CustomEditor (typeof(DefaultAsset))]
public class DefaultAssetInspector : Editor
{
	Texture2D texture = null;
	public Texture2D Texture 
	{
		get 
		{
			if(texture == null)
			{
				string filePath = Application.dataPath.Replace("Assets","") + AssetDatabase.GetAssetPath(target);
				if (File.Exists(filePath))     
				{
					byte[] fileData = File.ReadAllBytes(filePath);
					texture = new Texture2D(2, 2);
					texture.LoadImage(fileData);
				}
			}

			return texture;
		}
	}


	bool IsTexture
	{
		get 
		{
			string path = AssetDatabase.GetAssetPath(target);
			string ext = Path.GetExtension(path);

			return ext.Equals(".png");
		}
	}


	void OnDisable()
	{
		if(texture != null)
		{
			DestroyImmediate(texture);
			texture = null;
		}
	}


	public override bool HasPreviewGUI()
	{
		return IsTexture && (Texture != null);
	}


	public override void OnInspectorGUI ()
	{
//		GUILayout.Label("sweet place.\nhere can be your reklama");
	}


	public override void OnPreviewGUI(Rect r, GUIStyle background)
	{
		if (Event.current.type == EventType.Repaint) {
			background.Draw(r, false, false, false, false);
		}

		int num = Mathf.Max(texture.width, 1);
		int num2 = Mathf.Max(texture.height, 1);
		float num3 = Mathf.Min(Mathf.Min(r.width / (float)num, r.height / (float)num2), 1);
		Rect rect = new Rect(r.x, r.y, (float)num * num3, (float)num2 * num3);
		EditorGUI.DrawTextureTransparent(rect, Texture);
	}


	public override string GetInfoString()
	{
		string text = Texture.width.ToString() + "x" + Texture.height.ToString();
		return text;
	}
}
