using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerDrawer : PropertyDrawer 
{
	PropertyValueAttribute NamedAttribute { get { return ((PropertyValueAttribute)attribute); } }

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	{
		EditorGUI.LabelField(position, label);

		position.x += EditorGUIUtility.labelWidth;
		position.width -= EditorGUIUtility.labelWidth;

		string[] sortingLayerNames = GetSortingLayerNames();
		int[] sortingLayerIDs = GetSortingLayerIDs();

		int prevValue = property.Value<int>();

		int sortingLayerIndex = Mathf.Max(0, System.Array.IndexOf<int>(sortingLayerIDs, prevValue));
		sortingLayerIndex = EditorGUI.Popup(position, sortingLayerIndex, sortingLayerNames);
		int newValue = sortingLayerIDs[sortingLayerIndex];

		string propertyName = NamedAttribute.PropertyName;
		if (string.IsNullOrEmpty(propertyName))
		{
			property.intValue = newValue;
		}
		else
		{
			object obj = AttributeUtility.GetParentObjectFromProperty(property);

			this.fieldInfo.SetValue(obj, prevValue);

			Type containerType = obj.GetType();
			PropertyInfo p = containerType.GetProperty(propertyName);
			p.SetValue(obj, newValue, null);
		}
	}


	private string[] GetSortingLayerNames()
	{
		SortingLayer[] layers = SortingLayer.layers;
		string[] result = new string[layers.Length];
		for (int i = 0; i < layers.Length; i++)
		{
			result[i] = layers[i].name;
		}
		return result;
	}


	private int[] GetSortingLayerIDs() 
	{
		SortingLayer[] layers = SortingLayer.layers;
		int[] result = new int[layers.Length];
		for (int i = 0; i < layers.Length; i++)
		{
			result[i] = layers[i].id;
		}
		return result;
	}
}

