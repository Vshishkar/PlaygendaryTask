using UnityEngine;
using System;
using System.Collections.Generic;


namespace Serialization
{
	[System.Serializable]
	public class SerializedValue
	{
		public bool boolValue;
		public int intValue;
		public float floatValue;
		public string stringValue;
		public UnityEngine.Object objectValue;
		public Vector2 vector2Value;
		public Vector3 vector3Value;

		public SerializedType parameterType;
		public string parameterName;


		static Dictionary<Type, SerializedType> typeDict = new Dictionary<Type, SerializedType>
		{
			{typeof(bool), SerializedType.Boolean},
			{typeof(int), SerializedType.Integer},
			{typeof(float), SerializedType.Float},
			{typeof(string), SerializedType.String},
			{typeof(UnityEngine.Object), SerializedType.ObjectReference},
			{typeof(Vector2), SerializedType.Vector2},
			{typeof(Vector3), SerializedType.Vector3},
			{typeof(Enum), SerializedType.Integer},
		};


		public SerializedValue()
		{
			
		}


		public SerializedValue(object value, string name)
		{
			parameterType = typeDict[value.GetType()];
			parameterName = name;
			Value = value;
		}


		public SerializedValue(Type pType, string name)
		{
			parameterType = typeDict[pType];
			parameterName = name;
		}


		public void SetParameterType(Type pType)
		{
			parameterType = typeDict[pType];
		}


		public object Value
		{
			get
			{
				switch (parameterType) {
					case SerializedType.Boolean :
						return boolValue;
					case SerializedType.Integer :
						return intValue;
					case SerializedType.Float :
						return floatValue;
					case SerializedType.String :
						return stringValue;
					case SerializedType.ObjectReference :
						return objectValue;
					case SerializedType.Vector2 :
						return vector2Value;
					case SerializedType.Vector3 :
						return vector3Value;
				}

				return null;
			}
			set
			{
				switch (parameterType) 
				{
					case SerializedType.Boolean :
						boolValue = (bool)value;
						break;
					case SerializedType.Integer :
						intValue = (int)value;
						break;
					case SerializedType.Float :
						floatValue = (float)value;
						break;
					case SerializedType.String :
						stringValue = (string)value;
						break;
					case SerializedType.ObjectReference :
						objectValue = (UnityEngine.Object)value;
						break;
					case SerializedType.Vector2 :
						vector2Value = (Vector2)value;
						break;
					case SerializedType.Vector3 :
						vector3Value = (Vector3)value;
						break;
				}
			}
		}
	}
}