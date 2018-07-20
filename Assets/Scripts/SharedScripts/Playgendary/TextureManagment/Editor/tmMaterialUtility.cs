using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public static class tmMaterialUtility
{
	public static readonly string MATERIAL_SUB_PATH = "Assets/TextureManagmentCollections/Materials/";


	public static Material SharedMaterial(Material matForCheck)
	{
		List<Material> materials = AssetUtility.GetAssetsAtPath<Material>(MATERIAL_SUB_PATH);
		foreach(Material mat in materials)
		{
			if(mat.shader == matForCheck.shader)
			{
				int propertyCount = ShaderUtil.GetPropertyCount(matForCheck.shader);
				bool propertiesEquals = true;

				for (int i = 0; i < propertyCount && propertiesEquals; i++)
				{
					bool equal = CompareProperty(mat, matForCheck, i);
					propertiesEquals &= equal;
				}

				if(propertiesEquals)
				{
					return mat;
				}
			}
		}

		string path = MATERIAL_SUB_PATH + matForCheck.name + ".mat";

		Material existMat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;
		if (existMat != null)
		{
            CustomDebug.Log("Material with the same name already exist!");
			return existMat;
		}

		Material newMat = Object.Instantiate(matForCheck) as Material;
		newMat.name = matForCheck.name;

		AssetDatabase.CreateAsset(newMat, path);
		AssetDatabase.Refresh();
		AssetDatabase.SaveAssets();

        CustomDebug.Log("New material created: " + path);

		newMat = AssetDatabase.LoadAssetAtPath(path, typeof(Material)) as Material;

		return newMat;
	}


	static bool CompareProperty(Material a, Material b, int propID)
	{
		string propName = ShaderUtil.GetPropertyName(a.shader, propID);
        if(propName.Equals("_MainTex") || propName.Equals(MaterialExtension.LIGHTMAP_KEY) || propName.Equals(MaterialExtension.AO_KEY))
		{
			return true;
		}

		ShaderUtil.ShaderPropertyType propType = ShaderUtil.GetPropertyType(a.shader, propID);

		if(propType == ShaderUtil.ShaderPropertyType.Color)
		{
			return a.GetColor(propName).Equals(b.GetColor(propName));
		}

        if(propType == ShaderUtil.ShaderPropertyType.Float || propType == ShaderUtil.ShaderPropertyType.Range)
		{
			return Mathf.Abs(a.GetFloat(propName)- b.GetFloat(propName)) < float.Epsilon;
		}

		if(propType == ShaderUtil.ShaderPropertyType.TexEnv)
		{
			Texture texA = a.GetTexture(propName);
			Texture texB = b.GetTexture(propName);

			return texA && texB && texA.Equals(texB);
		}

		return false;
	}



	public static List<Material> SharedMaterials()
	{
		List<Material> materials = AssetUtility.GetAssetsAtPath<Material>(MATERIAL_SUB_PATH);
		return materials;
	}
}
