using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor {

	static Vector3 bufferPosition;
	static Vector3 bufferRotation;
	static Vector3 bufferScale;
	static bool useBuffer;
	
	public override void OnInspectorGUI() {
		var trans = target as Transform;
		Vector3 pos, rot, scale;
		EditorGUILayout.PrefixLabel("Position");
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("P", "Reset position", IsResetPositionValid(trans), 20f)) {
			EditorTools.RegisterUndo("Reset Position", trans);
			trans.localPosition = Vector3.zero;
		}
		EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 15f;
		pos = EditorTools.DrawVector3(trans.localPosition);
		EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0f;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.PrefixLabel("Rotation");
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset rotation", IsResetRotationValid(trans), 20f)) {
			EditorTools.RegisterUndo("Reset Rotation", trans);
			trans.localEulerAngles = Vector3.zero;
		}
		EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 15f;
		rot = EditorTools.DrawVector3(trans.localEulerAngles);
		EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0f;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.PrefixLabel("Scale");
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("S", "Reset scale", IsResetScaleValid(trans), 20f)) {
			EditorTools.RegisterUndo("Reset Scale", trans);
			trans.localScale = Vector3.one;
		}
		EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 15f;
		scale = EditorTools.DrawVector3(trans.localScale);
		EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0f;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		useBuffer = EditorTools.DrawToggle(useBuffer, string.Empty, "Use buffer for copy and paste transform values ", true, 20f);
		if (EditorTools.DrawButton("Copy to buffer", useBuffer)) {
			bufferPosition = trans.localPosition;
			bufferRotation = trans.localEulerAngles;
			bufferScale = trans.localScale;
		}
		if (EditorTools.DrawButton("Paste from buffer", useBuffer)) {
			pos = bufferPosition;
			rot = bufferRotation;
			scale = bufferScale;
		}
		EditorGUILayout.EndHorizontal();
		if (GUI.changed) {
			EditorTools.RegisterUndo("Transform Change", trans);
			trans.localPosition	= EditorTools.Validate(pos);
			trans.localEulerAngles = EditorTools.Validate(rot);
			trans.localScale = EditorTools.Validate(scale);
		}
	}

	bool IsResetPositionValid(Transform targetTransform) {
		Vector3 v = targetTransform.localPosition;
		return (v.x != 0f) || (v.y != 0f) || (v.z != 0f);
	}

	bool IsResetRotationValid(Transform targetTransform) {
		Vector3 v = targetTransform.localEulerAngles;
		return (v.x != 0f) || (v.y != 0f) || (v.z != 0f);
	}

	bool IsResetScaleValid(Transform targetTransform) {
		Vector3 v = targetTransform.localScale;
		return (v.x != 1f) || (v.y != 1f) || (v.z != 1f);
	}
}