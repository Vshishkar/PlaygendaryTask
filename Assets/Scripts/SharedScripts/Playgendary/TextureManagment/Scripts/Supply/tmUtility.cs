using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class tmUtility 
{
	public static string PlatformlessPath(string path)
	{
		if(!string.IsNullOrEmpty(path))
		{
			foreach(tmPlatform p in tmSettings.allPlatfrorms)
			{
				path = path.Replace(p.postfix, "");
			}
		}
		return path;
	}


	public static string PathForPlatform(string path, tmPlatform platform)
	{
		if(!string.IsNullOrEmpty(path))
		{
			string directory = System.IO.Path.GetDirectoryName(path);
			if(!string.IsNullOrEmpty(directory))
			{
				directory += "/";
			}
			string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
			string fileExtension = System.IO.Path.GetExtension(path);

			fileName = PlatformlessPath(fileName);
			path = directory + fileName + platform.postfix + fileExtension;
		}
		return path;
	}


	public static string NewGUID()
	{
		return System.Guid.NewGuid().ToString().Replace("-", "");
	}


	public static tmResourceCollectionLink ResourceLinkByGUID(string guid)
	{
		string resourcePath = tmSettings.ResourceFolder + tmSettings.ResourceLinkPrefix + guid;
		tmResourceCollectionLink link = Resources.Load(resourcePath, typeof(tmResourceCollectionLink)) as tmResourceCollectionLink;
		return link;
	}


    public static List<tmResourceCollectionLink> GetAllResourceLinks(tmPlatform platform)
    {
        tmResourceCollectionLink[] allLinks = Resources.LoadAll<tmResourceCollectionLink>(tmSettings.ResourceFolder);

        HashSet<string> allCollectionsGuids = new HashSet<string>();
        for (int i = 0; i < allLinks.Length; ++i)
        {
            string collectionGuid = PlatformlessPath(allLinks[i].name).Replace(tmSettings.ResourceLinkPrefix, "");
            allCollectionsGuids.Add(collectionGuid);
        }


        List<tmResourceCollectionLink> platformLinks = new List<tmResourceCollectionLink>();

        foreach (string collectionGuid in allCollectionsGuids)
        {
            platformLinks.Add(ResourceLinkByGUID(collectionGuid + platform.postfix));
        }

        return platformLinks;
    }


    public static List<tmResourceCollectionLink> GetAllResourceLinksFor(tmPlatform platform, List<tmResourceCollectionLink> _links)
    {
        HashSet<string> allCollectionsGuids = new HashSet<string>();
        for (int i = 0; i < _links.Count; ++i)
        {
            string collectionGuid = PlatformlessPath(_links[i].name).Replace(tmSettings.ResourceLinkPrefix, "");
            allCollectionsGuids.Add(collectionGuid);
        }


        List<tmResourceCollectionLink> platformLinks = new List<tmResourceCollectionLink>();

        foreach (string collectionGuid in allCollectionsGuids)
        {
            platformLinks.Add(ResourceLinkByGUID(collectionGuid + platform.postfix));
        }

        return platformLinks;
    }


	public static void ValidateMesh(Mesh mesh)
	{
		#if UNITY_EDITOR
		if(mesh != null)
		{
			if(!mesh.isReadable)
			{
				string path = UnityEditor.AssetDatabase.GetAssetPath(mesh);
				UnityEditor.ModelImporter mImporter = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.ModelImporter;
				mImporter.isReadable = true;
				UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
			}

			bool outOfRange = false;
			Vector2[] uv1 = mesh.uv;
			for (int i = uv1.Length - 1; i >= 0 && !outOfRange; i--) 
			{
				Vector2 v = uv1[i];
				outOfRange = (v.x < 0) || ( v.x > 1f) || (v.y < 0) || (v.y > 1f);
				if(outOfRange)
				{
					CustomDebug.LogError("WRONG MESH UV : " + "(" + v.x + " : " + v.y + ")" + " : " + mesh.name);
				}
			}

			if(outOfRange)
			{
				CustomDebug.LogError("WRONG MESH UV : " + UnityEditor.AssetDatabase.GetAssetPath(mesh) + "/" + mesh.name);
			}
		}
		#endif
	}
}
