using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;


[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer 
{

	List<string> enumNamesList;
	List<int> enumValuesList;

	public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
	{
		if(enumNamesList == null)
		{
			enumNamesList = new List<string>();
			enumValuesList = new List<int>();

			FieldInfo field = GetField(_property.serializedObject.targetObject.GetType(), _property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			Array enumValues = Enum.GetValues(field.FieldType);

			foreach(object enumValue in enumValues)
			{
				int intValue = (int)enumValue;
				if(intValue > 0)
				{
					enumValuesList.Add(intValue);
					enumNamesList.Add(enumValue.ToString());
				}
			}
		}

        int prevValue = _property.intValue;
		int prevMask = ConvertToMask( prevValue, enumValuesList);
		int newMask = EditorGUI.MaskField( _position, _label, prevMask, enumNamesList.ToArray());
		int newValue = ConvertToValue( newMask, enumValuesList);

		if(newValue != prevValue)
			_property.intValue = newValue;
	}


	FieldInfo GetField(System.Type type, string fieldName, BindingFlags flags)
	{
		FieldInfo field = null;

		while( (field == null) && (type != null) )
		{
			field = type.GetField(fieldName, flags);
			type = type.BaseType;
		}

		return field;
	}


	int ConvertToMask(int value, List<int> possibleValues)
	{
		int mask = 0;

		for (int i = 0; i < possibleValues.Count; i++) 
		{
			if( (value & possibleValues[i]) != 0 )
			{
				mask += (1 << i);
			}
		}

		return mask;
	}



	int ConvertToValue(int mask, List<int> possibleValues)
	{
		int value = 0;

		for (int i = 0; i < 32; i++) 
		{
			int val = 1 << i;

			if( (mask & val) != 0 && i < possibleValues.Count)
			{
				value += possibleValues[i];
			}
		}

		return value;
	}
}
