using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TweenTextureOffset))]
public class TweenTextureOffsetInspector : TweenerInspector 
{
	
	protected override void CustomInspectorGUI() 
    {
		var tTextureOffset = (TweenTextureOffset) tween;
		
        if (tTextureOffset.IsBeginStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorGUILayout.LabelField("Begin offset");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset offset to zero", IsResetOffsetValid(tTextureOffset.beginOffset), 20f)) {
			EditorTools.RegisterUndo("Reset begin offset", tTextureOffset);
			tTextureOffset.beginOffset = Vector2.zero;
		}
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(15f, 0);
        #endif
		tTextureOffset.beginOffset = EditorTools.DrawVector2(tTextureOffset.beginOffset);
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif
		if (EditorTools.DrawButton("S", "Set current offset", IsSetOffsetValid(tTextureOffset.beginOffset, tTextureOffset.CurrentOffset), 20f)) 
        {
			EditorTools.RegisterUndo("Set begin offset", tTextureOffset);
			tTextureOffset.beginOffset = tTextureOffset.CurrentOffset;
		}
		EditorGUILayout.EndHorizontal();
        if (tTextureOffset.IsEndStateSet) 
        {
			GUI.contentColor = Color.green;
		}
		EditorGUILayout.LabelField("End offset");
		GUI.contentColor = defaultContentColor;
		EditorGUILayout.BeginHorizontal();
		if (EditorTools.DrawButton("R", "Reset offset to zero", IsResetOffsetValid(tTextureOffset.endOffset), 20f)) 
        {
			EditorTools.RegisterUndo("Reset end offset", tTextureOffset);
			tTextureOffset.endOffset = Vector2.zero;
		}
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 15f;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(15f, 0);
        #endif
		tTextureOffset.endOffset = EditorTools.DrawVector2(tTextureOffset.endOffset);
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls();
        #endif
		if (EditorTools.DrawButton("S", "Set current offset", IsSetOffsetValid(tTextureOffset.endOffset, tTextureOffset.CurrentOffset), 20f)) 
        {
			EditorTools.RegisterUndo("Set end offset", tTextureOffset);
			tTextureOffset.endOffset = tTextureOffset.CurrentOffset;
		}
		EditorGUILayout.EndHorizontal();		

	}

	bool IsResetOffsetValid(Vector2 v) {
		return (v.x != 0f) || (v.y != 0f);
	}
	
	bool IsSetOffsetValid(Vector2 v, Vector2 cv) {
		return (v.x != cv.x) || (v.y != cv.y);
	}

	

}
