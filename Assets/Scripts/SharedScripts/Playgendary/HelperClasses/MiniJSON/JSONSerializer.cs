using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace MiniJSON 
{

	sealed class JSONSerializer 
	{
        public static bool SerializeEnumAsString
        {
            get;
            set;
        }

		StringBuilder builder;

		JSONSerializer() {
			builder = new StringBuilder();
		}

		public static string Serialize(object obj) {
			var instance = new JSONSerializer();

			instance.SerializeValue(obj);
            SerializeEnumAsString = false;

			return instance.builder.ToString();
		}

		void SerializeValue(object value) 
		{
			IList asList;
			IDictionary asDict;
			string asStr;

			if (value != null) 
			{
				Type valueType = value.GetType();
				if(Json.JsonConverters.ContainsKey(valueType))
				{
					object jsonSerializedValue = Json.JsonConverters[valueType].Serialize(value);
					SerializeValue(jsonSerializedValue);
					return;
				}
			}
			if (value == null) {
				builder.Append("null");
			} else if ((asStr = value as string) != null) {
				SerializeString(asStr);
			} else if (value is bool) {
				builder.Append((bool) value ? "true" : "false");
			} else if ((asList = value as IList) != null) {
				SerializeArray(asList);
			} else if ((asDict = value as IDictionary) != null) {
				SerializeDictionary(asDict);
			} else if (value is char) {
				SerializeString(new string((char) value, 1));
			} 
			else {
				SerializeOther(value);
			}
		}


		void SerializeDictionary(IDictionary obj) 
		{
			bool first = true;

			builder.Append('{');

			foreach (object e in obj.Keys) {
				if (!first) {
					builder.Append(',');
				}

				SerializeValue(e);
				builder.Append(':');

				SerializeValue(obj[e]);

				first = false;
			}

			builder.Append('}');
		}


		void SerializeArray(IList anArray)
		{
			builder.Append('[');

			bool first = true;

			foreach (object obj in anArray) 
			{
				if (!first) 
				{
					builder.Append(',');
				}

				SerializeValue(obj);

				first = false;
			}

			builder.Append(']');
		}


		void SerializeString(string str) {
			builder.Append('\"');

			char[] charArray = str.ToCharArray();
			foreach (var c in charArray) {
				switch (c) {
					case '"':
						builder.Append("\\\"");
						break;
					case '\\':
						builder.Append("\\\\");
						break;
					case '\b':
						builder.Append("\\b");
						break;
					case '\f':
						builder.Append("\\f");
						break;
					case '\n':
						builder.Append("\\n");
						break;
					case '\r':
						builder.Append("\\r");
						break;
					case '\t':
						builder.Append("\\t");
						break;
					default:
						int codepoint = Convert.ToInt32(c);
						if ((codepoint >= 32) && (codepoint <= 126)) {
							builder.Append(c);
						} else {
							builder.Append("\\u");
							builder.Append(codepoint.ToString("x4"));
						}
						break;
				}
			}

			builder.Append('\"');
		}

		void SerializeOther(object value) {
			// NOTE: decimals lose precision during serialization.
			// They always have, I'm just letting you know.
			// Previously floats and doubles lost precision too.
			if (value is float) 
			{
				builder.Append(((float) value).ToString("R"));
			} 
			else if (value is int
				|| value is uint
				|| value is long
				|| value is sbyte
				|| value is byte
				|| value is short
				|| value is ushort
				|| value is ulong) 
			{
				builder.Append(value);
			} 
			else if (value is double
				|| value is decimal) 
			{
				builder.Append(Convert.ToDouble(value).ToString("R"));
			}
			else if (value is Enum) 
			{
                if (SerializeEnumAsString)
                {
                    SerializeString(value.ToString());
                }
                else
                {
                    builder.Append((int)value);
                }
			} 
			else if (value is System.Guid)
			{
				SerializeString(value.ToString());
			}
			else if (value.GetType().IsClass || value.GetType().IsValueType)
			{
				SerializeClass(value);
			}
			else 
			{
				SerializeString(value.ToString());
			}
		}


		void SerializeClass(object value)
		{
			Type serializedType = value.GetType();
			System.Reflection.FieldInfo[] fields = serializedType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

			IDictionary<string, object> keyPair = new Dictionary<string, object>();
			foreach(System.Reflection.FieldInfo prop in fields)
			{
				if(prop.GetValue(value) != null)
				{
					keyPair.Add(prop.Name, prop.GetValue(value));
				}
			}

			SerializeDictionary((IDictionary)keyPair);
		}
	}
}