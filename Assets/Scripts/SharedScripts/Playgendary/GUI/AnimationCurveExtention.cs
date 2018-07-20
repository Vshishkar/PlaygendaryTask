using UnityEngine;
using System;
using System.Collections;

public static class AnimationCurveExtention
{
	public static float RoundTo(float x, float step)
	{
		return Mathf.Round(x / step) * step;
	}


	public static void RoundEdges(this AnimationCurve curve, float startTime = 0.0f, float endTime = 1.0f, float yStep = 0.05f)
	{
		var curKeys = curve.keys;

		if (curKeys.Length >= 2)
		{
			curKeys[0].time = startTime;
			curKeys[0].value = RoundTo(curKeys[0].value, yStep);

			curKeys[curKeys.Length - 1].time = endTime;
			curKeys[curKeys.Length - 1].value = RoundTo(curKeys[curKeys.Length - 1].value, yStep);
		}

		curve.keys = curKeys;
	}
}
