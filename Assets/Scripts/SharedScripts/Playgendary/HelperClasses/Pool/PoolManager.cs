using UnityEngine;
using System.Collections.Generic;

public class PoolManager : SingletonMonoBehaviour<PoolManager> 
{
    new static public PoolManager Instance
    { 
        get { 
			PoolManager pm = PoolManager.InstanceIfExist;
            if(pm == null)
            {
                GameObject g = new GameObject("PoolManager");
                pm = g.AddComponent<PoolManager>();
            }
            return pm;
        } 
    }

	public List<ObjectPool> pools = new List<ObjectPool>();
    public Dictionary<string, ObjectPool> poolMap = new Dictionary<string, ObjectPool>();

    Transform poolObjectsRoot;

    public Transform PoolObjectsRoot
    {
        get
        {
            if (poolObjectsRoot == null)
            {
                GameObject root = new GameObject("PoolObjectsRoot");
                poolObjectsRoot = root.transform;
            }

            return poolObjectsRoot;
        }
    }


	protected override void Awake() 
    {
		base.Awake();
		gameObject.GetComponentsInChildren<ObjectPool>(pools);

		#if UNITY_EDITOR
		pools.Sort
		(
			(a, b) => string.CompareOrdinal(a.name, b.name)
		);
		#endif

		foreach(ObjectPool pool in pools)
        {
			if(pool.prefab == null)
			{
				CustomDebug.LogError("Missing prefab in pool : " + pool.name, pool);
			}
			else
			{
				if(!poolMap.ContainsKey(pool.prefab.name))
				{
					poolMap.Add(pool.prefab.name, pool);
				}
				else
				{
					CustomDebug.LogError("Duplicate : " + pool.prefab.name, pool);
				}
			}
        }
	}


	public ObjectPool FindPool(Object prefab) 
    {
		ObjectPool pool = null;
		if(!poolMap.TryGetValue(prefab.name, out pool))
		{
//			CustomDebug.LogError("Cant find pool for prefab : " + prefab.name);
		}

		return pool;
	}


	public ObjectPool PoolForObject(GameObject prefab)
    {
		var pool = FindPool(prefab);
		if (pool == null)
        {
			var poolObject = new GameObject();
			poolObject.name = prefab.name + "Pool";
			poolObject.transform.position = Vector3.zero;
			poolObject.transform.parent = transform;
			pool = poolObject.AddComponent<ObjectPool>();
			pool.prefab = prefab;
			pool.autoExtend = true;

			pools.Add(pool);
            poolMap.Add(prefab.name, pool);
		}
		return pool;
	}


    public void RemoveObjectPool(ObjectPool pool)
    {
		if (pool != null)
		{
			pools.Remove(pool);
			poolMap.Remove(pool.prefab.name);
		}
    }
}
