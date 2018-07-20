using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GUILayoutCell))]
public class GUILayouterCellEditor : Editor 
{

	public override void OnInspectorGUI()
	{		
		base.OnInspectorGUI ();

		GUILayoutCell targetLayouterCell = (GUILayoutCell)target;

		GUILayout.Space (10);
		EditorGUILayout.Separator ();
		GUILayout.Space (10);

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Create VerticalLayouter ",GUILayout.MinWidth(20))) 
		{
			GameObject newLayouterObj = new GameObject ("VerticalLayout");
			GUILayouter newLayoter = newLayouterObj.AddComponent<GUILayouter> ();
			newLayoter.Type = GUILayouterType.Vertical;

			newLayouterObj.transform.parent = targetLayouterCell.CachedTransform;
			newLayouterObj.transform.localPosition = Vector3.zero;
			newLayouterObj.layer = targetLayouterCell.gameObject.layer;

			targetLayouterCell.LayoutHandlerObjects.Add (newLayouterObj);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Create HorizontalLayouter ",GUILayout.MinWidth(20))) 
		{
			GameObject newLayouterObj = new GameObject ("HorizontalLayout");
			GUILayouter newLayoter = newLayouterObj.AddComponent<GUILayouter> ();
			newLayoter.Type = GUILayouterType.Horizontal;

			newLayouterObj.transform.parent = targetLayouterCell.CachedTransform;
			newLayouterObj.transform.localPosition = Vector3.zero;
			newLayouterObj.layer = targetLayouterCell.gameObject.layer;

			targetLayouterCell.LayoutHandlerObjects.Add (newLayouterObj);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Create TextMesh ",GUILayout.MinWidth(20))) 
		{
			GameObject newLayouterObj = new GameObject ("Label");
			newLayouterObj.AddComponent<tk2dTextMesh> ();

			newLayouterObj.transform.parent = targetLayouterCell.CachedTransform;
			newLayouterObj.transform.localPosition = Vector3.zero;
			newLayouterObj.layer = targetLayouterCell.gameObject.layer;

			targetLayouterCell.LayoutHandlerObjects.Add (newLayouterObj);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Create Independent Sprite ",GUILayout.MinWidth(20))) 
		{
			GameObject newLayouterObj = new GameObject ("Sprite");
			newLayouterObj.AddComponent<tk2dSprite> ();

			newLayouterObj.transform.parent = targetLayouterCell.CachedTransform;
			newLayouterObj.transform.localPosition = Vector3.zero;
			newLayouterObj.layer = targetLayouterCell.gameObject.layer;
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Create Fill Sprite ",GUILayout.MinWidth(20))) 
		{
			GameObject newLayouterObj = new GameObject ("Sprite");
			newLayouterObj.AddComponent<tk2dSprite> ();

			newLayouterObj.transform.parent = targetLayouterCell.CachedTransform;
			newLayouterObj.transform.localPosition = Vector3.zero;
			newLayouterObj.layer = targetLayouterCell.gameObject.layer;

			targetLayouterCell.LayoutHandlerObjects.Add (newLayouterObj);
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Create SlicedSprite ",GUILayout.MinWidth(20))) 
		{
			GameObject newLayouterObj = new GameObject ("SlicedSprite");
			newLayouterObj.AddComponent<tk2dSlicedSprite> ();

			newLayouterObj.transform.parent = targetLayouterCell.CachedTransform;
			newLayouterObj.transform.localPosition = Vector3.zero;
			newLayouterObj.layer = targetLayouterCell.gameObject.layer;

			targetLayouterCell.LayoutHandlerObjects.Add (newLayouterObj);
		}
		EditorGUILayout.EndHorizontal ();

	}


}
