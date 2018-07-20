using UnityEngine;

public static class ObjectPoolHelper 
{
	
    static public void ReturnToPool(this GameObject t) 
    {
		var objectPoolInfo = t.GetComponent<PoolableObjectInfo>();
		if (objectPoolInfo != null) 
        {
			objectPoolInfo.ReturnToPool();
		} 
        else 
        {
			Object.Destroy(t);
		}
	}


	static public GameObject Clone(this GameObject t, bool autocreatepool = false, System.Action<GameObject> preAction = null) 
    {
		GameObject clone = null;
		var poolInfo = t.GetComponent<PoolableObjectInfo>();

        if(Application.isPlaying)
        {
            ObjectPool pool = null;

            if(poolInfo == null)
            {
                if(autocreatepool)
                {
                    pool = PoolManager.Instance.PoolForObject(t);
                }
            }
            else
            {
                pool = poolInfo.GetPool();
            } 

            if (pool != null)
            {
                clone = pool.Pop(preAction);
            }
        }

        if(clone == null) 
        {	
			clone = Object.Instantiate(t) as GameObject;
			clone.name = t.name;
		}

		return clone;
	}
}