using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;


namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(PropertyValueAttribute))]
	public class PropertyValueDrawer : PropertyDrawer  
	{
        PropertyValueAttribute NamedAttribute { get { return ((PropertyValueAttribute)attribute); } }


		public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
		{
            string propertyName = NamedAttribute.PropertyName;
            if (string.IsNullOrEmpty(propertyName))
            {
                char[] chars = _property.name.ToCharArray();
                chars[0] = char.ToUpper(chars[0]);
                propertyName = new string(chars);
            }

            object prevValue = _property.Value();

            SerializedProperty copy = _property.Copy();
            EditorGUI.PropertyField(_position, copy);

            object obj = AttributeUtility.GetParentObjectFromProperty(_property);
            Type containerType = obj.GetType();
            PropertyInfo p = containerType.GetProperty(propertyName);

            this.fieldInfo.SetValue(obj, prevValue);
            p.SetValue(obj, copy.Value(), null);
		}
	}
}
