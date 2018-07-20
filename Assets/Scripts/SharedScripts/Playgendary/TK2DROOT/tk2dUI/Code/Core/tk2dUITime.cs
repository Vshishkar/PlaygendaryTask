using UnityEngine;
using System.Collections;

/// <summary>
/// Time proxy class, independent to Time.timeScale
/// </summary>
public static class tk2dUITime {

	/// <summary>
	/// Use this in UI classes / when you need deltaTime unaffected by Time.timeScale
	/// </summary>
	public static float deltaTime 
    {
		get { return _deltaTime; }
	}

    static float _deltaTime;

    static readonly float time_30fps = 1.0f / 30.0f;

	/// <summary>
	/// Do not call. This is updated by tk2dUIManager
	/// </summary>
	public static void Init() 
	{
        _deltaTime = Time.maximumDeltaTime;
	}

	/// <summary>
	/// Do not call. This is updated by tk2dUIManager
	/// </summary>
	public static void Update() 
	{
		if (Time.timeScale < 0.001f) 
        {
            _deltaTime = Mathf.Min( time_30fps, Time.unscaledDeltaTime );
		}
		else 
        {
            _deltaTime = Time.unscaledDeltaTime;
		}
	}
}
