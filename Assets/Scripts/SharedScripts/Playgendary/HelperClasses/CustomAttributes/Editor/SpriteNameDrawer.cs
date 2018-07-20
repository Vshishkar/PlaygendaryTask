using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(SpriteNameAttribute))]
public class SpriteNameDrawer : PropertyDrawer
{
    SpriteNameAttribute SpriteNameAttribute { get { return ((SpriteNameAttribute)attribute); } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        EditorGUI.LabelField(position, label);

        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;

        string prevValue = property.Value<string>();
        string tk2dspriteName = SpriteNameAttribute.TargetTK2DSprite;

        if (string.IsNullOrEmpty(tk2dspriteName))
        {
            property.stringValue = string.Empty;
            EditorGUI.LabelField(position, "Check name reference");
        }
        else
        {
            object obj = AttributeUtility.GetParentObjectFromProperty(property);

            Type containerType = obj.GetType();

            FieldInfo f = null;

            while (containerType != null)
            {
                f = containerType.GetField(tk2dspriteName, BindingFlags.NonPublic | BindingFlags.Instance); //fix

                if (f != null)
                {
                    break;
                }

                containerType = containerType.BaseType;
            }

            if ((obj != null) && (f != null))
            {
                tk2dSpriteCollectionData collection = f.GetValue(obj) as tk2dSpriteCollectionData;
                if (collection == null)
                {
                    tk2dBaseSprite tk2dSprite = f.GetValue(obj) as tk2dBaseSprite;
                    if (tk2dSprite != null)
                    {
                        collection = tk2dSprite.Collection;
                    }
                }


                if (collection != null)
                {
                    if ((collection.inst != null) &&
                        (collection.inst.spriteDefinitions != null))
                    {
                        List<string> spriteNames = new List<string>();
                        for (int i = 0; i < collection.inst.spriteDefinitions.Length; i++)
                        {
                            tk2dSpriteDefinition def = collection.inst.spriteDefinitions[i];
                            if (def != null)
                            {
                                string name = def.name;
                                if (!string.IsNullOrEmpty(name))
                                {
                                    spriteNames.Add(name);
                                }
                            }
                        }

                        if (spriteNames.Count > 0)
                        {
                            spriteNames.Sort();
                            string[] spriteNamesArray = spriteNames.ToArray();

                            int spriteNameIndex = Mathf.Max(0, System.Array.IndexOf<string>(spriteNamesArray, prevValue));
                            spriteNameIndex = EditorGUI.Popup(position, spriteNameIndex, spriteNamesArray);
                            string newValue = spriteNamesArray[spriteNameIndex];

                            property.stringValue = newValue;
                        }
                        else
                        {
                            property.stringValue = string.Empty;
                            EditorGUI.LabelField(position, "Check sprites collection");
                        }
                    }
                    else
                    {
                        property.stringValue = string.Empty;
                        EditorGUI.LabelField(position, "Check instance collection");
                    }
                }
                else
                {
                    property.stringValue = string.Empty;
                    EditorGUI.LabelField(position, "Set TK2DSprite or TK2DCollection reference");
                }
            }
            else
            {
                property.stringValue = string.Empty;
                EditorGUI.LabelField(position, "Set TK2DSprite or TK2DCollection reference");
            }
        }
    }
}



