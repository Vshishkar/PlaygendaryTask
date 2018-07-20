using UnityEngine;
using System.Collections;

namespace MiniJSON 
{
	public abstract class JSONCustomConverter 
	{
		public abstract bool IsCanBeDeserialized(string type);
		public abstract bool IsCanBeDeserialized(System.Type type);

		public abstract object Serialize(object obj);
		public abstract object Deserialize(object obj);
	}
}
