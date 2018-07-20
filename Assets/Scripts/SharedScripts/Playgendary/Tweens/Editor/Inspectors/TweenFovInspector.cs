using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenFOV))]
public class TweenFovInspector : TweenerInspector
{
    protected override void CustomInspectorGUI()
    {
        TweenFOV tTransp = (TweenFOV) tween;

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Begin FOV", true, GUILayout.Width(150f));
        tTransp.beginFOV = EditorGUILayout.FloatField(tTransp.beginFOV, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("End FOV", true, GUILayout.Width(150f));
        tTransp.endFOV = EditorGUILayout.FloatField(tTransp.endFOV, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();
    }
}