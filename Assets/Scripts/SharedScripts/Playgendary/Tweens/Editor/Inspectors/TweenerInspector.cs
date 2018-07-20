using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class TweenerInspector : Editor
{

	protected Color defaultContentColor;
	protected Tweener tween;

	float factor = 1f;

	virtual protected void Awake()
	{
		defaultContentColor = GUI.contentColor;
		tween = (Tweener)target;
	}

	override public void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Method", true, GUILayout.Width(150f));
		tween.method = (Method)EditorGUILayout.EnumPopup(tween.method);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Steeper curves", true, GUILayout.Width(150f));
		tween.steeperCurves = EditorGUILayout.Toggle(tween.steeperCurves);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("NotUsedForScreenAnimatingCondition", true, GUILayout.Width(150f));
		tween.NotUsedForScreenAnimatingCondition = EditorGUILayout.Toggle(tween.NotUsedForScreenAnimatingCondition);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Style", true, GUILayout.Width(150f));
		tween.style = (Style)EditorGUILayout.EnumPopup(tween.style);
		EditorGUILayout.EndHorizontal();


        if (TweensCurves.Instance != null) 
        {
            List<string> curves = TweensCurves.Instance.IdList;
            string currentId = tween.AnimationCurveId;
            int index = 0;

            if (curves.Exists (id => id.Equals (currentId))) 
            {
                index = curves.IndexOf (currentId);
            }
            else
            {
                curves.Insert (index, currentId);
            }

            EditorGUILayout.BeginHorizontal ();
            EditorTools.DrawLabel ("Curve ID", true, GUILayout.Width (150f));
            int newIndex = EditorGUILayout.Popup (index, curves.ToArray ());
            EditorGUILayout.EndHorizontal ();

            if (newIndex != index) 
            {
                tween.AnimationCurveId = curves[newIndex];
		    }
        }

		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Curve", true, GUILayout.Width(150f));
		tween.useCurve = EditorTools.DrawToggle(tween.useCurve, string.Empty, "Use curve", true, 15f);
		tween.animationCurve = EditorGUILayout.CurveField(tween.animationCurve);
		tween.animationCurve.RoundEdges();

        if (TweensCurves.Instance != null)
		{
			if (EditorTools.DrawButton("S", "Save at prefab (don't forget apply!)", true, 20f))
			{
				TweensCurves.Instance.AddCurve(tween.AnimationCurveId, tween.animationCurve);
			}
			if (EditorTools.DrawButton("U", "Update curve from prefab", true, 20f))
			{
				tween.AnimationCurveId = tween.AnimationCurveId;
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Ignore time scale", true, GUILayout.Width(150f));
		tween.ignoreTimeScale = EditorGUILayout.Toggle(tween.ignoreTimeScale);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Set Begin State At Start", true, GUILayout.Width(150f));
		tween.shouldSetBeginStateAtStart = EditorGUILayout.Toggle(tween.shouldSetBeginStateAtStart);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Duration", true, GUILayout.Width(150f));
		const float defaultDuration = 1f;
		if (EditorTools.DrawButton(defaultDuration.ToString(), ("Set duration to " + defaultDuration), true, 20f))
		{
			tween.duration = defaultDuration;
		}
		tween.duration = EditorGUILayout.Slider(tween.duration, 0f, 10f);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Scale duration", true, GUILayout.Width(150f));
		const float defaultScaleDuration = 1f;
		if (EditorTools.DrawButton(defaultScaleDuration.ToString(), ("Set scale duration to " + defaultScaleDuration), true, 20f))
		{
			tween.scaleDuration = defaultScaleDuration;
		}
		tween.scaleDuration = EditorGUILayout.Slider(tween.scaleDuration, 0f, 5f);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.PrefixLabel("Call when finished");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Script");
		EditorGUILayout.LabelField("Call method");
		EditorGUILayout.EndHorizontal();
		EditorTools.DrawInvokeData(tween.invokeWhenFinished);
		Color defaultContentColor = GUI.contentColor;
		GUI.contentColor = Color.cyan;
		EditorGUILayout.LabelField("============================================================================================");
		GUI.contentColor = defaultContentColor;
		CustomInspectorGUI();
		GUI.contentColor = Color.cyan;
		EditorGUILayout.LabelField("============================================================================================");
		GUI.contentColor = defaultContentColor;
		if (Application.isPlaying)
		{
			DrawRunButtons(tween);
		}
		else {
			DrawStateSlider(tween);
		}
		EditorGUILayout.Space();
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	protected void DrawRunButtons(Tweener t)
	{
		EditorGUILayout.BeginHorizontal();
		Color defaultColor = GUI.skin.button.normal.textColor;
		GUI.skin.button.normal.textColor = t.IsBeginStateSet ? Color.green : defaultColor;
		if (EditorTools.DrawButton("Begin " + t.GetType().ToString().Replace("Tween", "").ToLower()))
		{
			t.SetBeginState();
		}
		GUI.skin.button.normal.textColor = t.IsEndStateSet ? Color.green : defaultColor;
		if (EditorTools.DrawButton("End " + t.GetType().ToString().Replace("Tween", "").ToLower()))
		{
			t.SetEndState();
		}
		GUI.skin.button.normal.textColor = defaultColor;
		EditorGUILayout.EndHorizontal();
	}

	protected void DrawStateSlider(Tweener t)
	{
		EditorGUILayout.BeginHorizontal();
		string state = (factor == 0f) ? "Begin" : ((factor == 1f) ? "End" : "Middle");
		GUILayout.Label(state, GUILayout.MaxWidth(40f));
		float newFactor = EditorGUILayout.Slider(factor, 0f, 1f);
		if (newFactor != factor)
		{
			factor = newFactor;
			t.Sample(factor, false);
		}
		EditorGUILayout.EndHorizontal();
	}

	abstract protected void CustomInspectorGUI();
}
