using UnityEngine;
using System.Collections;

public class tmStaticBatchedGameObject : MonoBehaviour 
{
	[SerializeField][ResourceLink] AssetLink staticLink;

	public AssetLink StaticLink 
	{
		get 
		{
			return staticLink;
		}
	}
}
