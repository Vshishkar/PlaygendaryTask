using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VectorCompactAttribute))]
public class VectorCompactDrawer : PropertyDrawer {

	VectorCompactAttribute enumAttribute { get { return ((VectorCompactAttribute)attribute); } }


	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		return 35f;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif

		if(property.type == "Vector2f")
		{
			property.vector2Value = EditorGUI.Vector2Field(position, label.text, property.vector2Value);
		}
		else if(property.type == "Vector3f")
		{
			property.vector3Value = EditorGUI.Vector3Field(position, label.text, property.vector3Value);
		}
	}
}
