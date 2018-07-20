using UnityEngine;
using UnityEditor;
using System.Reflection;


[CanEditMultipleObjects, CustomEditor(typeof(TextureImporter))]
public class TextureImporterCustomInspector : Editor {

	static System.Type internalType = GetType("TextureImporterInspector");

	static System.Type GetType(string typeName)
	{
		foreach(Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
		{
			System.Type[] types = a.GetTypes();

			foreach(System.Type t in types)
			{
				if(t.Name.Equals(typeName))
				{
					return t;
				}
			}
		}

		return null;
	}


	Editor baseEditor = null;
	int newIndex = -2;

	void OnEnable()
	{
		baseEditor = Editor.CreateEditor(targets, internalType);
	}


	void OnDisable()
	{
		DestroyImmediate(baseEditor);
	}


	public override void OnInspectorGUI() 
	{
//		EyeDropper.GetPickedColor();
//		System.Type eyeDropperType = GetType("EyeDropper");
//		MethodInfo mi = eyeDropperType.GetMethod("GetPickedColor", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
//		FieldInfo fi = eyeDropperType.GetField("s_PickCoordinates", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
//		Vector2 a = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
//		fi.SetValue(null, a);
//		Debug.Log(fi);
//		Debug.Log(mi);
//		Color c = (Color)mi.Invoke(null, null);
//
//
//		EditorGUILayout.ColorField(Color.white);
//		Debug.Log(
//			"r:" + ((int)(c.r * 256)) +
//			" g:" + ((int)(c.g * 256)) +
//			" b:" + ((int)(c.b * 256)) +
//			" a:" + ((int)(c.a * 256))
//		);
		baseEditor.OnInspectorGUI();

		GUILayout.Space(20);

		TextureImporter textureImporter = (TextureImporter)target;

		tmTextureCollectionIndex currentCollectionIndex = tmIndex.Instance.CollectionIndexForTexturePath(textureImporter.assetPath);
		string[] names = new string[tmIndex.Instance.TextureCollections.Count];

		int index = -1;
		for (int i = 0; i < tmIndex.Instance.TextureCollections.Count; i++) 
		{
			tmTextureCollectionIndex collectionIndex = tmIndex.Instance.TextureCollections[i];
			names[i] = collectionIndex.name;

			if(collectionIndex.Equals(currentCollectionIndex))
			{
				index = i;
			}
		}

		if(newIndex < -1)
		{
			newIndex = index;
		}
		newIndex = EditorGUILayout.Popup("Texture Atlas", newIndex, names);

		if(newIndex > 0)
		{
			tmTextureCollectionIndex collectionIndex = tmIndex.Instance.TextureCollections[newIndex];
			string collectionGUID = collectionIndex.textureCollectionGUID;
			string collectionGuidPath = tmUtility.PathForPlatform(collectionGUID, tmSettings.Instance.CurrentPlatform);
			tmResourceCollectionLink link = tmUtility.ResourceLinkByGUID(collectionGuidPath);
			tmTextureCollectionPlatform collection = link.collectionInEditor;
			collection.LoadTexture();
			Texture2D atlas = collection.Atlas;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(" ");
			Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(150f * atlas.width / atlas.height), GUILayout.Height(150f));
			EditorGUI.DrawRect(rect, Color.black); 
			EditorGUI.DrawTextureTransparent(rect, atlas, ScaleMode.ScaleToFit);
			EditorGUILayout.EndHorizontal();

			Object asset = tmEditorUtility.GUIDToAsset(collection.AtlasAssetGUID,  typeof(Object));
			if (rect.Contains(Event.current.mousePosition)) 
			{
				if (Event.current.clickCount == 1) 
				{
					if (asset)
					{
						EditorGUIUtility.PingObject(asset);
					}
					Event.current.Use();
				}
			}
		}

		GUILayout.BeginHorizontal();
		{
			if(currentCollectionIndex != null)
			{
				EditorGUILayout.PrefixLabel(" ");

				if(GUILayout.Button("Remove from atlas"))
				{
					newIndex = -1;
				}
			}

			GUILayout.FlexibleSpace();

			bool enabled = GUI.enabled;
			GUI.enabled = index != newIndex;

			if(GUILayout.Button("Revert"))
			{
				newIndex = -2;
			}
			if(GUILayout.Button("Apply"))
			{
				if(currentCollectionIndex != null)
				{
					tmTextureCollection collection = tmEditorUtility.GUIDToAsset(currentCollectionIndex.assetGUID, typeof(tmTextureCollection)) as tmTextureCollection;
                    CustomDebug.Log(collection);
					if(collection)
					{
						tmTextureDefenition def = collection.GetTextureDefenitionByID(AssetDatabase.AssetPathToGUID(textureImporter.assetPath));
						if(def != null)
						{
                            CustomDebug.Log(def.texture);

							collection.textureDefenitions.Remove(def);
							collection.Textures.Remove(def.texture);
							EditorUtility.SetDirty(collection);

							tmCollectionBuilder.BuildCollection(collection);
						}
					}
				}

				if(newIndex > 0)
				{
					tmTextureCollectionIndex newCollectionIndex = tmIndex.Instance.TextureCollections[newIndex];
					tmTextureCollection collection = tmEditorUtility.GUIDToAsset(newCollectionIndex.assetGUID, typeof(tmTextureCollection)) as tmTextureCollection;
					collection.Textures.Add( AssetDatabase.LoadAssetAtPath(textureImporter.assetPath, typeof(Texture2D)) as Texture2D );
					EditorUtility.SetDirty(collection);

					tmCollectionBuilder.BuildCollection(collection);
				}

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			GUI.enabled = enabled;
		}
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Label("Description");
		GUILayout.BeginHorizontal("Box");
		{
			textureImporter.userData = GUILayout.TextField(textureImporter.userData,GUI.skin.label);
		}
		GUILayout.EndHorizontal();

//		EditorGUILayout.LabelField("test label");
	}
}