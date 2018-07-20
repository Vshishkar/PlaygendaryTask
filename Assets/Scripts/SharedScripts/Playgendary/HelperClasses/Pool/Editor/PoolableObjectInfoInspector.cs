using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(PoolableObjectInfo))]
public class PoolableObjectInfoInspector : Editor {

	static List<PoolableObjectInfo> poolableObjectsInfo = new List<PoolableObjectInfo>();
	Color offColor = new Color(1f, 1f, 1f, 0.25f);
	Color onColor;

	void Awake() {
		poolableObjectsInfo.Clear();
		for (int i = 0; i < targets.Length; ++i) {
			if (targets[i] is PoolableObjectInfo) {
				poolableObjectsInfo.Add((PoolableObjectInfo) targets[i]);
			}
		}
		onColor = GUI.color;
	}

	public override void OnInspectorGUI() {
		var info = (PoolableObjectInfo) target;
		bool changed = false;

		var newPoolReference = (ObjectPool) EditorGUILayout.ObjectField("Pool reference", info.poolReference, typeof(ObjectPool), true);
		if (info.poolReference != newPoolReference) {
			info.poolReference = newPoolReference;
			changed = true;
		}

		var newInPool = EditorGUILayout.Toggle("In pool", info.inPool);
		if (info.inPool != newInPool) {
			RunAction(info, i => i.inPool = newInPool);
			changed = true;
		}

		EditorGUILayout.BeginHorizontal();
		{
			var newCheckX = EditorGUILayout.Toggle("Put to pool by left X", info.checkX);
            if (info.checkX != newCheckX) {
                RunAction(info, i => i.checkX = newCheckX);
				changed = true;
			}

			GUI.color = info.checkX ? onColor : offColor;
			EditorGUILayout.LabelField("Left X", GUILayout.Width(50f));
            var newDistanceToPutToPoolX = EditorGUILayout.FloatField(info.distanceToPutToPoolX);
            if (info.checkX && (info.distanceToPutToPoolX != newDistanceToPutToPoolX)) {
                RunAction(info, i => i.distanceToPutToPoolX = newDistanceToPutToPoolX);
				changed = true;
			}

			GUI.color = onColor;
		}
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            var newCheckRightX = EditorGUILayout.Toggle("Put to pool by right X", info.checkRightX);
            if (info.checkRightX != newCheckRightX) {
                RunAction(info, i => i.checkRightX = newCheckRightX);
                changed = true;
            }

            GUI.color = info.checkRightX ? onColor : offColor;
            EditorGUILayout.LabelField("Right X", GUILayout.Width(50f));
            var newDistanceToPutToPoolRightX = EditorGUILayout.FloatField(info.distanceToPutToPoolRightX);
            if (info.checkRightX && (info.distanceToPutToPoolRightX != newDistanceToPutToPoolRightX)) {
                RunAction(info, i => i.distanceToPutToPoolRightX = newDistanceToPutToPoolRightX);
                changed = true;
            }

            GUI.color = onColor;
        }
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			var newCheckY = EditorGUILayout.Toggle("Put to pool by Y", info.checkY);
			if (info.checkY != newCheckY) {
				RunAction(info, i => i.checkY = newCheckY);
				changed = true;
			}

			GUI.color = info.checkY ? onColor : offColor;
			EditorGUILayout.LabelField("Y", GUILayout.Width(50f));
			var newDistanceToPutToPoolY = EditorGUILayout.FloatField(info.distanceToPutToPoolY);
			if (info.checkY && (info.distanceToPutToPoolY != newDistanceToPutToPoolY)) {
				RunAction(info, i => i.distanceToPutToPoolY = newDistanceToPutToPoolY);
				changed = true;
			}
			GUI.color = onColor;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			var newCheckZ = EditorGUILayout.Toggle("Put to pool by Z", info.checkZ);
			if (info.checkZ != newCheckZ) {
				RunAction(info, i => i.checkZ = newCheckZ);
				changed = true;
			}

			GUI.color = info.checkZ ? onColor : offColor;
			EditorGUILayout.LabelField("Z", GUILayout.Width(50f));
			var newDistanceToPutToPoolZ = EditorGUILayout.FloatField(info.distanceToPutToPoolZ);
			if (info.checkZ && (info.distanceToPutToPoolZ != newDistanceToPutToPoolZ)) {
				RunAction(info, i => i.distanceToPutToPoolZ = newDistanceToPutToPoolZ);
				changed = true;
			}
			GUI.color = onColor;
		}
		EditorGUILayout.EndHorizontal();

		var newIsDisappearDistanceZFixed = EditorGUILayout.Toggle("Is disappear Z distance fixed", info.isDisappearDistanceZFixed);
		if (info.isDisappearDistanceZFixed != newIsDisappearDistanceZFixed) {
			RunAction(info, i => i.isDisappearDistanceZFixed = newIsDisappearDistanceZFixed);
			changed = true;
		}

		EditorGUILayout.BeginHorizontal();
		{
			var newCheckLifeTime = EditorGUILayout.Toggle("Put to pool by Life Time", info.checkLifeTime);
			if (info.checkLifeTime != newCheckLifeTime) {
				RunAction(info, i => i.checkLifeTime = newCheckLifeTime);
				changed = true;
			}
			
			GUI.color = info.checkLifeTime ? onColor : offColor;
			EditorGUILayout.LabelField("Life Time", GUILayout.Width(60f));
			var newLifeTimeToPutInPool = EditorGUILayout.FloatField(info.lifeTime);
			if (info.checkLifeTime && (System.Math.Abs(info.lifeTime - newLifeTimeToPutInPool) > float.Epsilon)) {
				RunAction(info, i => i.lifeTime = newLifeTimeToPutInPool);
				changed = true;
			}
			GUI.color = onColor;
		}
		EditorGUILayout.EndHorizontal();

		//EditorGUIUtility.LookLikeInspector();

		GUILayout.BeginHorizontal("Box");
		{
			if (GUILayout.Button("Create Pool")) {
				Create();
			}
		}
		GUILayout.EndHorizontal();

		if (changed) {
			EditorUtility.SetDirty(target);
		}
	}

	[MenuItem("GameObject/Create Other/Pool", false, 10001)]
	static void PoolAdd() {
		var objects = Selection.objects;
		foreach (Object o in objects) {
			var prefab = o as GameObject;
			var poolObject = new GameObject();

			poolObject.name = prefab.name + "Pool";
			poolObject.transform.position = Vector3.zero;
			var pool = poolObject.AddComponent<ObjectPool>();

			pool.autoExtend = true;
			pool.prefab = prefab;
			pool.preInstantiateCount = 1;
		}
	}
		
	void Create() {
		var objects = Selection.objects;
		foreach (Object o in objects) {
			var prefab = o as GameObject;
			var poolObject = new GameObject();

			poolObject.name = prefab.name + "Pool";
			poolObject.transform.position = Vector3.zero;
			var pool = poolObject.AddComponent<ObjectPool>();

			pool.autoExtend = true;
			pool.prefab = prefab;
			pool.preInstantiateCount = 1;
		}
	}

	static void RunAction(PoolableObjectInfo selected, System.Action<PoolableObjectInfo> action) {
		foreach (var info in poolableObjectsInfo) {
			action(info);
		}
		if (!poolableObjectsInfo.Contains(selected)) {
			action(selected);
		}
	}
}
