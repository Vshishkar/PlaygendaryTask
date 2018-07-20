using System.Collections.Generic;

public static class ListQueueExtension
{
	public static T Dequeue<T>(this List<T> list)
	{
		T temp = list[0];
		list.RemoveAt(0);
		return temp;
	}
	
	
	public static void Enqueue<T>(this List<T> list, T obj)
	{
		list.Add(obj);
	}
	
	
	public static T Peek<T>(this List<T> list)
	{
		return list[0];
	}
}