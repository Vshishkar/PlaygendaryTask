using UnityEngine;
using System.Collections;

public class GUICellColliderSetter : MonoBehaviour , ILayoutCellHandler
{
	#region Variables
	
	[SerializeField] GUILayoutCell rePositionPlace;
	[SerializeField] BoxCollider settedCollider;
	[SerializeField] float sizeMultiplier = 1;

	BoxCollider toReposition;
	BoxCollider ToReposition
	{
		get
		{
			if(toReposition == null)
			{
				if(settedCollider != null)
				{
					toReposition = settedCollider;
				}
				else
				{
					toReposition = GetComponent<BoxCollider>();
				}
			}
			return toReposition;
		}
	}

	#endregion

	#region ILayoutCellHandler implementation

	public void RepositionForCell (LayoutCellInfo info)
	{
		if (rePositionPlace)
		{
			float y = (rePositionPlace.transform.position.y - transform.position.y) / (Vector3.one / ((float)ScreenDimentions.Height / (float)Screen.height)).y;
			float x = (rePositionPlace.transform.position.x - transform.position.x) / (Vector3.one / ((float)ScreenDimentions.Width / (float)Screen.width)).x;

			Vector3 center = new Vector3 (x, y, 0);
			ToReposition.center = center;
			ToReposition.size = new Vector3 (rePositionPlace.RecievedRect.size.x * sizeMultiplier, rePositionPlace.RecievedRect.size.y * sizeMultiplier, 1);
		}
		else
		{
			ToReposition.size = new Vector3 (info.cellRect.size.x * sizeMultiplier,info.cellRect.size.y * sizeMultiplier, 1);
		}
	}

	#endregion


}
