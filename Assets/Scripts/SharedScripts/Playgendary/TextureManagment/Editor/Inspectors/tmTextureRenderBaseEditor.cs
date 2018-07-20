using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(tmTextureRenderBase), true)]
public class tmTextureRenderBaseEditor : Editor
{
	tmTextureRenderBase render 
	{
		get 
		{
			return target as tmTextureRenderBase;
		}
	}


	List<string> materialPaths = new List<string>();
	List<string> materialNames = new List<string>();


	void OnEnable()
	{
		materialPaths = AssetUtility.GetAssetPaths(tmMaterialUtility.MATERIAL_SUB_PATH);
		materialPaths.RemoveAll( (obj) => !obj.Contains(".mat") );
		materialPaths.Sort((x, y) => string.Compare(x, y, System.StringComparison.Ordinal) );

		for (int i = 0; i < materialPaths.Count; i++) 
		{
			string path = materialPaths[i];
			path = path.Replace(tmMaterialUtility.MATERIAL_SUB_PATH, "");
			path = path.Replace(".mat", "");

			materialNames.Add(path);
		}


		MeshFilter mf = render.GetComponent<MeshFilter>();
		if(mf != null)
		{
//			mf.hideFlags = HideFlags.NotEditable;
		}

		Renderer renderer = render.GetComponent<Renderer>();
		if(renderer != null)
		{
//			renderer.hideFlags = HideFlags.NotEditable;
//
//			renderer.receiveShadows = false;
//			renderer.useLightProbes = false;
//			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//			renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		}
	}



	void OnDisable()
	{
		if(!render.IsNull())
		{
			MeshFilter mf = render.GetComponent<MeshFilter>();
			if(mf != null)
			{
//				mf.hideFlags = HideFlags.None;
			}

			Renderer renderer = render.GetComponent<Renderer>();
			if(renderer != null)
			{
//				renderer.hideFlags = HideFlags.None;
			}
		}
	}



	public override void OnInspectorGUI()
	{
		//main texture
		{
			{
				bool mixed = false;
				foreach(tmTextureRenderBase r in targets)
				{
					mixed |= (r.MainTexCollectionGUID != render.MainTexCollectionGUID);
				}
				EditorGUI.showMixedValue = mixed;
				{
					string mainCollectionGUID = mixed ? "" : render.MainTexCollectionGUID;
					if(CollectionPopup("Main Collection", ref mainCollectionGUID))
					{
						foreach(tmTextureRenderBase r in targets)
						{
							r.MainTexCollectionGUID = mainCollectionGUID;
						}
					}
				}
				EditorGUI.showMixedValue = false;
			}

			{
				bool mixed = false;
				foreach(tmTextureRenderBase r in targets)
				{
					mixed |= (r.MainTextureID != render.MainTextureID);
				}
				EditorGUI.showMixedValue = mixed;
				{
					string mainTextureID = mixed ? "" : render.MainTextureID;
					if(TexturePopup("Main Texture", render.MainTexCollection, ref mainTextureID))
					{
						foreach(tmTextureRenderBase r in targets)
						{
							r.MainTextureID = mainTextureID;
						}
					}
				}
				EditorGUI.showMixedValue = false;
			}
		}

        //lightmap texture

		if (render.Mesh != null && render.Mesh.uv2.Length > 0)
		{
            bool isAOMaterial = render.Material != null && render.Material.IsAOMaterial();

			{
				bool mixed = false;
				foreach (tmTextureRenderBase r in targets)
				{
					mixed |= (r.LightmapCollectionGUID != render.LightmapCollectionGUID);
				}
				EditorGUI.showMixedValue = mixed;
				{
					string lightmapCollectionGUID = mixed ? "" : render.LightmapCollectionGUID;
                    if (CollectionPopup(isAOMaterial ? "AO Collection" : "Lightmap Collection", ref lightmapCollectionGUID))
					{
						foreach (tmTextureRenderBase r in targets)
						{
							r.LightmapCollectionGUID = lightmapCollectionGUID;
						}
					}
				}
				EditorGUI.showMixedValue = false;
			}
			{
				bool mixed = false;
				foreach (tmTextureRenderBase r in targets)
				{
					mixed |= (r.LightmapTextureID != render.LightmapTextureID);
				}
				EditorGUI.showMixedValue = mixed;
				{
					string lightmapTextureID = mixed ? "" : render.LightmapTextureID;
					if (TexturePopup(isAOMaterial ? "AO Texture" : "Lightmap Texture", render.LightmapCollection, ref lightmapTextureID))
					{
						foreach (tmTextureRenderBase r in targets)
						{
							r.LightmapTextureID = lightmapTextureID;
						}
					}
				}
				EditorGUI.showMixedValue = false;
			}
		}

		MeshField();
		MaterialField();

		render.ModifiedFlag = (tmTextureRender.ModifiedFlags)EditorGUILayout.EnumMaskField("ModifiedFlags", render.ModifiedFlag);

		if(GUI.changed)
		{
			render.Rebuild();
		}
	}


	void MaterialField()
	{
		bool mixed = false;
		foreach(tmTextureRenderBase r in targets)
		{
			mixed |= (r.Material != render.Material);
		}
		EditorGUI.showMixedValue = mixed;

		Material current = mixed ? null : render.Material;
		Material selected = MaterialPopup("Material", current);
		if(current != selected)
		{
			foreach(tmTextureRenderBase r in targets)
			{
				r.Material = selected;
			}
		}

		EditorGUI.showMixedValue = false;
	}


	void MeshField()
	{
		bool mixed = false;
		foreach(tmTextureRenderBase r in targets)
		{
			mixed |= (r.Mesh != render.Mesh);
		}
		EditorGUI.showMixedValue = mixed;
		Mesh tempMesh = new Mesh();
		Mesh oldMesh = mixed ? tempMesh : render.Mesh;
		Mesh newMesh = EditorGUILayout.ObjectField("Mesh", oldMesh, typeof(Mesh), false) as Mesh;
		if(oldMesh != newMesh)
		{
			foreach(tmTextureRenderBase r in targets)
			{
				r.Mesh = newMesh;
			}
		}
		DestroyImmediate(tempMesh);
		EditorGUI.showMixedValue = false;
	}


	Material MaterialPopup(string label, Material selected)
	{
		int index = -1;
		for (int i = 0; i < materialPaths.Count; i++) 
		{
			string selectedPath = !selected.IsNull() ? AssetDatabase.GetAssetPath(selected) : "";
			if (materialPaths[i] == selectedPath)
			{
				index = i;
			}
		}

		Rect rect = EditorGUILayout.GetControlRect();
		index = EditorGUI.Popup(rect, label, index, materialNames.ToArray());
		if(index > -1)
		{
			selected = AssetDatabase.LoadAssetAtPath(materialPaths[index], typeof(Material)) as Material;

			if (rect.Contains(Event.current.mousePosition)) 
			{
				if (Event.current.clickCount == 1) 
				{
					if (selected)
					{
						EditorGUIUtility.PingObject(selected);
					}
					Event.current.Use();
				}
			}
		}

		return selected;
	}


	static bool CollectionPopup(string label, ref string collectionGUID)
	{
		int collectionIndex = 0;
		List<string> collectionNames = new List<string>();
		collectionNames.Add("-");

		for (int i = 0; i < tmIndex.Instance.TextureCollections.Count; i++) 
		{
			tmTextureCollectionIndex collectionDescription = tmIndex.Instance.TextureCollections[i];
			collectionNames.Add(collectionDescription.name);

			if (collectionDescription.textureCollectionGUID.Equals(collectionGUID))
			{
				collectionIndex = i + 1;
			}
		}

		int lastCollectionIndex = collectionIndex;
		collectionIndex = EditorGUILayout.Popup(label, collectionIndex, collectionNames.ToArray());

		if (collectionIndex != -1) 
		{
			if (collectionIndex == 0) 
			{
				collectionGUID = "";
			}
			else
			{
				tmTextureCollectionIndex cd = tmIndex.Instance.TextureCollections[collectionIndex - 1];
				collectionGUID = cd.textureCollectionGUID;
			}
		}

		return lastCollectionIndex != collectionIndex;
	}


	static bool TexturePopup(string label, tmTextureCollectionBase collection, ref string texureID)
	{
		int entryIndex = 0;
		List<string> texureNames = new List<string>();
		texureNames.Add("-");

		if (collection != null) 
		{
			for (int i = 0; i < collection.textureDefenitions.Count; i++) 
			{
				tmTextureDefenition def = collection.textureDefenitions[i];
				texureNames.Add(def.textureName);

				if (def.textureGuid.Equals(texureID)) 
				{
					entryIndex = i + 1;
				}
			}
		}

		int lastIndex = entryIndex;
		entryIndex = EditorGUILayout.Popup(label, entryIndex, texureNames.ToArray());

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(" ");
		Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(100));
		EditorGUI.DrawRect(rect, Color.black);
		if (entryIndex != -1) 
		{
			if (entryIndex == 0) 
			{
				texureID = "";
			}
			else
			{
				tmTextureDefenition def = collection.textureDefenitions[entryIndex - 1];
				EditorGUI.DrawTextureTransparent(rect, def.texture, ScaleMode.ScaleToFit,  def.texture == null ? 1f : (def.texture.width * 1f / def.texture.height));
				texureID = def.textureGuid;

				if (rect.Contains(Event.current.mousePosition)) 
				{
					if (Event.current.clickCount == 1) 
					{
						if (def.texture)
						{
							EditorGUIUtility.PingObject(def.texture);
						}
						Event.current.Use();
					}
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		return lastIndex != entryIndex;
	}
}
