using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System.Collections;


public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
{
    #region Variables

    static T cachedInstance;

    #endregion


    #region Properties

	protected static string FileName
	{
		get { return typeof(T).Name; }
	}


	public static T Instance
	{
		get
		{
			if (cachedInstance == null)
			{
                cachedInstance = Resources.Load(FileName) as T;
			}

			#if UNITY_EDITOR
			if (cachedInstance == null)
			{
				cachedInstance = CreateAndSave();
			}
			#endif

			if (cachedInstance == null)
			{
                CustomDebug.LogWarning("No instance of " + FileName + " found, using default values");
				cachedInstance = ScriptableObject.CreateInstance<T>();
			}

			return cachedInstance;
		}
	}   


	public static bool DoesInstanceExist
	{
        get 
        {
            if (cachedInstance == null)
            {
                cachedInstance = Resources.Load(FileName) as T;
            }

            return cachedInstance != null;
        }
	}

    #endregion


	#if UNITY_EDITOR

	static T CreateAndSave()
	{
        T instance = ScriptableObject.CreateInstance<T>();

        //Saving during Awake() will crash Unity, delay saving until next editor frame
        //Saving during Build will call PreProcessBuildAttribute
        if ((EditorApplication.isPlayingOrWillChangePlaymode) || 
            (BuildPipeline.isBuildingPlayer) || (EditorApplication.isCompiling))
		{
            EditorApplication.delayCall += () =>
            {
                instance = ScriptableObject.CreateInstance<T>();
                SaveAsset(instance); 
            };
		}
		else
		{
            SaveAsset(instance);
		}
		return instance;
	}

	static void SaveAsset(T obj)
	{
        string defaultAssetPath = "Assets/Resources/" + FileName + ".asset";
        string dirName = Path.GetDirectoryName(defaultAssetPath);
		if(!Directory.Exists(dirName))
		{
			Directory.CreateDirectory(dirName);
		}
        AssetDatabase.CreateAsset(obj, defaultAssetPath);
		AssetDatabase.SaveAssets();

        CustomDebug.Log("Saved " + FileName + " instance");
	}

	#endif
}
