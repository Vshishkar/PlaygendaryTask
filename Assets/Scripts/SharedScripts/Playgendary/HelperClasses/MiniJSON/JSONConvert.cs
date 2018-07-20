using UnityEngine;
using System.Collections;


public static class JsonConvert
{
	public static string SerializeObject(object serialize)
	{
		string ser = MiniJSON.Json.Serialize(serialize);
		return ser;
	}


	public static T DeserializeObject<T>(string serialize)
	{
		return MiniJSON.Json.Deserialize<T>(serialize);
	}


	public static object DeserializeObject(string serialize, System.Type type)
	{
		return MiniJSON.Json.Deserialize(serialize, type);
	}
}
