using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenMaterialTransparency))]
public class TweenMaterialTransparencyInspector : TweenerInspector
{
	protected override void CustomInspectorGUI()
	{
		TweenMaterialTransparency tTransp = (TweenMaterialTransparency) tween;
		
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Begin transparency", true, GUILayout.Width(150f));
		tTransp.BeginTransparency = EditorGUILayout.Slider(tTransp.BeginTransparency, 0f, 1f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("End transparency", true, GUILayout.Width(150f));
		tTransp.EndTransparency = EditorGUILayout.Slider(tTransp.EndTransparency, 0f, 1f);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorTools.DrawLabel("Shader transparency id", true, GUILayout.Width(150f));
		tTransp.ShaderTransparencyId = EditorGUILayout.TextField(tTransp.ShaderTransparencyId);
		EditorGUILayout.EndHorizontal();
	}
}