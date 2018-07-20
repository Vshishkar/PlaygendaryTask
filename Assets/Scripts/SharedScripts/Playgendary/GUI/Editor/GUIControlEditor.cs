using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
 
[CustomEditor(typeof(GUIControl), true)]
public class GUIControlEditor : Editor
{ 
 
	#region Public
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();

        GUIControl selectedControl = (GUIControl) target;

        DrawControls(selectedControl);
        DrawTweeners(selectedControl);
               
        EditorUtility.SetDirty(target);

        EditorGUILayout.BeginHorizontal();

        if (selectedControl.IsHidden)
        {
            GUI.contentColor = Color.green;
        }
        else
        {
            GUI.contentColor = Color.white;
        }
        if (GUILayout.Button("Show"))
        {
            selectedControl.Show();
        }
        if (selectedControl.IsShown)
        {
            GUI.contentColor = Color.green;
        }
        else
        {
            GUI.contentColor = Color.white;
        }
        if (GUILayout.Button("Hide"))
        {
            selectedControl.Hide();
        }

        GUI.contentColor = Color.white;

        EditorGUILayout.EndHorizontal();

    }
	#endregion
 
 
	#region Private

    void DrawControls(GUIControl c)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+++ Add Control +++", GUILayout.MinWidth(100f)))
        {
            c.ControlsData.Add(new GUIControl.ControlData());
        }

        float fieldsSize = 30f;

        EditorGUILayout.EndHorizontal();

        List<GUIControl.ControlData> controlsForRemove = new List<GUIControl.ControlData>();

        foreach (GUIControl.ControlData td in c.ControlsData)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("________________________________________________________");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Control: ", GUILayout.MaxWidth(80f));

            td.control = (GUIControl)EditorGUILayout.ObjectField(td.control, typeof(GUIControl), true, GUILayout.MaxWidth(200f));

            if (GUILayout.Button("Delete", GUILayout.MaxWidth(50f)))
            {
                controlsForRemove.Add(td);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("ShowDelay: ", GUILayout.MaxWidth(65f));

            td.showDelay = EditorGUILayout.FloatField(td.showDelay, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.LabelField("Var: ", GUILayout.MaxWidth(30f));

            td.showDelayVariance = EditorGUILayout.FloatField(td.showDelayVariance, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.LabelField("HideDelay: ", GUILayout.MaxWidth(65f));

            td.hideDelay = EditorGUILayout.FloatField(td.hideDelay, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.LabelField("Var: ", GUILayout.MaxWidth(30f));

            td.hideDelayVariance = EditorGUILayout.FloatField(td.hideDelayVariance, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("________________________________________________________");
            EditorGUILayout.EndVertical();
        }

        c.ControlsData.RemoveAll(delegate(GUIControl.ControlData obj)
        {
            return controlsForRemove.Contains(obj);
        });
    }

    void DrawTweeners(GUIControl c)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+++ Add Tween +++", GUILayout.MinWidth(100f)))
        {
            c.TweensData.Add(new GUIControl.TweenData());
        }

        EditorGUILayout.EndHorizontal();

        List<GUIControl.TweenData> tweensForRemove = new List<GUIControl.TweenData>();

        float fieldsSize = 30f;

        foreach (GUIControl.TweenData td in c.TweensData)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("________________________________________________________");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Tween: ", GUILayout.MaxWidth(80f));

            td.tween = (Tweener)EditorGUILayout.ObjectField(td.tween, typeof(Tweener), true, GUILayout.MaxWidth(200f));

            if (GUILayout.Button("Delete", GUILayout.MaxWidth(50f)))
            {
                tweensForRemove.Add(td);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("ShowDelay: ", GUILayout.MaxWidth(65f));

            td.showDelay = EditorGUILayout.FloatField(td.showDelay, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.LabelField("Var: ", GUILayout.MaxWidth(30f));

            td.showDelayVariance = EditorGUILayout.FloatField(td.showDelayVariance, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.LabelField("HideDelay: ", GUILayout.MaxWidth(65f));

            td.hideDelay = EditorGUILayout.FloatField(td.hideDelay, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.LabelField("Var: ", GUILayout.MaxWidth(30f));

            td.hideDelayVariance = EditorGUILayout.FloatField(td.hideDelayVariance, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Show Duration Scale: ", GUILayout.MaxWidth(120f));

            td.showDurationScale = EditorGUILayout.FloatField(td.showDurationScale, GUILayout.MaxWidth(fieldsSize));
           
            EditorGUILayout.LabelField("Hide Duration Scale: ", GUILayout.MaxWidth(120f));

            td.hideDurationScale = EditorGUILayout.FloatField(td.hideDurationScale, GUILayout.MaxWidth(fieldsSize));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("________________________________________________________");
            EditorGUILayout.EndVertical();
        }

        c.TweensData.RemoveAll(delegate(GUIControl.TweenData obj)
        {
            return tweensForRemove.Contains(obj);
        });
    }

	#endregion
}