using UnityEngine;
using System;

public class EnumArrayAttribute : PropertyAttribute {

	public string PropertyName;
	public Type EnumType;

	public EnumArrayAttribute(string propertyName, Type enumType)
	{
		PropertyName = propertyName;
		EnumType = enumType;
	}
}
