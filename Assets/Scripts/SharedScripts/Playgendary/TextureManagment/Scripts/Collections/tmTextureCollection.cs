using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class tmTextureCollectionPlatformRef
{
	public string collectionName;
	public string collectionGuid;
	public string assetnGuid;
	public tmPlatform platform;
	#if UNITY_EDITOR
	public tmTextureCollectionPlatform collectionPlatform;
	#endif

}


public class tmTextureCollection : tmTextureCollectionBase
{
	[SerializeField] public List<tmTextureCollectionPlatformRef> platforms = new List<tmTextureCollectionPlatformRef>();
}
