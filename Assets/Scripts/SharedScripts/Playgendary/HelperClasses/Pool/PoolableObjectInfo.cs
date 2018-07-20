using UnityEngine;

public class PoolableObjectInfo : MonoBehaviour
{
	#region Variables

	public ObjectPool poolReference;
	public bool inPool;
	public float distanceToPutToPoolX;
    public float distanceToPutToPoolRightX;
	public float distanceToPutToPoolY;
	public float distanceToPutToPoolZ = -30;
	public bool checkX;
    public bool checkRightX;
	public bool checkY;
	public bool checkZ = true;
	public bool isDisappearDistanceZFixed;
	public bool checkLifeTime;
	public float lifeTime;

	private float lifeTimeCounter;

	Transform cachedTransform;

	#endregion


	#region Properties

	Transform CachedTransform
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

	#endregion


	#region Public methods

	public void ReturnToPool()
	{
		if ((poolReference != null) && !inPool) 
		{
			poolReference.Push(gameObject);
		}
	}


	public ObjectPool GetPool()
	{
		CheckPool();	

		return poolReference;
	}


	void CheckPool() 
	{
		if (poolReference == null) 
		{
			poolReference = PoolManager.Instance.PoolForObject(gameObject);
		}
	}

	#endregion
}
