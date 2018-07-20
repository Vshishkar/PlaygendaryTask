using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonAttributeDrawer : PropertyDrawer {
	ButtonAttribute NamedAttribute { get { return ((ButtonAttribute)attribute); } }
	List<string> methods = new List<string> ();


	public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
	{
		object parent = _property.serializedObject.targetObject;
		Type calledType = parent.GetType();
		MethodInfo[] allMethods = calledType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		methods.Clear();
		foreach(MethodInfo mi in allMethods)
		{
			methods.Add(mi.Name);
		}

		Rect popupRect = new Rect(_position.xMin, _position.yMin, 18, _position.height);
		Rect buttonRect = new Rect(_position.xMin + 25, _position.yMin, _position.width - 25, _position.height);

		int index = -1;
		for (int i = 0; i < methods.Count && index == -1; i++)
		{
			if (methods[i].Equals(_property.stringValue))
			{
				index = i;
			}
		}

		index = EditorGUI.Popup(popupRect, "", index, methods.ToArray());
		if(index != -1)
			_property.stringValue = methods[index];

		if(GUI.Button(buttonRect, _property.stringValue))
		{
			var methodInfo = calledType.GetMethod(_property.stringValue, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (methodInfo != null)
			{
				methodInfo.Invoke(parent, null);
			}
		}
	}
}
