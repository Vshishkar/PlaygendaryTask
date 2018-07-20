using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dUIScrollView))]
public class tk2dUIScrollViewEditor : Editor {

	public override void OnInspectorGUI() {
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(0, 0);
        #endif
		base.OnInspectorGUI();

		GUILayout.Label("Tools", EditorStyles.boldLabel);
		if (GUILayout.Button("Calculate content length")) {
			tk2dUIScrollView scrollView = (tk2dUIScrollView) target;

			Undo.RecordObject(scrollView, "Content length changed");
			Bounds b = tk2dUIItemBoundsHelper.GetRendererBoundsInChildren(scrollView.contentContainer, scrollView.contentContainer);
			float contentSize = (scrollView.scrollDirection == ScrollDirection.Horizontal) ? b.size.x : b.size.y;
			scrollView.contentLength = contentSize * 1.02f; // 5% more
			EditorUtility.SetDirty(scrollView);
		}
	}

	public void OnSceneGUI() {
		bool wasChange = false;
		tk2dUIScrollView scrollView = (tk2dUIScrollView) target;
		bool isVertical = scrollView.scrollDirection == ScrollDirection.Vertical;

		// Get rescaled transforms
		Matrix4x4 m = scrollView.transform.localToWorldMatrix;
		Vector3 up = m.MultiplyVector(Vector3.up);
		Vector3 right = m.MultiplyVector(Vector3.right);

		float newContentLength = tk2dUIControlsHelperEditor.DrawLengthHandles("Content Length", scrollView.contentLength, scrollView.transform.position, isVertical ? -up : right, Color.blue, isVertical ? .2f : -.2f, isVertical ? .4f : -.4f, .1f);
		if (newContentLength != scrollView.contentLength) {
			Undo.RecordObject(scrollView, "Content length changed");
			scrollView.contentLength = newContentLength;
			wasChange = true;
		}

		if (wasChange) {
			EditorUtility.SetDirty(scrollView);
		}
	}
}
