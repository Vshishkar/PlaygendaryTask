using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(tmSettings))]
public class tmSettingsEditor : Editor 
{
	tmSettings settings
	{
		get
		{
			return target as tmSettings;
		}
	}


	public override void OnInspectorGUI() 
	{
		//		base.OnInspectorGUI();

		tmPlatform targetPlatform = settings.TargetPlatform;
		if(PlatformPopup("Target platform", ref targetPlatform))
		{
			settings.TargetPlatform = targetPlatform;
			EditorUtility.SetDirty(settings);
			tmCollectionBuilder.ValidateResourceLinks();
		}


		tmPlatform currentPlatform = settings.CurrentPlatform;
		if(PlatformPopup("Current platform", ref currentPlatform))
		{
			settings.CurrentPlatform = currentPlatform;
			EditorUtility.SetDirty(settings);
		}


		tmPlatform defaultPlatform = settings.DefaultPlatform;
		if(PlatformPopup("Default platform", ref defaultPlatform))
		{
			settings.DefaultPlatform = defaultPlatform;
			EditorUtility.SetDirty(settings);
		}

		settings.autoRebuild = EditorGUILayout.Toggle("Auto Rebuild", settings.autoRebuild);
		settings.ForceStaticGeometry = EditorGUILayout.Toggle("Force Static Geometry", settings.ForceStaticGeometry);
		settings.rebuildMesh = EditorGUILayout.Toggle("Rebuild mesh uv", settings.rebuildMesh);
		settings.batching = EditorGUILayout.Toggle("Batching", settings.batching);
        settings.isImmediateTextureLoadEnabled = EditorGUILayout.Toggle("Synchronous texture load enabled", settings.isImmediateTextureLoadEnabled);
        settings.isAtlasesPreloadEnabled = EditorGUILayout.Toggle("Atlases preload on tmManager's Awake enabled", settings.isAtlasesPreloadEnabled);
        settings.isAtlasesUnloadEnabled = EditorGUILayout.Toggle("Atlases unload on ref count = 0 enabled", settings.isAtlasesUnloadEnabled);

		EditorUtility.SetDirty(settings);
	}


	bool PlatformPopup(string label, ref tmPlatform currentPlatform)
	{
		int selectedIndex = -1;
		List<string> entryNames = new List<string>();

		for (int i = 0; i < settings.texturePlatforms.Length; i++) 
		{
			tmPlatform platform = settings.texturePlatforms[i];
			entryNames.Add(platform.name);

			if(currentPlatform != null && !string.IsNullOrEmpty(currentPlatform.name))
			{
				if(currentPlatform.name.Equals(platform.name))
				{
					selectedIndex = i;
				}
			}
		}

		int lastIndex = selectedIndex;
		selectedIndex = EditorGUILayout.Popup(label, selectedIndex, entryNames.ToArray());

		if(selectedIndex != -1)
		{
			currentPlatform = settings.texturePlatforms[selectedIndex];
		}

		return lastIndex != selectedIndex;
	}
}
