using System.Reflection;

namespace Serialization
{
	[System.Serializable]
	public class SerializedField 
	{
		public string field;
		public string jsonValue;


		public void ApplyValue(object obj)
		{
			if(obj != null)
			{
				FieldInfo fieldInfo = SerializeUtility.GetDeepField(obj.GetType(), field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if(fieldInfo != null)
				{
					System.Type fieldType = fieldInfo.FieldType;
					object value = MiniJSON.Json.Deserialize(jsonValue, fieldInfo.FieldType);
					
					if (fieldType.IsInstanceOfType(value)) 
					{
						fieldInfo.SetValue(obj, value);
					}
				}
			}
		}
	}
}