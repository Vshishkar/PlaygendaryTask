using UnityEngine;
using System.Collections.Generic;


namespace MiniJSON 
{
	public class JSONDateTimeConverter : JSONCustomConverter
	{
		internal static readonly long EPOX_TICKS = 621355968000000000;


		#region implemented abstract members of JSONCustomConverter

		public override bool IsCanBeDeserialized(string type)
		{
			return type.Equals("DateTime");
		}


		public override bool IsCanBeDeserialized(System.Type type)
		{
			return type.Equals(typeof(System.DateTime));
		}


		public override object Serialize(object obj)
		{
			System.DateTime time = (System.DateTime)obj;
			string dateTime = time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

			var dict = new Dictionary<string, string>();
			dict.Add("__type", "DateTime");
			dict.Add("iso", dateTime);

			return dict;
		}


		public override object Deserialize(object obj)
		{
			var dict = obj as Dictionary<string, object>;
			string dateTime = dict["iso"] as string;

			return System.DateTime.Parse(dateTime);
		}

		#endregion
	}
}
