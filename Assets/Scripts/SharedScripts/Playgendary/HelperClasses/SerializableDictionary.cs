using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



[Serializable]
public abstract class SerializablePairBase < TKey, TValue >
	where TKey : IComparable
{
	public abstract TKey PairKey
	{
		get;
		set;
	}
	

	public abstract TValue PairValue
	{
		get;
		set;
	}
}



[Serializable]
public class SerializablePair < TKey, TValue > : SerializablePairBase < TKey, TValue >
	where TKey : IComparable
{
	[SerializeField] private TKey pairKey;
	[SerializeField] private TValue pairValue;

	public override TKey PairKey
	{
		get
		{
			return pairKey;
		}

		set
		{
			pairKey = value;
		}
	}

	public override TValue PairValue
	{
		get
		{
			return pairValue;
		}

		set
		{
			pairValue = value;
		}
	}
}



[Serializable]
public class SerializableDictionary < TPair, TKey, TValue >
	where TKey : IComparable
    where TPair : SerializablePairBase < TKey, TValue >, new()   
{
	[SerializeField] private TPair[] pairData;

	private Dictionary<TKey, TValue> cachedDictionary;
	private Dictionary<TKey, TValue> CachedDictionary
	{
		get
		{
			if (cachedDictionary == null)
			{
				cachedDictionary = new Dictionary<TKey, TValue>(Count);

				if (pairData != null) 
				{
					foreach (var curPair in pairData) 
					{
						if (cachedDictionary.ContainsKey (curPair.PairKey)) 
						{
							CustomDebug.LogError ("Duplicate keys in serializable dicitonary");
						}
					
						cachedDictionary [curPair.PairKey] = curPair.PairValue;
					}
				}
			}
			
			return cachedDictionary;
		}
	}


	#region Private interface
	
	private bool TryGetIndexOfKey(TKey _key, out int index)
	{
		bool hasFound = false;
		index = -1;
		
		for (int i = 0; i < Count; i++)
		{
			if (_key.Equals(pairData[i].PairKey))
			{
				index = i;
				hasFound = true;
			}
		}
		
		return hasFound;
	}
	
	
	#if UNITY_EDITOR
	private void SetCacheDirty()
	{
		cachedDictionary = null;
	}
	
	
	private void AddPair(TKey _key, TValue _value)
	{
		var curCount = Count;

		TPair[] newPairs = new TPair[curCount + 1];
		     
		Array.Copy(pairData, newPairs, curCount);            
        
        newPairs[curCount] = new TPair();

		newPairs[curCount].PairKey = _key;
		newPairs[curCount].PairValue = _value;

		pairData = newPairs;
		
		SetCacheDirty();
	}
	#endif
	
	#endregion
	
	
	#region Public interface
	
    public Dictionary<TKey, TValue>.KeyCollection Keys
    {
        get
        {
            return CachedDictionary.Keys;
        }
    }

	public int Count
	{
		get
		{
			return pairData.Length;
		}
	}
	
	public bool TryGetValue(TKey _key, out TValue outValue)
	{
		return CachedDictionary.TryGetValue(_key, out outValue);
	}
	
	
	#if UNITY_EDITOR
	public void SetValue(TKey _key, TValue _value)
	{
		int index;
		if (TryGetIndexOfKey(_key, out index))
		{
			pairData[index].PairValue = _value;
		}
		else
		{
			AddPair(_key, _value);
		}
	}
	#endif
	
	#endregion
}

