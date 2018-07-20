using UnityEngine;
using System.Collections.Generic;


public class AssetQueue
{
	readonly List<Asset> queue;


	public AssetQueue() 
	{
		queue = new List<Asset>();
	}


	public AssetQueue(int capacity)
	{
		if (capacity < 0) 
		{
			throw new System.ArgumentOutOfRangeException("capacity");
		}
		queue = new List<Asset>(capacity);
	}


	public int Count 
	{
		get { return queue.Count; }
	}


	public bool IsEmpty 
	{
		get { return queue.Count == 0; }
	}


	public bool IsReady 
	{
		get
		{ 
            return Count > 0 && queue.Peek().IsLoaded; 
		}
	}


	public void Clear()
	{
		queue.Clear();
	}


	public void Enqueue(Asset asset)
	{
		queue.Enqueue(asset);
	}


	public Asset Dequeue()
	{
		if(IsReady)
		{
			return queue.Dequeue();
		}

		return null;
	}
}