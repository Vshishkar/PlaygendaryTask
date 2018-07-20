using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public static class DeviceInfo
{
	public static string UUID
	{
		get { return SystemInfo.deviceUniqueIdentifier; }
	}


	public enum PerformanceClass
	{
		TooLow = 0,
		iPhone4 = 1,
		iPhone4S = 2,
		iPad3 = 3,
		iPad4 = 4,
		iPhone5 = 5,
		High = 6,
	}

	#if UNITY_ANDROID
	public const int LIMIT_PROCESSOR_MHZ = 1400;
	public const int LIMIT_RAM_MB = 1000;
	public const int LIMIT_PROCESSOR_CORES = 4;
	#endif

	public static PerformanceClass CurrentClass
	{
		get
		{
			PerformanceClass result = PerformanceClass.High;

			#if UNITY_ANDROID
			if(SystemInfo.processorFrequency < LIMIT_PROCESSOR_MHZ || SystemInfo.systemMemorySize < LIMIT_RAM_MB || SystemInfo.processorCount < LIMIT_PROCESSOR_CORES)
			{
				result = PerformanceClass.TooLow;
			}
			#endif

			#if UNITY_IOS
            #if UNITY_5_6_OR_NEWER
			switch (UnityEngine.iOS.Device.generation)
			{
			case UnityEngine.iOS.DeviceGeneration.iPhone:
			case UnityEngine.iOS.DeviceGeneration.iPhone3G:
			case UnityEngine.iOS.DeviceGeneration.iPhone3GS:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
#else
			switch (iPhone.generation)
			{
			case iPhoneGeneration.iPhone:
			case iPhoneGeneration.iPhone3G:
			case iPhoneGeneration.iPhone3GS:
			case iPhoneGeneration.iPodTouch1Gen:
			case iPhoneGeneration.iPodTouch2Gen:
			case iPhoneGeneration.iPodTouch3Gen:
			case iPhoneGeneration.iPodTouch4Gen:
			case iPhoneGeneration.iPad1Gen:
#endif
				result = PerformanceClass.TooLow;
				break;
            #if UNITY_5_6_OR_NEWER
			case UnityEngine.iOS.DeviceGeneration.iPhone4:
            #else
			case iPhoneGeneration.iPhone4:
            #endif
				result = PerformanceClass.iPhone4;
				break;
            #if UNITY_5_6_OR_NEWER
			case UnityEngine.iOS.DeviceGeneration.iPhone4S:
			case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
			case UnityEngine.iOS.DeviceGeneration.iPad2Gen:
			case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:
            #else
			case iPhoneGeneration.iPhone4S:
			case iPhoneGeneration.iPodTouch5Gen:
			case iPhoneGeneration.iPad2Gen:
			case iPhoneGeneration.iPadMini1Gen:
            #endif
				result = PerformanceClass.iPhone4S;
				break;
            #if UNITY_5_6_OR_NEWER
			case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
            #else
			case iPhoneGeneration.iPad3Gen:
            #endif
				result = PerformanceClass.iPad3;
				break;
            #if UNITY_5_6_OR_NEWER
			case UnityEngine.iOS.DeviceGeneration.iPhone5:
			case UnityEngine.iOS.DeviceGeneration.iPhone5C:
            #else
			case iPhoneGeneration.iPhone5:
			case iPhoneGeneration.iPhone5C:
            #endif
				result = PerformanceClass.iPhone5;
				break;
            #if UNITY_5_6_OR_NEWER
			case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
            #else
			case iPhoneGeneration.iPad4Gen:
            #endif
				result = PerformanceClass.iPad4;
				break;
			}
            #endif

			return result;
		}
	}
}
