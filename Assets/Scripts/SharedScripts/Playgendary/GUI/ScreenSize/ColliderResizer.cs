using UnityEngine;
using System.Collections;

public class ColliderResizer : MonoBehaviour, ILayoutCellHandler
{
	#region Variables

	[SerializeField] float yCoof = 1f;
	[SerializeField] float xCoof = 1f;
	[SerializeField] BoxCollider settedCollider;

	BoxCollider buttonCollider;

	BoxCollider ButtonCollider
	{
		get
		{
			if(buttonCollider == null)
			{
				if(settedCollider != null)
				{
					buttonCollider = settedCollider;
				}
				else
				{
					buttonCollider = GetComponent<BoxCollider>();
				}
			}
			return buttonCollider;
		}
	}

	#endregion


	#region Interface

	public void RepositionForCell(LayoutCellInfo info)
	{
		if(ButtonCollider == null)
		{
			CustomDebug.LogError("Collider not set !");
			return;
		}

		ButtonCollider.size = new Vector3 (info.cellRect.width * xCoof, info.cellRect.height * yCoof, 1f);
	}

	#endregion
}
