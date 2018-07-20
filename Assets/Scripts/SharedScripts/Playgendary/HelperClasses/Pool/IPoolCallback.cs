using UnityEngine;

public interface IPoolCallback 
{
	void OnCreateFromPool();
	void OnReturnToPool();

	void OnPop();
	void OnPush();
}
