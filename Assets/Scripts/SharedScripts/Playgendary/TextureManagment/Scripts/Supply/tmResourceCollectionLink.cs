using UnityEngine;
using System.Collections;

public class tmResourceCollectionLink : ScriptableObject 
{
	#if UNITY_EDITOR
	public tmTextureCollectionPlatform collectionInEditor;
	#endif

	public tmTextureCollectionPlatform collection;
}
