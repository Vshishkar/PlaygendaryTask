using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TweenTk2dSpriteScale))]
public class TweenTk2dSpriteScaleInspector : TweenerInspector
{
    protected override void CustomInspectorGUI()
    {
        TweenTk2dSpriteScale tScale = (TweenTk2dSpriteScale) tween;

        EditorGUILayout.LabelField("Begin scale");
        GUI.contentColor = defaultContentColor;
        EditorGUILayout.BeginHorizontal();
       
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(15f, 0);
        #endif
        tScale.BeginScale = EditorTools.DrawVector3(tScale.BeginScale);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("End scale");
        GUI.contentColor = defaultContentColor;
        EditorGUILayout.BeginHorizontal();

        tScale.EndScale = EditorTools.DrawVector3(tScale.EndScale);


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorTools.DrawLabel("Tween target", true, GUILayout.Width(100f));

        EditorGUILayout.EndHorizontal();
    }
	
}
