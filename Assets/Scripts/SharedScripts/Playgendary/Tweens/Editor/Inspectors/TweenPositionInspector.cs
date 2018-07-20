using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TweenPosition))]
public class TweenPositionInspector : TweenerInspector {
	
	protected override void CustomInspectorGUI() {
		var tPosition = (TweenPosition) tween;
		
        if (tPosition.IsBeginStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorGUILayout.LabelField("Begin position");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset position to zero", IsResetPositionValid(tPosition.beginPosition), 20f)) 
        {
			EditorTools.RegisterUndo("Reset begin position", tPosition);
			tPosition.beginPosition = Vector3.zero;
		}
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(15f, 0);
        #endif

		tPosition.beginPosition = EditorTools.DrawVector3(tPosition.beginPosition);
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif
        if (EditorTools.DrawButton("S", "Set current position", IsSetPositionValid(tPosition.beginPosition, tPosition.CachedTransform.localPosition), 20f)) 
        {
			EditorTools.RegisterUndo("Set begin position", tPosition);
            tPosition.beginPosition = tPosition.CachedTransform.localPosition;
		}
		EditorGUILayout.EndHorizontal();
        if (tPosition.IsEndStateSet) 
        {
			GUI.contentColor = Color.green;
		}

		EditorGUILayout.LabelField("End position");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset position to zero", IsResetPositionValid(tPosition.endPosition), 20f)) {
			EditorTools.RegisterUndo("Reset end position", tPosition);
			tPosition.endPosition = Vector3.zero;
		}
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(15f, 0);
        #endif
		tPosition.endPosition = EditorTools.DrawVector3(tPosition.endPosition);
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif
        if (EditorTools.DrawButton("S", "Set current position", IsSetPositionValid(tPosition.endPosition, tPosition.CachedTransform.localPosition), 20f)) {
			EditorTools.RegisterUndo("Set end position", tPosition);
            tPosition.endPosition = tPosition.CachedTransform.localPosition;
		}
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Need Reclculate Position: ", GUILayout.MaxWidth(150));
        tPosition.needRecalculatePositions = EditorGUILayout.Toggle(tPosition.needRecalculatePositions, GUILayout.MaxWidth(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ignore Positions Recalculations: ", GUILayout.MaxWidth(150));
        tPosition.useAbsolutePosValues = EditorGUILayout.Toggle(tPosition.useAbsolutePosValues, GUILayout.MaxWidth(50));
        EditorGUILayout.EndHorizontal();
	}

	bool IsResetPositionValid(Vector3 v) {
		return (v.x != 0f) || (v.y != 0f) || (v.z != 0f);
	}
	
	bool IsSetPositionValid(Vector3 v, Vector3 cv) {
		return (v.x != cv.x) || (v.y != cv.y) || (v.z != cv.z);
	}
	
}
