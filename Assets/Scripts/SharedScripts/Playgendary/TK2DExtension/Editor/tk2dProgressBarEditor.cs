using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CustomEditor(typeof(tk2dProgressBar))]
class tk2dProgressBarEditor : tk2dSpriteEditor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		tk2dProgressBar bar = target as tk2dProgressBar;

		bar.fillDirection = (tk2dProgressBar.FillDirection)EditorGUILayout.EnumPopup ("Fill Direction", bar.fillDirection);
		bar.fillAmount = EditorGUILayout.Slider("Fill Amount", bar.fillAmount, 0, 1);
		bar.invert = EditorGUILayout.Toggle("Invert", bar.invert);
        bar.rotated = EditorGUILayout.Toggle("Rotated", bar.rotated);

		EditorUtility.SetDirty( target );
	}

}
