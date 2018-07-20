using UnityEngine;

public sealed class CustomDebug
{
    static public readonly string DEFINE_DEBUG = "DEBUG_TARGET";

    #region Public properties

    public static bool Enable
    {
        get
        {
            #if UNITY_EDITOR || DEBUG_TARGET
            return true;
            #else
            return Debug.isDebugBuild;
            #endif
        }
    }

    #endregion


	#region Public methods

	public static void Log(object message)
    {
        if (Enable) Debug.Log(message);
	}


	public static void Log(object message, UnityEngine.Object context)
	{
        if (Enable) Debug.Log(message, context);
    }


	public static void LogWarning(object message)
	{
        if (Enable) Debug.LogWarning(message);
    }


	public static void LogWarning(object message, UnityEngine.Object context)
	{
        if (Enable) Debug.LogWarning(message, context);
	}


	public static void LogError(object message)
	{
        if (Enable) Debug.LogError(message);
    }
	
	
	public static void LogError(object message, UnityEngine.Object context)
	{
        if (Enable) Debug.LogError(message, context);
    }


    public static void LogFormat(string format, params object[] args)
    {
        if (Enable) Debug.LogFormat(format, args);
    }
        
    #endregion
}