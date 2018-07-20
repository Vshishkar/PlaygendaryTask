using UnityEngine;
using System.Collections.Generic;


namespace MiniJSON 
{
	public class JSONAnimationCurveConverter : JSONCustomConverter 
	{
		#region implemented abstract members of JSONCustomConverter

		public override bool IsCanBeDeserialized(string type)
		{
			return type.Equals("AnimationCurve");
		}


		public override bool IsCanBeDeserialized(System.Type type)
		{
			return type.Equals(typeof(AnimationCurve));
		}


		public override object Serialize(object obj)
		{
			AnimationCurve curve = (AnimationCurve)obj;
			var dict = new Dictionary<string, object>();
			dict.Add("__type", "AnimationCurve");
			dict.Add("keys", curve.keys);

			return dict;
		}


		public override object Deserialize(object obj)
		{
			AnimationCurve curve = new AnimationCurve();
			var dict = obj as Dictionary<string, object>;
			object[] keys = dict["keys"] as object[];

			for (int length = keys.Length, i = 0; i < length; i++) 
			{
				Keyframe frame = (Keyframe)Json.JsonConverters[typeof(Keyframe)].Deserialize(keys[i]);
				curve.AddKey(frame);
			}

			return curve;
		}

		#endregion


	}
}
