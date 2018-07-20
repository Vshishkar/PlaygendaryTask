using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TweenCameraHeight))]
public class TweenCameraHeightInspector : TweenerInspector
{
    protected override void CustomInspectorGUI()
    {
        TweenCameraHeight tTransp = (TweenCameraHeight) tween;

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Begin Height", true, GUILayout.Width(150f));
        tTransp.beginHeight = EditorGUILayout.FloatField(tTransp.beginHeight, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("End height", true, GUILayout.Width(150f));
        tTransp.endHeight = EditorGUILayout.FloatField(tTransp.endHeight, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();
    }
}