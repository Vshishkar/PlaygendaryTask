using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class EditorTools {
	
	static public Vector3 Validate(Vector3 v) {
		v.x = float.IsNaN(v.x) ? 0f : v.x;
		v.y = float.IsNaN(v.y) ? 0f : v.y;
		v.z = float.IsNaN(v.z) ? 0f : v.z;
		return v;
	}
	
	static public void RegisterUndo(string name, params Object[] objects) {
		if ((objects != null) && (objects.Length > 0)) {
			foreach (Object obj in objects) {
				if (obj == null) {
					continue;
				}
				Undo.RecordObject(obj, name);
				EditorUtility.SetDirty(obj);
			}
		} else {
//			Undo.RegisterSceneUndo(name);
		}
	}

	static public Vector3 DrawVector3(Vector3 value) {
		GUILayoutOption opt = GUILayout.MinWidth(30f);
		value.x = EditorGUILayout.FloatField("X", value.x, opt);
		value.y = EditorGUILayout.FloatField("Y", value.y, opt);
		value.z = EditorGUILayout.FloatField("Z", value.z, opt);
		return value;
	}

	static public Vector2 DrawVector2(Vector2 value) {
		GUILayoutOption opt = GUILayout.MinWidth(45f);
		value.x = EditorGUILayout.FloatField("X", value.x, opt);
		value.y = EditorGUILayout.FloatField("Y", value.y, opt);
		return value;
	}
	
	static public float DrawFloatField(float value, string title, string tooltip, bool enabled, params GUILayoutOption[] options) {
		if (enabled) return EditorGUILayout.FloatField(new GUIContent(title, tooltip), value, options);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.FloatField(new GUIContent(title, tooltip), value, options);
		GUI.color = saveColor;
		return value;
	}

	static public float DrawFloatField(float value, string title, bool enabled, params GUILayoutOption[] options) {
		if (enabled) return EditorGUILayout.FloatField(title, value, options);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.FloatField(title, value, options);
		GUI.color = saveColor;
		return value;
	}

	static public float DrawFloatField(float value, string title, bool enabled = true) {
		if (enabled) return EditorGUILayout.FloatField(title, value);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.FloatField(title, value);
		GUI.color = saveColor;
		return value;
	}

	static public System.Enum DrawEnumField(System.Enum value, string title, string tooltip, bool enabled, params GUILayoutOption[] options) {
		if (enabled) return EditorGUILayout.EnumPopup(new GUIContent(title, tooltip), value, options);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.EnumPopup(new GUIContent(title, tooltip), value, options);
		GUI.color = saveColor;
		return value;
	}

	static public System.Enum DrawEnumField(System.Enum value, string title, bool enabled, params GUILayoutOption[] options) {
		if (enabled) return EditorGUILayout.EnumPopup(title, value, options);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.EnumPopup(title, value, options);
		GUI.color = saveColor;
		return value;
	}

	static public System.Enum DrawEnumField(System.Enum value, string title, bool enabled = true) {
		if (enabled) return EditorGUILayout.EnumPopup(title, value);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.EnumPopup(title, value);
		GUI.color = saveColor;
		return value;
	}

	static public bool DrawButton(string title, string tooltip, bool enabled, float width) {
		if (enabled) return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
		GUI.color = saveColor;
		return false;
	}
	
	static public bool DrawButton(string title, bool enabled, float width) {
		if (enabled) return GUILayout.Button(title, GUILayout.Width(width));
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Button(title, GUILayout.Width(width));
		GUI.color = saveColor;
		return false;
	}

	static public bool DrawButton(string title, string tooltip, bool enabled) {
		if (enabled) return GUILayout.Button(new GUIContent(title, tooltip));
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Button(new GUIContent(title, tooltip));
		GUI.color = saveColor;
		return false;
	}
	
	static public bool DrawButton(string title, bool enabled = true) {
		if (enabled) return GUILayout.Button(title);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Button(title);
		GUI.color = saveColor;
		return false;
	}

	static public bool DrawToggle(bool value, string title, string tooltip, bool enabled, float width) {
		if (enabled) return GUILayout.Toggle(value, new GUIContent(title, tooltip), GUILayout.Width(width));
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Toggle(value, new GUIContent(title, tooltip), GUILayout.Width(width));
		GUI.color = saveColor;
		return value;
	}

	static public bool DrawToggle(bool value, string title, bool enabled, float width) {
		if (enabled) return GUILayout.Toggle(value, title, GUILayout.Width(width));
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Toggle(value, title, GUILayout.Width(width));
		GUI.color = saveColor;
		return value;
	}

	static public bool DrawToggle(bool value, string title, bool enabled = true) {
		if (enabled) return GUILayout.Toggle(value, title);
		Color saveColor = GUI.color;
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUILayout.Toggle(value, title);
		GUI.color = saveColor;
		return value;
	}
	
	static public void DrawLabel(string title, string tooltip, bool enabled, params GUILayoutOption[] options) {
		Color saveColor = GUI.color;
		if (!enabled) GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.LabelField(new GUIContent(title, tooltip), options);
		GUI.color = saveColor;
	}

	static public void DrawLabel(string title, bool enabled = true, params GUILayoutOption[] options) {
		Color saveColor = GUI.color;
		if (!enabled) GUI.color = new Color(1f, 1f, 1f, 0.25f);
		EditorGUILayout.LabelField(title, options);
		GUI.color = saveColor;
	}

	static public void DrawInvokeData(InvokeData invoke) {
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset script", (invoke.script != null), 20f)) invoke.script = invoke.newScript = null;
		if (invoke.script != null) {
			string title = invoke.script.GetType().ToString();
			string tooltip = "Set component " + title;
			GUILayout.Label(new GUIContent(title, tooltip), GUILayout.MinWidth(20f), GUILayout.MaxWidth(90f));
		}
		MonoBehaviour newScript = (MonoBehaviour) EditorGUILayout.ObjectField(invoke.script, typeof(MonoBehaviour), true);
		if (newScript != invoke.script) invoke.newScript = newScript;
		invoke.method = EditorGUILayout.TextField(invoke.method);
		EditorGUILayout.EndHorizontal();
		if (invoke.newScript != null) {
			MonoBehaviour[] components = invoke.newScript.GetComponents<MonoBehaviour>();
			if (components.Length > 1) {
				for (int j = 0; j < components.Length; j++) {
					if (EditorTools.DrawButton("Set " + components[j].GetType().ToString() + " script")) {
						invoke.script = components[j];
						invoke.newScript = null;
					}
				}
				if (EditorTools.DrawButton("Undo")) invoke.newScript = null;
				EditorGUILayout.Space();
			} else {
				invoke.script = components[0];
				invoke.newScript = null;
			}
		}
	}
}