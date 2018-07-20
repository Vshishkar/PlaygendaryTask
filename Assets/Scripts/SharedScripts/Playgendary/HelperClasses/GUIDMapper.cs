using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using System.Text;
#endif

public static class GUIDMapper 
{
    #region Variables

    #if !UNITY_EDITOR
    const int GUID_LENGTH = 32;
    static Dictionary<string, string> guidMap = new Dictionary<string, string>();
    static bool isParse;
    #endif

    #endregion


    #region Public methods

    public static string GUIDToAssetPath(string guid)
    {
        string path = string.Empty;
        #if UNITY_EDITOR
        path = AssetDatabase.GUIDToAssetPath(guid);
        #else
        if (!isParse)
        {
            Parse();
        }
        if (!guidMap.TryGetValue(guid, out path))
        {
            CustomDebug.Log("GUIDMapper.Log can't find path for Guid : " + guid);
        }
        #endif
        return path;
    }

    #endregion


    #region Private methods

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        #if !UNITY_EDITOR
        if (!isParse)
        {
            Parse();
        }
        #endif
    }

    #if !UNITY_EDITOR
    static void Parse()
    {
        if (!isParse)
        {
            string filePath = Application.streamingAssetsPath + "/guids";
            string result = string.Empty;
            bool isCorrect = false;

            float time = Time.realtimeSinceStartup;

            if (filePath.Contains(PathUtils.WWW_FILE_PREFIX))
            {
                using (WWW wwwFile = new WWW(filePath))
                {
                    while (!wwwFile.isDone) { System.Threading.Thread.Sleep(1); }
                    isCorrect = string.IsNullOrEmpty(wwwFile.error);
                    if (isCorrect)
                    {
                        result = wwwFile.text;
                    }
                }
            }
            else
            {
                isCorrect = File.Exists(filePath);
                if (isCorrect)
                {
                    result = System.IO.File.ReadAllText(filePath);
                }
            }

            if (isCorrect)
            {
                string[] paths = result.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, pathsLength = paths.Length; i < pathsLength; i++) 
                {
                    var line = paths[i];
                    string guid = line.Substring(0, GUID_LENGTH);
                    string path = line.Substring(GUID_LENGTH, line.Length - GUID_LENGTH);
                    guidMap.Add(guid, path);
                }

                time = (Time.realtimeSinceStartup - time) * 1000;
                CustomDebug.Log("GUIDMapper.Parse by : " + time.ToString("f10") + " miliseconds for " + guidMap.Keys.Count);
            }
            else
            {
                CustomDebug.LogError("GUIDMapper.Error can't find file : " + filePath);
            }
            isParse = true;
        }
    }
    #endif

    #endregion


    #region Editor

    #if UNITY_EDITOR
    [MenuItem("Assets/CollectGUIDs", false, 10001)]
    static void SvnAdd()
    {
        WriteGUIDs();
    }

    [PreProcessBuild(1)]
    static void AddGUIDsToBuild(BuildTarget target)
    {
        WriteGUIDs();
    }

    static void WriteGUIDs()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        if (Directory.Exists(Application.streamingAssetsPath))
        {
            string filePath = Application.streamingAssetsPath + "/guids";
            string[] paths = AssetDatabase.GetAllAssetPaths();

            StringBuilder sb = new StringBuilder();
            for (int i = 0, pathsLength = paths.Length; i < pathsLength; i++)
            {
                var path = paths[i];
                if (
                    path.StartsWith("Assets/", System.StringComparison.Ordinal) &&
                    !path.Contains("Editor") &&
                    !path.Contains(".cs") &&
                    (
                        path.Contains("Resources") ||
                        path.Contains("StreamingAssets")
                    ))
                {
                    sb.Append(AssetDatabase.AssetPathToGUID(path));
                    sb.AppendLine(path);
                }
            }

            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            System.IO.File.WriteAllText(file.FullName, sb.ToString());
        }   
    }
    #endif    

    #endregion
}
