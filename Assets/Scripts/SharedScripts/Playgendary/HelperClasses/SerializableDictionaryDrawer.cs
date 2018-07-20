using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;



#region New key holder

[System.Serializable]
public abstract class ScriptableHolder<T> : ScriptableObject
{
	[SerializeField] public T value;

	public abstract void PutValue(SerializedProperty destination);
	public abstract bool KeysAreEqual(SerializedProperty k1, SerializedProperty k2);
}


[System.Serializable]
public class StringHolder : ScriptableHolder<string>
{
	public override void PutValue(SerializedProperty destination)
	{
		destination.stringValue = value;
	}
	
	public override bool KeysAreEqual(SerializedProperty k1, SerializedProperty k2)
	{
		return k1.stringValue == k2.stringValue;
	}
}


[System.Serializable]
public class IntHolder : ScriptableHolder<int>
{
	public override void PutValue(SerializedProperty destination)
	{
		destination.intValue = value;
	}
	
	public override bool KeysAreEqual(SerializedProperty k1, SerializedProperty k2)
	{
		return k1.intValue == k2.intValue;
	}
}


[System.Serializable]
public class EnumHolder < T > : ScriptableHolder < T >
{
	public override void PutValue(SerializedProperty destination)
	{
		destination.enumValueIndex = System.Array.IndexOf(System.Enum.GetValues(typeof(T)), value);
	}
	
	public override bool KeysAreEqual(SerializedProperty k1, SerializedProperty k2)
	{
		return k1.enumValueIndex == k2.enumValueIndex;
	}
}

#endregion



#region Pair wrapper

public abstract class SerializablePairWrapperBase
{
	public SerializablePairWrapperBase() {}
	
	public abstract SerializedProperty KeyPropertyForPairProperty(SerializedProperty pairProp);
	public abstract SerializedProperty ValuePropertyForPairProperty(SerializedProperty pairProp);
}


public class SerializablePairWrapper : SerializablePairWrapperBase
{
	public SerializablePairWrapper() {}
	
	protected virtual string KeyPropertyName
	{
		get
		{
			return "pairKey";
		}
	}
	
	protected virtual string ValuePropertyName
	{
		get
		{
			return "pairValue";
		}
	}
	
	public override SerializedProperty KeyPropertyForPairProperty(SerializedProperty pairProp)
	{
		return pairProp.FindPropertyRelative(KeyPropertyName);
	}
	
	public override SerializedProperty ValuePropertyForPairProperty(SerializedProperty pairProp)
	{
		return pairProp.FindPropertyRelative(ValuePropertyName);
	}
}

#endregion



#region Rect extention

public static class RectExtention
{
	public static Rect CutRectVertical(ref Rect input, float heightToCut)
	{
		Rect result = input;
		result.height = heightToCut;
		
		input.yMin += result.height;
		
		return result;
	}


	public static Rect CutRectVertcalRelative(ref Rect input, float amount)
	{
		Rect result = input;
		result.height *= amount;

		input.yMin += result.height;

		return input;
	}


	public static Rect CutRectHorizontal(ref Rect input, float widthToCut)
	{
		Rect result = input;
		result.width = widthToCut;
		
		input.xMin += result.width;
		
		return result;
	}
	
	
	public static Rect CutRectHorizontalRelative(ref Rect input, float amount)
	{
		Rect result = input;
		result.width *= amount;
		
		input.xMin += result.width;
		
		return result;
	}
}

#endregion



public class SerializableDictionaryDrawer < TPairWrapper, THolder, TKey, TValue > : PropertyDrawer 
	where TKey : System.IComparable
		where THolder : ScriptableHolder < TKey >
		where TPairWrapper : SerializablePairWrapperBase, new ()
{
	#region PropertyWrapper 
	
	class PropertyWrapper
	{
		SerializedProperty root;
		TPairWrapper pairWrapper = new TPairWrapper();
		
		private SerializedProperty PairsArray
		{
			get
			{
				return root.FindPropertyRelative("pairData");
			}
		}
		
		
		public PropertyWrapper(SerializedProperty prop)
		{
			root = prop;
		}
		
		public int Count
		{
			get
			{
				return PairsArray.arraySize;
			}
		}
		
		public float ElementsHeight
		{
			get
			{
				float result = 0.0f;
				
				for (int i = 0; i < Count; i++)
				{
					result += GetElementHeight(i);
				}
				
				return result;
			}
		}
		
		
		public float GetElementHeight(int index)
		{
			var curKey = GetKeyAtIndex(index);
			var curValue = GetValueAtIndex(index);
			return Mathf.Max(EditorGUI.GetPropertyHeight(curKey), EditorGUI.GetPropertyHeight(curValue));
		}
		
		
		public SerializedProperty GetKeyAtIndex(int index)
		{
			return pairWrapper.KeyPropertyForPairProperty(PairsArray.GetArrayElementAtIndex(index));
		}
		
		
		public SerializedProperty GetValueAtIndex(int index)
		{
			return pairWrapper.ValuePropertyForPairProperty(PairsArray.GetArrayElementAtIndex(index));
		}
		
		
		public void InsertPairAtIndex(int index)
		{
			PairsArray.InsertArrayElementAtIndex(index);
		}
		
		
		public void DeletePairAtIndex(int index)
		{
			PairsArray.DeleteArrayElementAtIndex(index);
		}
		
		
		public void Apply()
		{
			root.serializedObject.ApplyModifiedProperties();
		}
	}
	
	#endregion
	
	
	static bool popOverToggle = false;
	
	static THolder holder;
	THolder Holder
	{
		get
		{
			if (holder == null)
			{
				var newInst = ScriptableObject.CreateInstance(typeof(THolder));
				holder = newInst as THolder;
			}
			
			return holder;
		}
	}
	
	
	static SerializedObject serializedHolder;
	SerializedObject SerializedHolder
	{
		get
		{
			if (serializedHolder == null || serializedHolder.targetObject != Holder)
			{
				serializedHolder = new SerializedObject(Holder);
			}
			
			return serializedHolder;
		}
	}
	
	
	SerializedProperty currentNewKey
	{
		get
		{
			return SerializedHolder.FindProperty("value");
		}
	}
	
	
	public float AddControlHeight
	{
		get
		{
			return TotalLineSpacing;
		}
	}
	
	
	public float FoldOutHeight
	{
		get
		{
			return TotalLineSpacing;
		}
	}
	
	
	public override float GetPropertyHeight (SerializedProperty prop,
	                                         GUIContent label)
	{
		if (popOverToggle)
		{
			PropertyWrapper curWrapper = new PropertyWrapper(prop);
			return AddControlHeight + FoldOutHeight + curWrapper.ElementsHeight;
		}
		else
		{
			return base.GetPropertyHeight(prop, label);
		}
	}
	
	
	private float TotalLineSpacing
	{
		get
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
	}
	
	
	private bool TryAddNewPair(PropertyWrapper wrapper, SerializedProperty newKey)
	{
		bool suchKeyExists = false;
		
		for (int i = 0; i < wrapper.Count; i++)
		{
			var curKey = wrapper.GetKeyAtIndex(i);
			if (Holder.KeysAreEqual(curKey, newKey))
			{
				suchKeyExists = true;
			}
		}
		
		if (!suchKeyExists)
		{
			int curSize = wrapper.Count;
			wrapper.InsertPairAtIndex(curSize);
			Holder.PutValue(wrapper.GetKeyAtIndex(curSize));
			wrapper.Apply();
			return true;
		}
		else
		{
			CustomDebug.LogError("Key already exists " + newKey.ToString());
			return false;
		}
	}
	
	
	private bool DrawOneElement(ref Rect inputRect, PropertyWrapper wrapper, int index)
	{
		Rect currentRect = RectExtention.CutRectVertical(ref inputRect, wrapper.GetElementHeight(index));
		
		Rect buttonRect = RectExtention.CutRectHorizontal(ref currentRect, TotalLineSpacing);
		buttonRect.width = buttonRect.height = EditorGUIUtility.singleLineHeight;
		if (GUI.Button(buttonRect, "-"))
		{
			wrapper.DeletePairAtIndex(index);
			return false;
		}
		else
		{
			EditorGUI.BeginDisabledGroup(true);
			{
				EditorGUI.PropertyField(RectExtention.CutRectHorizontalRelative(ref currentRect, 0.3f), 
				                        wrapper.GetKeyAtIndex(index),
				                        GUIContent.none,
				                        true);
			}
			EditorGUI.EndDisabledGroup();
			
			EditorGUI.indentLevel ++;
			{
				EditorGUI.PropertyField(currentRect,
				                        wrapper.GetValueAtIndex(index),
				                        GUIContent.none,
				                        true);
			}
			EditorGUI.indentLevel --;
			
			return true;
		}
	}
	
	
	private void DrawElements(ref Rect inputRect, PropertyWrapper wrapper)
	{
		for (int i = 0; i < wrapper.Count; i++)
		{
			if (!DrawOneElement(ref inputRect, wrapper, i))
			{
				break;
			}
		}
	}
	
	
	private void DrawAddControl(ref Rect inputRect, PropertyWrapper wrapper)
	{
		Rect currentRect = RectExtention.CutRectVertical(ref inputRect, AddControlHeight);
		
		EditorGUI.PropertyField(RectExtention.CutRectHorizontalRelative(ref currentRect, 0.8f), currentNewKey, GUIContent.none);
		SerializedHolder.ApplyModifiedProperties();
		
		
		if (GUI.Button(currentRect, "Add"))
		{
			TryAddNewPair(wrapper, currentNewKey);
		}
	}
	
	
	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label)
	{
		Rect currentRect = pos;
		
		popOverToggle = EditorGUI.Foldout(RectExtention.CutRectVertical(ref currentRect, FoldOutHeight), popOverToggle, label, true);
		
		if (popOverToggle)
		{
			PropertyWrapper curWrapper = new PropertyWrapper(prop);
			
			DrawAddControl(ref currentRect, curWrapper);
			DrawElements(ref currentRect, curWrapper);
		}
	}
}


public class SerializableDictionaryDrawer < THolder, TKey, TValue > : SerializableDictionaryDrawer < SerializablePairWrapper, THolder, TKey, TValue  > 
	where TKey : System.IComparable
		where THolder : ScriptableHolder < TKey >
{}


#endif

