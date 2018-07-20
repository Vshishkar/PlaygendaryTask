using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer (typeof(TweenData))]
//public class TweenDataDrawer : PropertyDrawer {
//
//	const int curveWidth = 50;
//	const float min = 0;
//	const float max = 1;
//
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//		SerializedProperty tween = property.FindPropertyRelative("tween");
//		Object lastObject = tween.objectReferenceValue;
//		EditorGUI.PropertyField(position, tween, new GUIContent(tween.name));
//		SerializedProperty newTween = property.FindPropertyRelative("newtTween");
//		if (lastObject != tween.objectReferenceValue) {
//			Debug.Log("change");
//			newTween.objectReferenceValue = tween.objectReferenceValue;
//			tween.objectReferenceValue = lastObject;
//		}
//		if (newTween.objectReferenceValue != null) {
//			MonoBehaviour[] components = (newTween.objectReferenceValue as MonoBehaviour).gameObject.GetComponents<MonoBehaviour>();
//			if (components.Length > 1) {
//				for (int j = 0; j < components.Length; j++) {
//					if (EditorTools.DrawButton("Set " + components[j].GetType().ToString() + " script")) {
//						tween.objectReferenceValue = components[j];
//						newTween.objectReferenceValue = null;
//					}
//				}
//				if (EditorTools.DrawButton("Undo")) newTween.objectReferenceValue = null;
//				EditorGUILayout.Space();
//			} else {
//				tween.objectReferenceValue = components[0];
//				newTween.objectReferenceValue = null;
//			}
//		}
//
//
//
//		//EditorGUI.PropertyField(position, newTween, new GUIContent(newTween.name));
//
//		//	// draw buttons
//		//}
//
//
//		SerializedProperty delay = property.FindPropertyRelative("delay");
//		SerializedProperty random = property.FindPropertyRelative("random");
//		SerializedProperty randomMin = property.FindPropertyRelative("randomMin");
//		SerializedProperty randomMax = property.FindPropertyRelative("randomMax");
//
//
//
//
//
////		SerializedProperty scale = property.FindPropertyRelative("scale");
//		SerializedProperty curve = property.FindPropertyRelative("curve");
//
//		// Draw scale
//		EditorGUI.Slider(new Rect(position.x, position.y, position.width - curveWidth, position.height), scale, min, max, label);
//
//		// Draw curve
//		int indent = EditorGUI.indentLevel;
//		EditorGUI.indentLevel = 0;
//		EditorGUI.PropertyField(new Rect(position.width - curveWidth, position.y, curveWidth, position.height), curve, GUIContent.none);
//		EditorGUI.indentLevel = indent;
//	}
//}