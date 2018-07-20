using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class tmTexturePostprocessor : AssetPostprocessor 
{
	void OnPreprocessTexture()
	{
		if (tmSettings.DoesInstanceExist && tmSettings.Instance.autoRebuild) // fix while unity not launching
		{
			if (tmIndex.DoesInstanceExist && tmIndex.Instance.CollectionIndexForTexturePath(assetPath) != null)
			{
				tmCollectionBuilder.ConfigureSpriteTextureImporter(assetPath);
			}
		}
	}

	static List<string> waitForImportAssets = new List<string>();

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		waitForImportAssets.AddRange(importedAssets);
		EditorApplication.delayCall -= UpdateModifiedAssets;
		EditorApplication.delayCall += UpdateModifiedAssets;
	}


	static void UpdateModifiedAssets()
	{
		string[] importedAssets = waitForImportAssets.ToArray();
		waitForImportAssets.Clear();

		if (tmSettings.Instance.autoRebuild && importedAssets != null && importedAssets.Length != 0)
		{
			tmCollectionBuilder.BuildCollectionsForModifiedAssets(importedAssets);
		}

		List<Material> modifiedMaterials = new List<Material>();
		foreach(string path in importedAssets)
		{
			if (path.Contains(tmMaterialUtility.MATERIAL_SUB_PATH)) 
			{
				Material mat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
				if(mat != null && !modifiedMaterials.Contains(mat))
				{
					modifiedMaterials.Add(mat);
				}
			}
		}

		if(modifiedMaterials.Count > 0)
		{
			tmManager.Instance.ClearMaterials();
		}

		List<tmTextureRender> renders = GameObjectExtension.GetAllObjectsInScene<tmTextureRender>();
		renders.ForEach(
			f =>
			{
				if(modifiedMaterials.Contains(f.Material))
				{
					f.ModifiedFlag |= tmTextureRender.ModifiedFlags.ModifiedMaterial;
				}
			}
		);
	}
}

