using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenCameraSize))]
public class TweenCameraSizeInspector : TweenerInspector
{
    protected override void CustomInspectorGUI()
    {
        TweenCameraSize tTransp = (TweenCameraSize) tween;

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Begin Size", true, GUILayout.Width(150f));
        tTransp.beginSize = EditorGUILayout.FloatField(tTransp.beginSize, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("End Size", true, GUILayout.Width(150f));
        tTransp.endSize = EditorGUILayout.FloatField(tTransp.endSize, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();
    }
}