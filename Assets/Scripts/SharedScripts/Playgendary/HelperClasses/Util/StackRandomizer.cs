using UnityEngine;
using System.Collections.Generic;

public class StackRandomizer {

	public int stackSize;
	
	readonly List <int> objects = new List<int>();
	List <int> stack = new List<int>();
	List <int> copyArray = new List <int>();
	
	public StackRandomizer(int count, int stackSize) {
		int startValue = 0;
		this.stackSize = (count <= stackSize) ? (count - 1) : stackSize;
		for (int i = 0; i < count; i++) {
			objects.Add(startValue);
			startValue++;
		}
	}

	public StackRandomizer(int startValue, int count, int stackSize) {
		this.stackSize = (count <= stackSize) ? (count - 1) : stackSize;
		for (int i = 0; i < count; i++) {
			objects.Add(startValue);
			startValue++;
		}
	}

	public int GetRandomIndex() {
		copyArray.Clear();
		for (int i = 0; i < objects.Count; i++) {			
			if (!stack.Contains(objects[i])) {
				copyArray.Add(objects[i]);
			}
		}
		int index = copyArray[Random.Range(0, copyArray.Count)];
		stack.Add(index);
		if (stack.Count > stackSize) {
			stack.RemoveAt(0);
		}
		return index;
	}
}
