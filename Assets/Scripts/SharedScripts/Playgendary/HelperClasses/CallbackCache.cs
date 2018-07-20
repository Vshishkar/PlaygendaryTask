using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CallbackCache <T>
{
	Dictionary<string, List<System.Action<T>>> callbacks = new Dictionary<string, List<System.Action<T>>>();


	public void AddCallback(string key, System.Action<T> callback)
	{
		List<System.Action<T>> curList;
		if (!callbacks.TryGetValue(key, out curList))
		{
			curList = new List<System.Action<T>>();
			callbacks.Add(key, curList);
		}

		if (!curList.Contains(callback))
		{
			curList.Add(callback);
		}
	}


	public bool Call(string key, T arg)
	{
		List<System.Action<T>> curList;
		if (callbacks.TryGetValue(key, out curList))
		{
			foreach (var curCaller in curList)
			{
				curCaller(arg);
			}
			callbacks.Remove(key);

			return true;
		}
		else
		{
			return false;
		}
	}


	public bool DoesCallersExist(string key)
	{
		return callbacks.ContainsKey(key);
	}


	public void RemoveAll(string key)
	{
		callbacks.Remove(key);
	}
}
