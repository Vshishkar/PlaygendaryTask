using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TweenColor))]
public class TweenColorInspector : TweenerInspector {
	
	protected override void CustomInspectorGUI() {
		var tColor = (TweenColor) tween;
		
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("RGBA chanels mask", true, GUILayout.Width(150f));
		Color defaultColor = GUI.backgroundColor;
		GUI.backgroundColor = Color.red;
		tColor.useChanelMask[0] = EditorGUILayout.Toggle(tColor.useChanelMask[0], GUILayout.Width(20f));
		GUI.backgroundColor = Color.green;
		tColor.useChanelMask[1] = EditorGUILayout.Toggle(tColor.useChanelMask[1], GUILayout.Width(20f));
		GUI.backgroundColor = Color.blue;
		tColor.useChanelMask[2] = EditorGUILayout.Toggle(tColor.useChanelMask[2], GUILayout.Width(20f));
		GUI.backgroundColor = Color.white;
		tColor.useChanelMask[3] = EditorGUILayout.Toggle(tColor.useChanelMask[3], GUILayout.Width(20f));
		GUI.backgroundColor = defaultColor;
		EditorGUILayout.EndHorizontal();
        if (tColor.IsBeginStateSet) GUI.contentColor = Color.green;
		EditorGUILayout.LabelField("Begin " + (tColor.IsOnlyAlphaTween ? "alpha" : "color"));
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset alpha value", IsResetColorValid(tColor.beginColor), 20f)) {
			EditorTools.RegisterUndo("Reset alpha value", tColor);
			tColor.beginColor.a = 0f;
		}
		if (tColor.IsOnlyAlphaTween) {
			tColor.beginColor.a = EditorGUILayout.Slider(tColor.beginColor.a, 0f ,1f);
		} else {
			tColor.beginColor = EditorGUILayout.ColorField(tColor.beginColor);
		}
		if (EditorTools.DrawButton("S", ("Set current " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")), IsSetColorValid(tColor.beginColor, tColor.CurrentColor, tColor.IsOnlyAlphaTween), 20f)) {
			EditorTools.RegisterUndo(("Set begin " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")), tColor);
			if (tColor.IsOnlyAlphaTween) {
				tColor.beginColor.a = tColor.CurrentColor.a;
			} else {
				tColor.beginColor = tColor.CurrentColor;
			}
		}
		EditorGUILayout.EndHorizontal();
        if (tColor.IsEndStateSet) GUI.contentColor = Color.green;
		EditorGUILayout.LabelField("End " + (tColor.IsOnlyAlphaTween ? "alpha" : "color"));
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset alpha value", IsResetColorValid(tColor.endColor), 20f)) {
			EditorTools.RegisterUndo("Reset alpha value", tColor);
			tColor.endColor.a = 0f;
		}
		if (tColor.IsOnlyAlphaTween) {
			tColor.endColor.a = EditorGUILayout.Slider(tColor.endColor.a, 0f ,1f);
		} else {
			tColor.endColor = EditorGUILayout.ColorField(tColor.endColor);
		}
		if (EditorTools.DrawButton("S", ("Set current " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")), IsSetColorValid(tColor.endColor, tColor.CurrentColor, tColor.IsOnlyAlphaTween), 20f)) {
			EditorTools.RegisterUndo(("Set end " + (tColor.IsOnlyAlphaTween ? "alpha value" : "color")), tColor);
			if (tColor.IsOnlyAlphaTween) {
				tColor.endColor.a = tColor.CurrentColor.a;
			} else {
				tColor.endColor = tColor.CurrentColor;
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));
		if (EditorTools.DrawButton("R", "Reset target", IsResetTargetValid(tColor), 20f)) {
			EditorTools.RegisterUndo("Reset target", tColor);
			tColor.Target = null;
		}
		tColor.Target = (GameObject) EditorGUILayout.ObjectField(tColor.Target, typeof(GameObject), true);
		EditorGUILayout.EndHorizontal();
	}
	
	bool IsResetColorValid(Color c) {
		return (c.a != 0f);
	}
	
	bool IsSetColorValid(Color cc, Color nc, bool alpha) {
		return !alpha && ((cc.r != nc.r) || (cc.g != nc.g) || (cc.b != nc.b)) || (cc.a != nc.a);
	}
	
	bool IsResetTargetValid(TweenColor t) {
		return t.Target != t.gameObject;
	}
}
