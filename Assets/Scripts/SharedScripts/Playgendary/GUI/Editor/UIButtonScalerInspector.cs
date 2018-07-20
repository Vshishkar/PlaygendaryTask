using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIButtonScaler))]
public class UIButtonScalerInspector : Editor {

	Color defaultContentColor;

    public override void OnInspectorGUI() {
		var buttonScaler = (UIButtonScaler) target;
		defaultContentColor = GUI.contentColor;
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("My UIItem", true, GUILayout.Width(100f));
		if (EditorTools.DrawButton("R", "Reset my UIItem", IsResetMyItemValid(buttonScaler), 20f)) {
			EditorTools.RegisterUndo("Reset my UIItem", buttonScaler);
			buttonScaler.myItem = buttonScaler.GetComponent<tk2dUIItem>();
		}
		buttonScaler.myItem = (tk2dUIItem) EditorGUILayout.ObjectField(buttonScaler.MyItem, typeof(tk2dUIItem), true);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Use OnRelease instead of OnUp", true, GUILayout.Width(180f));
		buttonScaler.useOnReleaseInsteadOfOnUp = EditorGUILayout.Toggle(buttonScaler.useOnReleaseInsteadOfOnUp);
		EditorTools.DrawLabel("Cached up scale on awake", true, GUILayout.Width(150f));
		buttonScaler.cachedUpScaleOnAwake = EditorGUILayout.Toggle(buttonScaler.cachedUpScaleOnAwake);
		EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Ignore TimeScale", true, GUILayout.Width(180f));
        buttonScaler.isIgnoringTimeScale = EditorGUILayout.Toggle(buttonScaler.isIgnoringTimeScale);
        EditorTools.DrawLabel("Scale BoxCollider", true, GUILayout.Width(150f));
        buttonScaler.isScaleBoxCollider = EditorGUILayout.Toggle(buttonScaler.isScaleBoxCollider);
        EditorGUILayout.EndHorizontal();
		GUI.contentColor = Color.cyan;
		EditorGUILayout.LabelField("OnDown section ============================================================================================");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tDuration", true, GUILayout.Width(100f));
		const float defaultDownDuration = 0.1f;
		if (EditorTools.DrawButton(defaultDownDuration.ToString(), ("Set duration to " + defaultDownDuration), true, 30f)) {
			buttonScaler.downDuration = defaultDownDuration;
		}
		buttonScaler.downDuration = EditorGUILayout.Slider(buttonScaler.downDuration, 0f, 1f);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tMethod", true, GUILayout.Width(100f));
		buttonScaler.downMethod = (Method) EditorGUILayout.EnumPopup(buttonScaler.downMethod);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tScale", true, GUILayout.Width(100f));
		if (EditorTools.DrawButton("R", "Reset scale to one", IsResetScaleValid(buttonScaler.downScale), 20f)) {
			EditorTools.RegisterUndo("Reset down scale", buttonScaler);
			buttonScaler.downScale = Vector3.one;
		}
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
		EditorGUIUtility.LookLikeControls(15f, 0);
        #endif
		buttonScaler.downScale = EditorTools.DrawVector3(buttonScaler.downScale);
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif
		if (EditorTools.DrawButton("S", "Set current scale", IsSetScaleValid(buttonScaler.downScale, buttonScaler.CachedTransform.localScale), 20f)) {
			EditorTools.RegisterUndo("Set down scale", buttonScaler);
			buttonScaler.downScale = buttonScaler.CachedTransform.localScale;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tCurve", true, GUILayout.Width(100f));
		buttonScaler.useDownCurve = EditorTools.DrawToggle(buttonScaler.useDownCurve, string.Empty, "Use down curve", true, 15f);
		buttonScaler.downCurve = EditorGUILayout.CurveField(buttonScaler.downCurve);

		EditorGUILayout.EndHorizontal();
		GUI.contentColor = Color.cyan;
		EditorGUILayout.LabelField("OnUp/OnRelease section ====================================================================================");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tDuration", true, GUILayout.Width(100f));
		const float defaultUpDuration = 0.5f;
		if (EditorTools.DrawButton(defaultUpDuration.ToString(), ("Set duration to " + defaultUpDuration), true, 30f)) {
			buttonScaler.upDuration = defaultUpDuration;
		}
		buttonScaler.upDuration = EditorGUILayout.Slider(buttonScaler.upDuration, 0f, 1f);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tMethod", true, GUILayout.Width(100f));
		buttonScaler.upMethod = (Method) EditorGUILayout.EnumPopup(buttonScaler.upMethod);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tScale", true, GUILayout.Width(100f));
		if (EditorTools.DrawButton("R", "Reset scale to one", IsResetScaleValid(buttonScaler.upScale), 20f)) {
			EditorTools.RegisterUndo("Reset up scale", buttonScaler);
			buttonScaler.upScale = Vector3.one;
		}
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(15f, 0);
        #endif
		buttonScaler.upScale = EditorTools.DrawVector3(buttonScaler.upScale);
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif
		if (EditorTools.DrawButton("S", "Set current scale", IsSetScaleValid(buttonScaler.upScale, buttonScaler.CachedTransform.localScale), 20f)) {
			EditorTools.RegisterUndo("Set up scale", buttonScaler);
			buttonScaler.upScale = buttonScaler.CachedTransform.localScale;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("\tCurve", true, GUILayout.Width(100f));
		buttonScaler.useUpCurve = EditorTools.DrawToggle(buttonScaler.useUpCurve, string.Empty, "Use up curve", true, 15f);
		buttonScaler.upCurve = EditorGUILayout.CurveField(buttonScaler.upCurve);
		
		EditorGUILayout.EndHorizontal();
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
    }

	bool IsResetMyItemValid(UIButtonScaler bs) {
		tk2dUIItem item = bs.GetComponent<tk2dUIItem>();
		return (item != null) && (bs.myItem != item);
	}

	bool IsResetScaleValid(Vector3 v) {
		return (v.x != 1f) || (v.y != 1f) || (v.z != 1f);
	}

	bool IsSetScaleValid(Vector3 v, Vector3 cv) {
		return (v.x != cv.x) || (v.y != cv.y) || (v.z != cv.z);
	}
}



