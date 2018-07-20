using UnityEngine;
using System.Collections.Generic;


public static class ArrayExtention
{
	public static T RandomObject<T>(this IList<T> list) 
	{
		return (list.Count > 0) ? list[Random.Range(0, list.Count)] : default(T);
	}


	public static T LastObject<T>(this IList<T> list) 
	{
		return (list.Count > 0) ? list[list.Count - 1] : default(T);
	}


	public static T FirstObject<T>(this IList<T> list) 
	{
		return (list.Count > 0) ? list[0] : default(T);
	}


	public static List<T> Diff<T>(IList<T> array1, IList<T> array2)
	{
		var subset = new List<T>();

		foreach (T e1 in array1)
		{
			if (!array2.Contains(e1))
			{
				subset.Add(e1);
			}
		}

		foreach (T e2 in array2)
		{
			if (!array1.Contains(e2))
			{
				subset.Add(e2);
			}
		}

		return subset;
	}


    public static bool ArrayEqual<T>(this IList<T> array1, IList<T> array2) where T : Object
    {
        if (array2 == null || array1 == null)
            return false;

        bool equal = (array1.Count == array2.Count);

		if (equal)
		{
			for (int i = 0; i < array1.Count && equal; i++)
			{
				if (array1[i])
					equal &= (array1[i] == (array2[i]));
			}
		}

        return equal;
    }


	public static T[] EnsureLength<T>(T[] array, int desiredLength, bool ensureAtLeast = false)
	{
		if ((array == null) ||
		    (((ensureAtLeast) ? (array.Length < desiredLength) : (array.Length != desiredLength))))
		{
			return new T[desiredLength];
		}
		else
		{
			return array;
		}
	}


    public static bool ArrayContains<T>(this T[] array, T value) where T : Object
    {
        bool result = (array != null) && (value != null);
        if (result)
        {
            result = false;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
}
