using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Serialization
{
	public static class SerializeUtility
	{
		public static FieldInfo GetDeepField(System.Type type, string name, BindingFlags flags)
		{
			FieldInfo field = null;

			while(field == null && type != null)
			{
				field = type.GetField(name, flags);
				type = type.BaseType;
			}

			return field;
		}


		public static FieldInfo[] GetDeepFields(System.Type type, BindingFlags flags)
		{
			List<FieldInfo> fields = new List<FieldInfo>();

			while(type != null)
			{
				FieldInfo[] typeFields = type.GetFields(flags);

				foreach(var field in typeFields)
				{
					if(!field.IsNotSerialized)
					{
						fields.Add(field);
					}
				}

				type = type.BaseType;
			}

			return fields.ToArray();
		}



		public static Dictionary<string, object> FindFields(Object unityObject, System.Type attribute = null)
		{
			Dictionary<string, object> fields = new Dictionary<string, object>();

			FieldInfo[] typeFields = GetDeepFields(unityObject.GetType(), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (var field in typeFields) 
			{
				if(
					field != null && 
					(attribute != null && field.IsDefined(attribute, true))
				)
				{
					object value = field.GetValue(unityObject);
					fields.Add(field.Name, value);
				}
			}

//			UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(unityObject);
//			var sp = so.GetIterator();
//			bool child = true;
//			while (sp.NextVisible(child))
//			{
//				string fieldName = sp.propertyPath;
//				FieldInfo field = GetDeepField(unityObject.GetType(), fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
//
//				Debug.Log("FindFields : " + fieldName);
//
//				if(
//					field != null && 
//					(attribute != null && field.IsDefined(attribute, true))
//				)
//				{
//					object value = field.GetValue(unityObject);
//					fields.Add(fieldName, value);
//				}
//
//				child = false;
//			}

			return fields;
		}


		public static List<SerializedField> DiffFields(Object original, Object instance, System.Type attribute = null)
		{
			List<SerializedField> overrideFields = new List<SerializedField>();

			Dictionary<string, object> dict = Serialization.SerializeUtility.FindFields(instance, attribute);
			Dictionary<string, object> dict2 = Serialization.SerializeUtility.FindFields(original, attribute);
	
			foreach(var pair in dict)
			{
				if(dict2.ContainsKey(pair.Key))
				{
					string ser1 = JsonConvert.SerializeObject(pair.Value);
					string ser2 = JsonConvert.SerializeObject(dict2[pair.Key]);

					if(ser1 != ser2)
					{
						Serialization.SerializedField field = new Serialization.SerializedField();
						field.field = pair.Key;
						field.jsonValue = MiniJSON.Json.Serialize(pair.Value);
						overrideFields.Add(field);

						CustomDebug.Log(pair.Key + " : " + pair.Value);
					}
				}
			}

			return overrideFields;
		}
	}
}