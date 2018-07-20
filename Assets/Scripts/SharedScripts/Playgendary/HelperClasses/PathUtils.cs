using UnityEngine;
using System.Collections;

public static class PathUtils 
{
    #region Constants

    public const string CSVExtention = "csv";
    public const string PNGExtention = "png";
    public const string CAFExtention = "caf";

    public const char CHAR_SLASH    = '/';
    public const char CHAR_DOT      = '.';

    #if !UNITY_WSA
    public const string WWW_FILE_PREFIX = "file://";
    #else
    public const string WWW_FILE_PREFIX = "file:///";
    #endif

    public const string WWW_JAR_PREFIX = "jar:";

    const string FORMAT_PATH_ADDITION   = "{0}{1}";
    const string FORMAT_PATH_COMPONENT  = "{0}/{1}";
    const string FORMAT_PATH_EXTENTION  = "{0}.{1}";
    const string FORMAT_PATH_BASHSHELL  = "\"{0}\"";

    #endregion



    #region Unity Utils

    public static string GetDataRoot()
    {
        string result = Application.streamingAssetsPath;

        #if UNITY_EDITOR
        result = Application.streamingAssetsPath.RemoveLastPathComponent();
        #elif UNITY_ANDROID
        result = Application.streamingAssetsPath;
        #elif UNITY_IOS
        result = Application.streamingAssetsPath.RemoveLastPathComponent().RemoveLastPathComponent();
        #else 
        result = Application.streamingAssetsPath.RemoveLastPathComponent();
        #endif

        return result;
    }


    /// <summary>
    /// Appends the StreamingAsset path to file.
    /// </summary>
    public static string GetStreamingAssetsFile(string fileName, string extention = null)
    {
        string result = Application.streamingAssetsPath;
        result = result.AppendPathComponent(fileName);
        if (extention != null)
        {
            result = result.AppendPathExtention(extention);
        }
        return result;
    }


    /// <summary>
    /// Appends the StreamingAsset path to file.
    /// </summary>
    public static string GetStreamingAssetsCSV(string fileName)
    {
        return GetStreamingAssetsFile(fileName, CSVExtention);
    }


    /// <summary>
    /// Appends the StreamingAsset path to file.
    /// </summary>
    public static string GetStreamingAssetsPNG(string fileName)
    {
        return GetStreamingAssetsFile(fileName, PNGExtention);
    }


    /// <summary>
    /// Appends the StreamingAsset path to file.
    /// </summary>
    public static string GetStreamingAssetsCAF(string fileName)
    {
        return GetStreamingAssetsFile(fileName, CAFExtention);
    }

    #endregion  



    #region String Utils

    public static string AppendPathComponent(this string str, string pathComponent)
    {
        return string.Format(FORMAT_PATH_COMPONENT, str, pathComponent);
    }


    public static string AppendPathExtention(this string str, string pathExtention)
    {
        return string.Format(FORMAT_PATH_EXTENTION, str, pathExtention);
    }


    /// <summary>
    /// Appends the WWW prefix for work with WWW class.
    /// </summary>
    public static string AppendWWWFilePrefix(this string str)
    {
        if (!str.Contains(WWW_FILE_PREFIX))
        {
            str = string.Format(FORMAT_PATH_ADDITION, WWW_FILE_PREFIX, str);
        }
        return str;
    }


    /// <summary>
    /// Appends the JAR prefix for StreamingAsset on Android.
    /// </summary>
    public static string AppendWWWJARPrefix(this string str)
    {
        if (!str.Contains(WWW_JAR_PREFIX))
        {
            str = string.Format(FORMAT_PATH_ADDITION, WWW_JAR_PREFIX, str);
        }
        return str;
    }


    public static string RemoveLastPathComponent(this string str)
    {        
        if (str.Length >= 2)
        {
            int idx = str.LastIndexOf(CHAR_SLASH, str.Length - 2);
            if (idx > 0)
            {
               return str.Remove(idx);
            }
        }

        return string.Empty;
    }


    public static string RemovePathExtention(this string str)
    {
        if (str.Length >= 2)
        {
            int idx = str.LastIndexOf(CHAR_DOT, str.Length - 2);
            if (idx > 0)
            {
                return str.Remove(idx);
            }
        }

        return string.Empty;
    }


    public static string ShellString(this string str)
    {
        return string.Format(FORMAT_PATH_BASHSHELL, str);
    }                

    #endregion
}
