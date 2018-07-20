using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenParticleSystemStartSize))]
public class TweenParticleSystemStartSizeInspector : TweenerInspector
{
	protected override void CustomInspectorGUI()
	{
		TweenParticleSystemStartSize tSize = (TweenParticleSystemStartSize) tween;

		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Begin start size", true, GUILayout.Width(150f));
		tSize.BeginSize = EditorGUILayout.Slider(tSize.BeginSize, 0f, 10f);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("End start size", true, GUILayout.Width(150f));
		tSize.EndSize = EditorGUILayout.Slider(tSize.EndSize, 0f, 10f);
		EditorGUILayout.EndHorizontal();
	}
}