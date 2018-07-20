using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class tmPlatform
{
	public string name = "";
	public string postfix = "";
	public float scale = 1.0f;
	[SerializeField] string guid;


	public tmPlatform(string name, string postfix, float scale, string guid) 
	{ 
		this.name = name; 
		this.postfix = postfix; 
		this.scale = scale;
		this.guid = guid;
	}


	public override bool Equals(object obj)
	{
		tmPlatform other = obj as tmPlatform;
		if(other != null)
		{
			return this.guid.Equals(other.guid);
		}

		return false;
	}


	public override int GetHashCode()
	{
		// Analysis disable once NonReadonlyReferencedInGetHashCode
		return guid.GetHashCode();
	}
}