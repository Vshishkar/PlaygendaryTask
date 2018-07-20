using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TweenGUICellSize))]
public class TweenGUICellSizeInspector : TweenerInspector 
{
    protected override void CustomInspectorGUI()
    {
        TweenGUICellSize currentTween = (TweenGUICellSize)tween;

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Begin size", true, GUILayout.Width(150f));
        currentTween.beginSize = EditorGUILayout.FloatField(currentTween.beginSize, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("End size", true, GUILayout.Width(150f));
        currentTween.endSize = EditorGUILayout.FloatField(currentTween.endSize, GUILayout.MinWidth(150f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));
        if (EditorTools.DrawButton("R", "Reset target", IsResetTargetValid(currentTween), 20f)) 
        {
            EditorTools.RegisterUndo("Reset target", currentTween);
            currentTween.Target = null;
        }
        currentTween.Target = (GUILayoutCell)EditorGUILayout.ObjectField(currentTween.Target, typeof(GUILayoutCell), true);
        EditorGUILayout.EndHorizontal();
    }


    bool IsResetTargetValid(TweenGUICellSize currentTween)
    {
        return (currentTween.Target != currentTween.gameObject);
    }
}
