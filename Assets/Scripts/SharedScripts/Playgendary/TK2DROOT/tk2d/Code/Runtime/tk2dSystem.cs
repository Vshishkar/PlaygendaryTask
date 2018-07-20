using UnityEngine;
using System.Collections;

[System.Serializable]
public class tk2dResourceTocEntry
{
	public string resourceGUID = "";
	public string assetName = "";
	public string assetGUID = "";
}

[System.Serializable]
public class tk2dAssetPlatform
{
	public tk2dAssetPlatform(string name, float scale) { this.name = name; this.scale = scale; }
	public string name = "";
	public float scale = 1.0f;
}


public class tk2dSystem : ScriptableObject
{
    #region Variables

    // prefix to apply to all guids to avoid matching errors
	public const string guidPrefix = "tk2d/tk2d_";
	public const string assetName = "tk2d/tk2dSystem";
	public const string assetFileName = "tk2dSystem.asset";

    // constants platforms
    public const string platform1X = "1x";
    public const string platform2X = "2x";
    public const string platform4X = "4x";
    const int retinaTreshold = 1900;

    // instance
    static tk2dSystem _inst = null;

    // platforms
    [SerializeField]
    string usePlatform = platform2X;
    [SerializeField]
    bool autoDetectPlatform = true;

    [System.NonSerialized]
    public tk2dAssetPlatform[] assetPlatforms = new tk2dAssetPlatform[] {
        new tk2dAssetPlatform(platform1X, 1.0f),
        new tk2dAssetPlatform(platform2X, 2.0f),
        new tk2dAssetPlatform(platform4X, 4.0f),
    };

    static bool isRetina = false;
    static bool isRetinaInitialized = false;
    static bool currentPlatformInitialized = false;
    static string currentPlatform = platform2X; // Not serialized, this should be set up on wake

    // resources
    [SerializeField]
    tk2dResourceTocEntry[] allResourceEntries = new tk2dResourceTocEntry[0];

    #endregion


    #region Properties

    #region Instance

    public static tk2dSystem inst
    {
        get 
        {
            if (_inst == null)
            {
                // Attempt to load the global instance and create one if it doesn't exist
                _inst = Resources.Load(assetName, typeof(tk2dSystem)) as tk2dSystem;
                if (_inst == null)
                {
                    _inst = ScriptableObject.CreateInstance<tk2dSystem>();
                }
                // We don't want to destroy this throughout the lifetime of the game
                DontDestroyOnLoad(_inst);
            }
            return _inst;
        }
    }


    // Variant which will not create the instance if it doesn't exist
    public static tk2dSystem inst_NoCreate
    {
        get
        {
            if (_inst == null)
                _inst = Resources.Load(assetName, typeof(tk2dSystem)) as tk2dSystem;
            return _inst;
        }
    }

    #endregion


    #region Platforms

    public static bool IsRetina
    {
        get 
        { 
            if (!isRetinaInitialized)
            {
                isRetina = CurrentPlatform.Equals(platform4X);
                isRetinaInitialized = true;
            }
            return isRetina; 
        }
    }


    public static string CurrentPlatform
    {
        get 
        {
            if (!currentPlatformInitialized)
            {
                currentPlatform = platform2X;
                if ((tk2dSystem.inst_NoCreate != null) && (!tk2dSystem.inst_NoCreate.autoDetectPlatform))
                {
                    currentPlatform = tk2dSystem.inst_NoCreate.usePlatform;
                }
                else
                {
                    if (Screen.height > retinaTreshold || Screen.width > retinaTreshold)
                    {
                        currentPlatform = platform4X;
                    }
                    else
                    {
                        currentPlatform = platform2X;
                    }
                }

                #if UNITY_EDITOR
                // Hack, don't have access to editor classes from here
                currentPlatform = UnityEditor.EditorPrefs.GetString("tk2d_platform", platform2X);
                #endif

                currentPlatformInitialized = true;
            }

            return currentPlatform;
        } 
        #if UNITY_EDITOR
        set 
        { 
            if (value != currentPlatform)
            {
                switch (value)
                {
                    case platform1X:
                    case platform2X:
                    case platform4X:
                        currentPlatform = value;
                        break;
                    default:
                        currentPlatform = platform2X;
                        break;
                }
                currentPlatformInitialized = false;
                isRetinaInitialized = false;
            }
        }
        #endif
    }
        
    #if UNITY_EDITOR
    public void SetUsePlatform(string value) 
    { 
        switch (value)
        {
            case platform1X:
            case platform2X:
            case platform4X:
                usePlatform = value;
                break;
            default:
                usePlatform = platform2X;
                break;
        }
    } 
    #endif

    // This is a hack to work around a bug in Unity 4.x
    // Scene serialization will serialize the actively bound texture
    // but not the material during the build, only when [ExecuteInEditMode]
    // is on, eg. on sprites.
    // To work around: Create the file tk2dOverrideBuildMaterial in the project root
    //                 outside Assets before you start the build, and delete it after
    //                 your build is complete.
    public static bool OverrideBuildMaterial 
    {
        get 
        {
            #if UNITY_EDITOR
            return System.IO.File.Exists("tk2dOverrideBuildMaterial");
            #else
            return false;
            #endif
        }
    }

    #endregion

    #endregion


    #region Unity lifecycles

    private tk2dSystem() { }

    #endregion


    #region Public methods

    #region Platforms

    public static tk2dAssetPlatform GetAssetPlatform(string platform)
    {
        tk2dSystem inst = tk2dSystem.inst_NoCreate;
        if (inst == null) return null;

        for (int i = 0; i < inst.assetPlatforms.Length; ++i)
        {
            if (inst.assetPlatforms[i].name == platform)
                return inst.assetPlatforms[i];
        }
        return null;
    }

    #endregion

    #region Resources

    #if UNITY_EDITOR
    public tk2dResourceTocEntry[] Editor__Toc { get { return allResourceEntries; } set { allResourceEntries = value; } }
    #endif

    public static T LoadResourceByGUID<T>(string guid) where T : UnityEngine.Object { return inst.LoadResourceByGUIDImpl<T>(guid); }
    public static T LoadResourceByName<T>(string guid) where T : UnityEngine.Object { return inst.LoadResourceByNameImpl<T>(guid); }

    #endregion

    #endregion


    #region Private methods

	// Loads a resource by GUID
	// Return null if it doesn't exist
	T LoadResourceByGUIDImpl<T>(string guid) where T : UnityEngine.Object
	{
		tk2dResource resource = Resources.Load(guidPrefix + guid, typeof(tk2dResource)) as tk2dResource;
		if (resource != null)
			return resource.objectReference as T;
		else
			return null;
	}

	// Loads a resource by name
	// Returns null if the name can't be found, or load fails for any other reason
	T LoadResourceByNameImpl<T>(string name) where T : UnityEngine.Object
	{
		// TODO: create and use a dictionary
		for (int i = 0; i < allResourceEntries.Length; ++i)
		{
			if (allResourceEntries[i] != null && allResourceEntries[i].assetName == name)
				return LoadResourceByGUIDImpl<T>(allResourceEntries[i].assetGUID);
		}
		return null;
	}
        
    #endregion
}


// A few lines about the need for a resource wrapper
// Moving data objects in and out of resources directories is not practical, and potential for name collision is there.
// Furthermore, the system is lightweight, with one asset for each loadable item. 
// With the decoupling of data & resource objects, it will now be possible to move files in and out of resources for platform 
// specific builds without messing about with actual data objects. It is possible to rebuild all necessary asset files from 
// just the tk2dSystem object, so the guid files can be deleted and reconstructed if necessary.
