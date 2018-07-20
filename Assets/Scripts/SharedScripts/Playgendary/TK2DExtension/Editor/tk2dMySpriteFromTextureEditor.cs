using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(tk2dMySpriteFromTexture))]
class tk2dMySpriteFromTextureEditor : Editor {
	
	public override void OnInspectorGUI() {
		tk2dMySpriteFromTexture target = (tk2dMySpriteFromTexture) this.target;
        #if UNITY_5_4_OR_NEWER
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
        #else
        EditorGUIUtility.LookLikeControls(0, 0);
        #endif
		EditorGUI.BeginChangeCheck();
		Texture texture = EditorGUILayout.ObjectField("Texture", target.Texture, typeof(Texture), false) as Texture;
		if (texture == null) {
            #if UNITY_5_4_OR_NEWER
            EditorGUIUtility.labelWidth = 0;
            EditorGUIUtility.fieldWidth = 0;
            #else
            EditorGUIUtility.LookLikeControls();
            #endif
			tk2dGuiUtility.InfoBox("Drag a texture into the texture slot above.", tk2dGuiUtility.WarningLevel.Error);
		}
		tk2dBaseSprite.Anchor anchor = target.anchor;
		bool rebuild = target.rebuild;
		tk2dSpriteCollectionSize spriteCollectionSize = new tk2dSpriteCollectionSize();
		spriteCollectionSize.CopyFrom(target.spriteCollectionSize);
		if (texture != null) {
			rebuild = EditorGUILayout.Toggle("Rebuild on Awake", target.rebuild);
			anchor = (tk2dBaseSprite.Anchor) EditorGUILayout.EnumPopup("Anchor", target.anchor);
			SpriteCollectionSizeField(spriteCollectionSize);
		}
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(target, "Sprite From Texture");
			target.anchor = anchor;
			target.rebuild = rebuild;
			target.spriteCollectionSize = spriteCollectionSize;
			target.Texture = texture;
			target.ForceBuild();
		}
		if (GUILayout.Button("Force build")) target.ForceBuild();
	}

	void SpriteCollectionSizeField(tk2dSpriteCollectionSize spriteCollectionSize) {
		GUIContent sc = new GUIContent("Sprite Collection Size", null, "The resolution this sprite will be pixel perfect at");
		spriteCollectionSize.type = (tk2dSpriteCollectionSize.Type) EditorGUILayout.EnumPopup(sc, spriteCollectionSize.type);
		if (spriteCollectionSize.type == tk2dSpriteCollectionSize.Type.Explicit) {
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField("Resolution", "");
			EditorGUI.indentLevel++;
			spriteCollectionSize.width = EditorGUILayout.IntField("Width", (int) spriteCollectionSize.width);
			spriteCollectionSize.height = EditorGUILayout.IntField("Height", (int) spriteCollectionSize.height);
			EditorGUI.indentLevel--;
			spriteCollectionSize.orthoSize = EditorGUILayout.FloatField("Ortho Size", spriteCollectionSize.orthoSize);
			EditorGUI.indentLevel--;
		}
	}
}