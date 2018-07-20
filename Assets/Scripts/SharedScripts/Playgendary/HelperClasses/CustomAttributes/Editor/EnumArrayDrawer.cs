using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer {

	EnumArrayAttribute NamedAttribute { get { return ((EnumArrayAttribute)attribute); } }


	public override float GetPropertyHeight (SerializedProperty _property, GUIContent label) {

		float size = 16;

		if(_property.isExpanded)
		{
			foreach (SerializedProperty p in _property)
			{
				size += EditorGUI.GetPropertyHeight(p);
			}
		}

		return size;
	}


	Rect rect;

	public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
	{
		_position.height = 16;

		SerializedProperty prop = _property.FindPropertyRelative(NamedAttribute.PropertyName);
		if(prop != null)
			_label.text = prop.enumNames[prop.enumValueIndex];

		_property.isExpanded = EditorGUI.Foldout(_position, _property.isExpanded, _label);
		if(_property.isExpanded)
		{
			rect = new Rect(_position.xMin + 10, _position.yMin, _position.width - 10, 16);

			DrawPropertiesRecursively(rect, _property);
		}
	}


	void DrawPropertiesRecursively(Rect _position, SerializedProperty _property)
	{
		rect = _position;

		foreach (SerializedProperty p in _property)
		{
			float height = EditorGUI.GetPropertyHeight(p);
			rect = new Rect(_position.xMin + 10, rect.yMin + height, _position.width - 10, height);
			EditorGUI.PropertyField(rect, p);

			if(p.hasChildren && p.isExpanded)
				DrawPropertiesRecursively(rect, p);
		}
	}
}
