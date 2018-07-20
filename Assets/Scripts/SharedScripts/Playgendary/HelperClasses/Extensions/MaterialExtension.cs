using UnityEngine;
using System.Collections;

public static class MaterialExtension 
{
	public static readonly string LIGHTMAP_KEY = "_Lightmap";
    public static readonly string AO_KEY = "_AO";


    public static bool IsAOMaterial(this Material mat)
    {
        return mat.HasProperty(AO_KEY);
    }


	public static Texture GetLightmapTexture(this Material mat)
	{
		if(mat.HasProperty(LIGHTMAP_KEY))
		{
			return mat.GetTexture(LIGHTMAP_KEY);
		}
		else if (mat.HasProperty(AO_KEY))
		{
			return mat.GetTexture(AO_KEY);
		}

		return null;
	}


	public static void SetLightmapTexture(this Material mat, Texture texture)
	{
		if(mat.HasProperty(LIGHTMAP_KEY))
		{
			mat.SetTexture(LIGHTMAP_KEY, texture);
		}
		else if (mat.HasProperty(AO_KEY))
		{
			mat.SetTexture(AO_KEY, texture);
		}
	}
}
