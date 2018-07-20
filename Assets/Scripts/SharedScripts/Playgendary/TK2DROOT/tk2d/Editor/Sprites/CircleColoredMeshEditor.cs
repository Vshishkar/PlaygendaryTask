using UnityEngine;
using UnityEditor; 


[CanEditMultipleObjects]
[CustomEditor(typeof(CircleColoredMesh))]
class CircleColoredMeshEditor : tk2dSpriteEditor 
{
    public override void OnInspectorGUI()
    {
        base.DrawSpriteEditorGUI();

        GUILayout.BeginHorizontal();

        CircleColoredMesh c = (CircleColoredMesh)target;

        c.VertexCount = EditorGUILayout.IntField("Vertex Count", c.VertexCount);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("RebuildMesh"))
        {
            c.Build();
        }

        GUILayout.EndHorizontal();
    }

    new public void OnSceneGUI()
    {
    }
}