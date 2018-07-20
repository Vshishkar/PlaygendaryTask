using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;



public static class AttributeUtility
{
    private static FieldInfo GetFieldInfoFromPropertyPath (Type host, string path, out Type type)
    {
        FieldInfo fieldInfo = null;
        type = host;
        string[] array = path.Split (new char[] {
            '.'
        });

        for (int i = 0; i < array.Length; i++)
        {
            string text = array [i];
            if (i < array.Length - 1 && text == "Array" && array [i + 1].StartsWith ("data[")) 
            {
                if (type.IsArrayOrList ()) 
                {
                    type = type.GetArrayOrListElementType ();
                }
                i++;
            }
            else 
            {
                FieldInfo fieldInfo2 = null;
                Type type2 = type;
                while (fieldInfo2 == null && type2 != null) 
                {
                    fieldInfo2 = type2.GetField (text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    type2 = type2.BaseType;
                }
                if (fieldInfo2 == null) 
                {
                    type = null;
                    return null;
                }
                fieldInfo = fieldInfo2;
                type = fieldInfo.FieldType;
            }
        }
        return fieldInfo;
    }


    public static object GetObjectFromProperty(SerializedProperty _property, int up = 0)
    {
        object current = _property.serializedObject.targetObject;
        string[] fields = _property.propertyPath.Split('.');

        current = GetObjectFromFields(current, fields, fields.Length);
        return current;
    }


    public static object GetParentObjectFromProperty(SerializedProperty _property, int up = 0)
    {
        object current = _property.serializedObject.targetObject;
        string[] fields = _property.propertyPath.Split('.');

        current = GetObjectFromFields(current, fields, fields.Length - 1);
        return current;
    }


    static object GetObjectFromFields(object current, string[] fields, int length)
    {
        for (int i = 0; i < length; i++) 
        {
            string fieldName = fields[i];

            if(fieldName.Equals("Array"))
            {
                fieldName = fields[++i];
                string indexString = fieldName.Substring(5, fieldName.Length - 6);
                int index = int.Parse(indexString);

                System.Type type = current.GetType();
                if(type.IsArray)
                {
                    System.Array array = current as System.Array;
                    current = array.GetValue(index);
                }
                else if( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) 
                {
                    IList list = current as IList;
                    current = list[index];
                }
            }
            else
            {
                FieldInfo field = current.GetType().GetField(fields[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                current = field.GetValue(current);
            }
        }

        return current;
    }


    static bool IsArrayOrList (this Type listType)
    {
        return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition () == typeof(List<>));
    }


    static Type GetArrayOrListElementType (this Type listType)
    {
        if (listType.IsArray) {
            return listType.GetElementType ();
        }
        if (listType.IsGenericType && listType.GetGenericTypeDefinition () == typeof(List<>)) {
            return listType.GetGenericArguments () [0];
        }
        return null;
    }



    public static T Value<T>(this SerializedProperty property)
    {
        Type valueType = typeof(T);

        if (valueType.IsEnum)
            return (T)Enum.ToObject(valueType, property.enumValueIndex);

        if (typeof(Color).IsAssignableFrom(valueType))
            return (T)(object)property.colorValue;
        else if (typeof(LayerMask).IsAssignableFrom(valueType))
            return (T)(object)property.intValue;
        else if (typeof(Vector2).IsAssignableFrom(valueType))
            return (T)(object)property.vector2Value;
        else if (typeof(Vector3).IsAssignableFrom(valueType))
            return (T)(object)property.vector3Value;
        else if (typeof(Rect).IsAssignableFrom(valueType))
            return (T)(object)property.rectValue;
        else if (typeof(AnimationCurve).IsAssignableFrom(valueType))
            return (T)(object)property.animationCurveValue;
        else if (typeof(Bounds).IsAssignableFrom(valueType))
            return (T)(object)property.boundsValue;
        else if (typeof(Quaternion).IsAssignableFrom(valueType))
            return (T)(object)property.quaternionValue;

        if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            return (T)(object)property.objectReferenceValue;

        if (typeof(int).IsAssignableFrom(valueType))
            return (T)(object)property.intValue;
        else if (typeof(bool).IsAssignableFrom(valueType))
            return (T)(object)property.boolValue;
        else if (typeof(float).IsAssignableFrom(valueType))
            return (T)(object)property.floatValue;
        else if (typeof(string).IsAssignableFrom(valueType))
            return (T)(object)property.stringValue;
        else if (typeof(char).IsAssignableFrom(valueType))
            return (T)(object)property.intValue;

        throw new NotImplementedException("Unimplemented propertyType "+property.propertyType+".");
    }


    public static object Value(this SerializedProperty property)
    {
        switch (property.propertyType) {
            case SerializedPropertyType.Integer:
                return property.intValue;
            case SerializedPropertyType.Boolean:
                return property.boolValue;
            case SerializedPropertyType.Float:
                return property.floatValue;
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return property.colorValue;
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return property.intValue;
            case SerializedPropertyType.Enum:
                int enumI = property.enumValueIndex;
                return new KeyValuePair<int, string>(enumI, property.enumNames[enumI]);
            case SerializedPropertyType.Vector2:
                return property.vector2Value;
            case SerializedPropertyType.Vector3:
                return property.vector3Value;
            case SerializedPropertyType.Rect:
                return property.rectValue;
            case SerializedPropertyType.ArraySize:
                return property.intValue;
            case SerializedPropertyType.Character:
                return (char)property.intValue;
            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return property.boundsValue;
            case SerializedPropertyType.Quaternion:
                return property.quaternionValue;

            default:
                throw new NotImplementedException("Unimplemented propertyType "+property.propertyType+".");
        }
    }
}
