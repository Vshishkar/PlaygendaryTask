using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;


public class tmEditorUtility
{
    public static string GetNewPrefabPath(string name) // name is the filename of the prefab EXCLUDING .prefab
    {
        Object obj = Selection.activeObject;
        string assetPath = AssetDatabase.GetAssetPath(obj);
        if (assetPath.Length == 0)
        {
            assetPath = SaveFileInProject("Create...", "Assets/", name, "prefab");
        }
        else
        {
            // is a directory
            string path = System.IO.Directory.Exists(assetPath) ? assetPath : System.IO.Path.GetDirectoryName(assetPath);
            assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".prefab");
        }

        return assetPath;
    }


    public static string SaveFileInProject(string title, string directory, string filename, string ext)
    {
        string path = EditorUtility.SaveFilePanel(title, directory, filename, ext);
        if (path.Length == 0) // cancelled
            return "";
        string cwd = System.IO.Directory.GetCurrentDirectory().Replace("\\","/") + "/assets/";
        if (path.ToLower().IndexOf(cwd.ToLower()) != 0)
        {
            path = "";
            EditorUtility.DisplayDialog(title, "Assets must be saved inside the Assets folder", "Ok");
        }
        else 
        {
            path = path.Substring(cwd.Length - "/assets".Length);
        }
        return path;
    }


	public static string AssetToGUID(Object asset)
	{
		return AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath(asset) );
	}


	public static Object GUIDToAsset(string guid, System.Type type)
	{
		return AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(guid), type );
	}


	public static int NiceRescaleK( float scale ) {
		if (scale > 0.499f && scale < 0.501f) {
			return 2;
		}
		else if (scale > 0.249f && scale < 0.251f) {
			return 4;
		}
		return 0;
	}

	// Rescale a texture
	// Only supports
	public static Texture2D RescaleTexture(Texture2D texture, float scale) 
	{
		// If globalTextureRescale is 0.5 or 0.25, average pixels from the larger image. Otherwise just pick one pixel, and look really bad
		int niceRescaleK = NiceRescaleK( scale );
		bool niceRescale = niceRescaleK != 0;
		if (texture != null) 
        {
			int k = niceRescaleK;
			int srcW = texture.width, srcH = texture.height;
			int dstW = niceRescale ? ((srcW + k - 1) / k) : (int)(srcW * scale);
			int dstH = niceRescale ? ((srcH + k - 1) / k) : (int)(srcH * scale);
            Texture2D dstTex = new Texture2D(dstW, dstH, texture.format, texture.mipmapCount > 0);
			for (int dstY = 0; dstY < dstH; ++dstY) 
            {
				for (int dstX = 0; dstX < dstW; ++dstX) 
                {
					if (niceRescale) 
                    {
						Color sumColor = new Color(0, 0, 0, 0);
						float w = 0.0f;
						for (int dy = 0; dy < k; ++dy) 
                        {
							int srcY = dstY * k + dy;
							if (srcY >= srcH) continue;
							for (int dx = 0; dx < k; ++dx) 
                            {
								int srcX = dstX * k + dx;
								if (srcX >= srcW) continue;
								w += 1.0f;
								Color srcColor = texture.GetPixel(srcX, srcY);
								sumColor += srcColor;
							}
                        }
                        dstTex.SetPixel(dstX, dstY, (w > 0.0f) ? (sumColor * (1.0f / w)) : Color.black);
					} 
                    else
                    {
                        Color c = texture.GetPixelBilinear((float)dstX / (float)dstW, (float)dstY / (float)dstH);
						dstTex.SetPixel(dstX, dstY, c);
					}
				}
			}
			dstTex.Apply();
			return dstTex;
		}
		else 
        {
			return null;
		}
	}


	public static Mesh MeshFromSubmesh(Mesh mesh, int submeshIndex)
	{
		Mesh submesh = new Mesh();
		submesh.name = mesh.name + submeshIndex;

		int[] triangles = mesh.GetTriangles(submeshIndex);
		int indexCount = triangles.Length;

		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector2[] uv = mesh.uv;
		Vector2[] uv2 = mesh.uv2;

		bool use_uv2 = uv2.Length > 0;
		bool use_normals = normals.Length > 0;

		List<Vector3> newvertices = new List<Vector3>();
		List<Vector3> newnormals = new List<Vector3>();
		List<Vector2> newuv = new List<Vector2>();
		List<Vector2> newuv2 = new List<Vector2>();

		Dictionary<int, int> indexMap = new Dictionary<int, int>();
		int currentIndex = 0;

		for(int j = 0; j < indexCount; j++)
		{
			int index = triangles[j];
			if(indexMap.ContainsKey(index))
			{
				triangles[j] = indexMap[index];
			}
			else
			{
				newvertices.Add(vertices[index]);
				newuv.Add(uv[index]);

				if(use_uv2)
				{
					newuv2.Add(uv2[index]);
				}

				if(use_normals)
				{
					newnormals.Add(normals[index]);
				}

				triangles[j] = currentIndex;
				indexMap.Add(index, currentIndex);

				currentIndex++;
			}
		}

		submesh.vertices = newvertices.ToArray();
		submesh.uv = newuv.ToArray();

		if(use_uv2)
		{
			submesh.uv2 = newuv2.ToArray();
		}

		if(use_normals)
		{
			submesh.normals = newnormals.ToArray();
		}

		submesh.SetTriangles(triangles, 0);

        #if !UNITY_5_5_OR_NEWER
        submesh.Optimize();
        #endif
		submesh.RecalculateBounds();

		return submesh;
	}


	public static void SplitMesh(Mesh mesh)
	{
		for (int i = 0; i < mesh.subMeshCount; i++) 
		{
			string name = mesh.name + "_" + i;

			string assetPath = AssetDatabase.GetAssetPath(mesh);
			Mesh newMesh = MeshFromSubmesh(mesh, i);
			newMesh.name = name;

            CustomDebug.Log("add to : "  + assetPath);
			AssetDatabase.AddObjectToAsset(newMesh, assetPath);
			AssetDatabase.Refresh();
		}
	}


	public static string FullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: FullPath(go.transform.parent.gameObject) + "/" + go.name;
	}


	public static ulong Hash(Object asset)
	{
		string path = AssetDatabase.GetAssetPath(asset);
		string assetPath = Application.dataPath.Replace("Assets","") + path;
		string metaPath = Application.dataPath.Replace("Assets","") + path + ".meta";

		ulong timestamp = Hash(assetPath) + Hash(metaPath);

		return timestamp;
	}


	public static ulong Hash(string path)
	{
		using (FileStream stream = File.OpenRead(path))
		{
			SHA256Managed sha = new SHA256Managed();
			byte[] checksum = sha.ComputeHash(stream);
			return System.BitConverter.ToUInt64(checksum, 0);
		}
	}
}
