using UnityEngine;
using System.Collections;


namespace Serialization
{
	[System.Serializable]
	public class JSONSerializedValue : ISerializationCallbackReceiver 
	{
		[SerializeField] string json;
		[SerializeField] string type;
		object _value;


		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				type = _value.GetType().ToString();
				json = JsonConvert.SerializeObject(Value);
			}
		}


		#region ISerializationCallbackReceiver implementation

		public void OnBeforeSerialize()
		{
			if(Value != null)
			{
				json = JsonConvert.SerializeObject(Value);
			}
		}


		public void OnAfterDeserialize()
		{
			if(string.IsNullOrEmpty(json))
			{
				System.Type t = string.IsNullOrEmpty(type) ? typeof(object) : System.Type.GetType(type);
				Value = JsonConvert.DeserializeObject(json, t);
			}
		}

		#endregion
	}
}