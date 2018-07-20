using UnityEngine;
using UnityEditor;
using System.Reflection;


[InitializeOnLoad]
public static class tmColorPicker
{
	static tmColorPicker()
	{
//		EditorApplication.update -= OnGUI;
//		EditorApplication.update += OnGUI;
//
//		SceneView.onSceneGUIDelegate -= OnGUI;
//		SceneView.onSceneGUIDelegate += OnGUI;
	}
//
//
//	static System.Type eyeDropperType = GetType("EyeDropper");
//	static MethodInfo mi = eyeDropperType.GetMethod("GetPickedColor", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
//	static FieldInfo fi = eyeDropperType.GetField("s_PickCoordinates", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
//
//
//	static System.Type GetType(string typeName)
//	{
//		foreach(Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
//		{
//			System.Type[] types = a.GetTypes();
//
//			foreach(System.Type t in types)
//			{
//				if(t.Name.Equals(typeName))
//				{
//					return t;
//				}
//			}
//		}
//
//		return null;
//	}
//
//
//	static Event currentEvent;
//	static Vector2 editorPos;
//
//
//	static void OnGUI(SceneView view)
//	{
//		currentEvent = Event.current;
//		editorPos = GUIUtility.GUIToScreenPoint(Vector2.zero);
//	}
//
//
//	static void OnGUI()
//	{
//		if(currentEvent.isMouse && currentEvent.control)
//		{
//			Vector2 pos = currentEvent.mousePosition + new Vector2(editorPos.x - 2, editorPos.y - 36);
//
//			fi.SetValue(null, pos);
//			Color c = (Color)mi.Invoke(null, null);
//			Debug.Log(
//				"r:" + ((int)(c.r * 256)) +
//				" g:" + ((int)(c.g * 256)) +
//				" b:" + ((int)(c.b * 256)) +
//				" a:" + ((int)(c.a * 256))
//			);
//		}
//	}
}
