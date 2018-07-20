using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    List<string> temp = new List<string>();

    SceneNameAttribute SceneNameAttribute { get { return ((SceneNameAttribute)attribute); } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        EditorGUI.LabelField(position, label);

        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;

        string[] sceneNames = GetSceneNames();

        string prevValue = property.Value<string>();

        int sceneNameIndex = Mathf.Max(0, System.Array.IndexOf<string>(sceneNames, prevValue));
        sceneNameIndex = EditorGUI.Popup(position, sceneNameIndex, sceneNames);
        string newValue = sceneNames[sceneNameIndex];

        property.stringValue = newValue;
    }


    private string[] GetSceneNames()
    {
        temp.Clear();

        foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/')+1);
                name = name.Substring(0,name.Length-6);
                temp.Add(name);
            }
        }
        return temp.ToArray();
    }
}
