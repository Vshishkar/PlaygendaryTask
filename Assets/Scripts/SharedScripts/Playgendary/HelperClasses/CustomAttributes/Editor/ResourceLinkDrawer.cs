using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;


[CustomPropertyDrawer(typeof(ResourceLinkAttribute))]
public class ResourceLinkDrawer : PropertyDrawer 
{
	ResourceLinkAttribute NamedAttribute { get { return ((ResourceLinkAttribute)attribute); } }

	static Dictionary<string, string> normalizedNames = new Dictionary<string, string>();
	public static string UpperSeparatedName(string name)
	{
		string nName = "";
		if(!normalizedNames.TryGetValue(name, out nName))
		{
			foreach(char letter in name)
			{
				if(string.IsNullOrEmpty(nName))
				{
					nName += char.ToUpper(letter);
				}
				else
				{
					if (char.IsUpper(letter))
						nName += " " + letter;
					else
						nName += letter;
				}
			}

			normalizedNames.Add(name, nName);
		}

		return nName;
	}


	static Dictionary<string, Object> guidAssetMapper = new Dictionary<string, Object>();
	static Object GUIDToAsset(string guid, System.Type type)
	{
		Object asset;
		if(!guidAssetMapper.TryGetValue(guid, out asset) || (asset == null))
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
			guidAssetMapper.Remove(guid);
			guidAssetMapper.Add(guid, asset);
		}

		return asset;
	}


	static MethodInfo SetAsset = typeof(AssetLink).GetMethod("SetAsset", BindingFlags.Instance | BindingFlags.NonPublic);



	public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
	{
		Object asset = null;
		SerializedProperty assetGUIDProperty = _property.FindPropertyRelative("assetGUID");

		if(!string.IsNullOrEmpty(assetGUIDProperty.stringValue))
		{
			asset = GUIDToAsset(assetGUIDProperty.stringValue, NamedAttribute.resourceType);
		}

		Object newAsset = EditorGUI.ObjectField(_position, UpperSeparatedName(_property.name), asset, NamedAttribute.resourceType, false);
		if(asset != newAsset)
		{
			object current = _property.serializedObject.targetObject;
			string[] fields = _property.propertyPath.Split('.');

			for (int i = 0; i < fields.Length; i++) 
			{
				string fieldName = fields[i];

				if(fieldName.Equals("Array"))
				{
					fieldName = fields[++i];
					string indexString = fieldName.Substring(5, fieldName.Length - 6);
					int index = int.Parse(indexString);

					System.Type type = current.GetType();
					if(type.IsArray)
					{
						System.Array array = current as System.Array;
						current = array.GetValue(index);
					}
					else if( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) 
					{
						IList list = current as IList;
						current = list[index];
					}
				}
				else
				{
					FieldInfo field = current.GetType().GetField(fields[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					current = field.GetValue(current);
				}
			}

			AssetLink obj = current as AssetLink;
			SetAsset.Invoke(obj, new object[] {newAsset});
//			obj.asset = newAsset;
			_property.serializedObject.ApplyModifiedProperties();

			EditorUtility.SetDirty(_property.serializedObject.targetObject);
		}
	}
}
