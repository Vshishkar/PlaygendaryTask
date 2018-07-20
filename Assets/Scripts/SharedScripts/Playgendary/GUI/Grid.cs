using UnityEngine;
using System.Collections.Generic;

using System;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal,
		Vertical
	}
	
	public Arrangement arrangement = Arrangement.Horizontal;
	public int maxPerLine;
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	public bool repositionNow;
	public bool sorted;

    public event Action OnRepositionEnded;

	Transform cachedTransform;

	public Transform CachedTransform 
	{
		get
		{
			if (cachedTransform == null)
			{
				cachedTransform = transform;
			}
			return cachedTransform;
		}
	}
	
	void Start() 
	{
		Reposition();
	}

	void Update() 
	{
		if (repositionNow) 
		{
			repositionNow = false;
			Reposition();
		}
	}

	public Transform[] GetChilds()
	{
		var list = new List<Transform>();
		foreach (Transform child in CachedTransform) 
		{
			list.Add(child);
		}

		if (sorted) 
		{
			list.Sort((a, b) => string.Compare(a.name, b.name));
		}
		return list.ToArray();
	}

	public void Add(Transform newChild) 
	{
		newChild.parent = CachedTransform;
	}

	public void Remove(Transform newChild)
	{
		newChild.parent = null;
	}

	public void Reposition() 
	{
		int x = 0, y = 0;
		var childs = GetChilds();
		for (int i = 0; i < childs.Length; ++i)
		{
			var newPosition = (arrangement == Arrangement.Horizontal) ?
				new Vector3((cellWidth * x), (-cellHeight * y), 0f) :
				new Vector3((cellWidth * y), (-cellHeight * x), 0f);

			childs[i].localPosition = newPosition;

			if ((++x >= maxPerLine) && (maxPerLine > 0)) 
			{
				x = 0;
				++y;
			}
		}

        if (OnRepositionEnded != null)
        {
            OnRepositionEnded();
        }
	}   

}