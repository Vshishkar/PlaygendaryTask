using UnityEngine;
using System.Collections.Generic;


namespace MiniJSON 
{
	public class JSONKeyFrameConverter : JSONCustomConverter 
	{
		#region implemented abstract members of JSONCustomConverter

		public override bool IsCanBeDeserialized(string type)
		{
			return type.Equals("Keyframe");
		}


		public override bool IsCanBeDeserialized(System.Type type)
		{
			return type.Equals(typeof(Keyframe));
		}


		public override object Serialize(object obj)
		{
			Keyframe frame = (Keyframe)obj;
			var dict = new Dictionary<string, object>();

			dict.Add("inTangent", frame.inTangent);
			dict.Add("outTangent", frame.outTangent);
			dict.Add("tangentMode", frame.tangentMode);
			dict.Add("time", frame.time);
			dict.Add("value", frame.value);

			return dict;
		}


		public override object Deserialize(object obj)
		{
			Dictionary<string, object> dict = obj as Dictionary<string, object>;

			Keyframe key = new Keyframe();
			key.inTangent = System.Convert.ToSingle(dict["inTangent"]);
			key.outTangent = System.Convert.ToSingle(dict["outTangent"]);
			key.tangentMode = System.Convert.ToInt32(dict["tangentMode"]);
			key.time = System.Convert.ToSingle(dict["time"]);
			key.value = System.Convert.ToSingle(dict["value"]);

			return key;
		}

		#endregion


	}
}