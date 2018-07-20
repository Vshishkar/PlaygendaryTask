using UnityEngine;
using System;

public class GUIOrientationManger : SingletonMonoBehaviour<GUIOrientationManger> 
{
	#region Variables

    public static event Action<GUILayouterRotationType> OnDeviceOrientationChanged;
	GUILayouterRotationType currentRotationType = GUILayouterRotationType.Both;

    DeviceOrientation deviceOrientation = DeviceOrientation.Unknown;
	ScreenOrientation screenOrientation = ScreenOrientation.Unknown;

	#endregion


	#region Properties

	public GUILayouterRotationType CurrentRotationType
	{
		get { return currentRotationType; }
	}

    
	#endregion


	#region Unity lifecycles

	protected override void Awake()
	{
        base.Awake();
		if (Screen.height > Screen.width)
		{
			RotateToPortrait();
//			currentRotationType = GUILayouterRotationType.Portrait;
		}
		else
		{
			RotateToLandscape();
//			currentRotationType = GUILayouterRotationType.Landscape;
		}
	}


	void Update()
	{
		#if UNITY_EDITOR
		UpdateEditor();
		#else
		UpdateGame();
		#endif
	}

	#endregion


	#region Private methods

	void UpdateEditor()
	{
		if (Screen.height > Screen.width)
		{
			if ((screenOrientation != ScreenOrientation.Portrait) &&
				(screenOrientation != ScreenOrientation.PortraitUpsideDown))
			{
				screenOrientation = ScreenOrientation.Portrait;
				RotateToPortrait();
			}
		}
		else
		{
			if ((screenOrientation != ScreenOrientation.LandscapeLeft) &&
				(screenOrientation != ScreenOrientation.LandscapeRight))
			{
				screenOrientation = ScreenOrientation.LandscapeRight;
				RotateToLandscape();
			}
		}
	}


	void UpdateGame()
	{
		if ((Screen.autorotateToLandscapeLeft) ||
			(Screen.autorotateToLandscapeRight) ||
			(Screen.autorotateToPortrait) ||
			(Screen.autorotateToPortraitUpsideDown))
		{
			DeviceOrientation device = Input.deviceOrientation;

			if (device == deviceOrientation)
			{
				return;
			}

			deviceOrientation = device;       

			if (((deviceOrientation == DeviceOrientation.LandscapeLeft) && (Screen.autorotateToLandscapeLeft)) ||
				((deviceOrientation == DeviceOrientation.LandscapeRight) && (Screen.autorotateToLandscapeRight)))
			{
				RotateToLandscape();
			}
			else if (((deviceOrientation == DeviceOrientation.Portrait) && (Screen.autorotateToPortrait)) ||
				((deviceOrientation == DeviceOrientation.PortraitUpsideDown) && (Screen.autorotateToPortraitUpsideDown)))
			{
				RotateToPortrait();
			}
		}
		else if (screenOrientation != Screen.orientation)
		{
			// Init orientation
			screenOrientation = Screen.orientation;
			if ((screenOrientation == ScreenOrientation.LandscapeLeft) ||
				(screenOrientation == ScreenOrientation.LandscapeRight) || 
				(screenOrientation == ScreenOrientation.Landscape))
			{
				RotateToLandscape();
			}
			else if ((screenOrientation == ScreenOrientation.Portrait) ||
				(screenOrientation == ScreenOrientation.PortraitUpsideDown))
			{
				RotateToPortrait();
			}
		}
	}


	public void RotateToLandscape()
	{
		int height = SizeHelperSettings.Instance.baseHeight;
		int width = SizeHelperSettings.Instance.baseWidth;
		SizeHelperSettings.Instance.baseHeight = height > width ? width : height;
		SizeHelperSettings.Instance.baseWidth = height > width ? height : width;

		currentRotationType = GUILayouterRotationType.Landscape;

		if (OnDeviceOrientationChanged != null)
		{
			OnDeviceOrientationChanged(currentRotationType);
		}
	}
        

	public void RotateToPortrait()
	{
		int height = SizeHelperSettings.Instance.baseHeight;
		int width = SizeHelperSettings.Instance.baseWidth;
		SizeHelperSettings.Instance.baseWidth = height > width ? width : height;
		SizeHelperSettings.Instance.baseHeight = height > width ? height : width;

		currentRotationType = GUILayouterRotationType.Portrait;

		if (OnDeviceOrientationChanged != null)
		{
			OnDeviceOrientationChanged(currentRotationType);
		}
	}

	#endregion
}
