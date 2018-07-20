using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpriteRenderInitializer))]
class SpriteRenderInitializerEditor : Editor
{
    // Serialized properties are going to be far too much hassle
    private SpriteRenderInitializer[] targetRenderInitializers = new SpriteRenderInitializer[0];
   

    public override void OnInspectorGUI()
    {
        DrawSpriteEditorGUI();
    }


    protected T[] GetTargetsOfType<T>( Object[] objects ) where T : UnityEngine.Object {
        List<T> ts = new List<T>();
        foreach (Object o in objects) {
            T s = o as T;
            if (s != null)
                ts.Add(s);
        }
        return ts.ToArray();
    }

    protected void OnEnable()
    {
        targetRenderInitializers = GetTargetsOfType<SpriteRenderInitializer>( targets );
       
    }

    void OnDestroy()
    {
        targetRenderInitializers = new SpriteRenderInitializer[0];

        tk2dSpriteThumbnailCache.Done();
        tk2dGrid.Done();
        tk2dEditorSkin.Done();
    }

    // Callback and delegate
    void SpriteChangedCallbackImpl(tk2dSpriteCollectionData spriteCollection, int spriteId, object data)
    {
        tk2dUndo.RecordObjects(targetRenderInitializers, "Sprite Change");

        foreach (SpriteRenderInitializer s in targetRenderInitializers) {
			s.SetSprite(spriteCollection, spriteId, (tk2dBaseSprite.Anchor)data);
            EditorUtility.SetDirty(s);
        }
    }
    tk2dSpriteGuiUtility.SpriteChangedCallback _spriteChangedCallbackInstance = null;
    tk2dSpriteGuiUtility.SpriteChangedCallback spriteChangedCallbackInstance {
        get {
            if (_spriteChangedCallbackInstance == null) {
                _spriteChangedCallbackInstance = new tk2dSpriteGuiUtility.SpriteChangedCallback( SpriteChangedCallbackImpl );
            }
            return _spriteChangedCallbackInstance;
        }
    }

    protected void DrawSpriteEditorGUI()
    {
        Event ev = Event.current;
		tk2dSpriteGuiUtility.SpriteSelector( targetRenderInitializers[0].Collection, targetRenderInitializers[0].SpriteID, spriteChangedCallbackInstance, targetRenderInitializers[0].AnchorPoint);
		

		if (targetRenderInitializers[0].Collection != null)
        {
            if (tk2dPreferences.inst.displayTextureThumbs) {
                SpriteRenderInitializer renderInitializer = targetRenderInitializers[0];
                tk2dSpriteDefinition def = renderInitializer.CurrentSprite;
                if (renderInitializer.Collection.version < 1 || def.texelSize == Vector2.zero)
                {
                    string message = "";

                    message = "No thumbnail data.";
                    if (renderInitializer.Collection.version < 1)
                        message += "\nPlease rebuild Sprite Collection.";

                    tk2dGuiUtility.InfoBox(message, tk2dGuiUtility.WarningLevel.Info);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" ");

                    int tileSize = 128;
                    Rect r = GUILayoutUtility.GetRect(tileSize, tileSize, GUILayout.ExpandWidth(false));
                    tk2dGrid.Draw(r);
                    tk2dSpriteThumbnailCache.DrawSpriteTextureInRect(r, def, Color.white);

                    GUILayout.EndHorizontal();

                    r = GUILayoutUtility.GetLastRect();
                    if (ev.type == EventType.MouseDown && ev.button == 0 && r.Contains(ev.mousePosition)) {
                        tk2dSpriteGuiUtility.SpriteSelectorPopup( renderInitializer.Collection, renderInitializer.SpriteID, spriteChangedCallbackInstance, null );
                    }
                }
            }           
           
            GUILayout.Space(8);           

			GUILayout.BeginHorizontal();
			tk2dBaseSprite.Anchor anchor = (tk2dBaseSprite.Anchor)EditorTools.DrawEnumField(targetRenderInitializers[0].AnchorPoint, "AnchorPoint");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			int pixelsPerMeter = (int)EditorTools.DrawFloatField(targetRenderInitializers[0].TargetPixelsPerMeter, "Pixels Per Meter");
			GUILayout.EndHorizontal();

			if ((anchor != targetRenderInitializers[0].AnchorPoint) || (pixelsPerMeter != targetRenderInitializers[0].TargetPixelsPerMeter))
			{
				targetRenderInitializers[0].AnchorPoint = anchor;
				targetRenderInitializers[0].TargetPixelsPerMeter = pixelsPerMeter;
				targetRenderInitializers[0].ForceBuild();
			}
			
		}
        else
        {
            tk2dGuiUtility.InfoBox("Please select a sprite collection.", tk2dGuiUtility.WarningLevel.Error);        
        }


        bool needUpdatePrefabs = false;
        if (GUI.changed)
        {
            foreach (SpriteRenderInitializer sprite in targetRenderInitializers) {
                if (PrefabUtility.GetPrefabType(sprite) == PrefabType.Prefab)
                    needUpdatePrefabs = true;
                EditorUtility.SetDirty(sprite);
            }
        }

        // This is a prefab, and changes need to be propagated. This isn't supported in Unity 3.4
        if (needUpdatePrefabs)
        {
            // Rebuild prefab instances
            tk2dBaseSprite[] allSprites = Resources.FindObjectsOfTypeAll(typeof(tk2dBaseSprite)) as tk2dBaseSprite[];
            foreach (var spr in allSprites)
            {
                if (PrefabUtility.GetPrefabType(spr) == PrefabType.PrefabInstance)
                {
                    Object parent = PrefabUtility.GetPrefabParent(spr.gameObject);
                    bool found = false;
                    foreach (SpriteRenderInitializer sprite in targetRenderInitializers) {
                        if (sprite.gameObject == parent) {
                            found = true;
                            break;
                        }
                    }

                    if (found) {
                        // Reset all prefab states
                        var propMod = PrefabUtility.GetPropertyModifications(spr);
                        PrefabUtility.ResetToPrefabState(spr);
                        PrefabUtility.SetPropertyModifications(spr, propMod);

                        spr.ForceBuild();
                    }
                }
            }
        }
    }   


}

