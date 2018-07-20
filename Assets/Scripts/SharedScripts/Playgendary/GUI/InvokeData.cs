using UnityEngine;
using System;

[System.Serializable]
public class InvokeData {
	
	public MonoBehaviour script;
	public MonoBehaviour newScript;
	public string method;
	public float delay;
	public Action toDo;
	public bool isTrigger;
	
	public InvokeData() {
		this.script = null;
		this.method = "";
		this.delay = 0f;
		this.toDo = null;
		this.isTrigger = false;
		this.newScript = null;
	}
	
	public bool Call() {
		return Call(0f);
	}
	
	public bool Call(float delay) {
		if ((script != null) && !string.IsNullOrEmpty(method)) {
			script.Invoke(method, delay);
			return true;
		}
		if (toDo != null) {
			toDo();
			return true;
		}
		if (isTrigger) {
			Reset();
		}
		return false;
	}
	
	public void Reset() {
		script = newScript = null;
		method = "";
		toDo = null;
	}
}
