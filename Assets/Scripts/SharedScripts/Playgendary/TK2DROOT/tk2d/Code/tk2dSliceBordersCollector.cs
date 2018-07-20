using UnityEngine;
using System.Collections;



[System.Serializable]
public class tk2dSliceBorders
{
	/// <summary>
	/// Borders used if this sprite used as sliced sprite
	/// </summary>
	/// <summary>
	/// Top border in sprite fraction (0 - Top, 1 - Bottom)
	/// </summary>
	public float borderTop = 0.5f;
	/// <summary>
	/// Bottom border in sprite fraction (0 - Bottom, 1 - Top)
	/// </summary>
	public float borderBottom = 0.5f;
	/// <summary>
	/// Left border in sprite fraction (0 - Left, 1 - Right)
	/// </summary>
	public float borderLeft = 0.5f;
	/// <summary>
	/// Right border in sprite fraction (1 - Right, 0 - Left)
	/// </summary>
	public float borderRight = 0.5f;
}




public class tk2dSliceBordersCollector : ScriptableSingleton < tk2dSliceBordersCollector > 
{
	/// <summary>
	/// Dictionary that contains borders
	/// </summary>
	[System.Serializable]
	public class NameToBorderPair : SerializablePair < string, tk2dSliceBorders > {}
	
	[System.Serializable]
	public class NameToBorderDictionary : SerializableDictionary < NameToBorderPair, string, tk2dSliceBorders > {}	

#if UNITY_EDITOR
	[UnityEditor.CustomPropertyDrawer(typeof(NameToBorderDictionary))]
	public class NameToBorderDictionaryDrawer : SerializableDictionaryDrawer<StringHolder, string, tk2dSliceBorders>
	{}
#endif
	
	[SerializeField] public NameToBorderDictionary borders;
//	tk2dSliceBorders DefaultBorders = new tk2dSliceBorders();


	public tk2dSliceBorders GetBordersForID(string strID)
	{
		tk2dSliceBorders result;
		if (borders.TryGetValue(strID, out result))
		{
			return result;
		}
		else
		{
            return new tk2dSliceBorders();
		}
	}


#if UNITY_EDITOR
	public void SetBordersForID(string strID, tk2dSliceBorders _newBord)
	{
		borders.SetValue(strID, _newBord);
        UnityEditor.EditorUtility.SetDirty(this);
	}
#endif
}

