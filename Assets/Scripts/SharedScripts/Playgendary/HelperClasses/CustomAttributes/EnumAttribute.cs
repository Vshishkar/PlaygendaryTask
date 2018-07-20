using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public class EnumAttribute : PropertyAttribute 
{
	string[] listForEnum;

	Type listSourceClass;
	string listSourceMethod;


	public string[] ListForEnum
	{
		get
		{
            if (listSourceClass != null)
            {
                if (!string.IsNullOrEmpty(listSourceMethod))
                {
                    UpdateMethod();
                }
                else
                {
                    UpdateFields();
                }
            }

            return listForEnum;
		}
	}


	EnumAttribute()
	{

	}


    public EnumAttribute(Type sourceClassConstFields)
    {
        if (sourceClassConstFields != null)
        {
            listSourceClass = sourceClassConstFields;
            listSourceMethod = null;
            UpdateFields();
        }
    }


    public EnumAttribute(Type sourceClassList, string sourceMethodList)
    {
        if (sourceClassList != null)
        {
            listSourceClass = sourceClassList;

            if (!string.IsNullOrEmpty(sourceMethodList))
            {
                listSourceMethod = sourceMethodList;
                UpdateMethod();
            }
            else
            {
                listSourceMethod = null;
            }
        }
    }


	public EnumAttribute(params object[] list)
	{
		if (list.Length > 0)
		{
            listForEnum = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
                listForEnum[i] = list[i].ToString();
			}
		}
	}


	public EnumAttribute(string[] list)
	{
		if (list.Length > 0)
		{
            listForEnum = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
                listForEnum[i] = list[i];
			}
		}
	}



	void UpdateMethod()
	{
        object[] list = listSourceClass.GetMethod(listSourceMethod).Invoke(null, null) as object[];

		if (list.Length > 0)
		{
            listForEnum = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
			{
                if (list[i] != null)
                {
                    listForEnum[i] = list[i].ToString();
                }
			}
		}
	}


    void UpdateFields()
    {
        List<string> list = new List<string>();
        foreach (FieldInfo field in listSourceClass.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.IsLiteral && !field.IsInitOnly)
            {
                object fieldValue = field.GetRawConstantValue();
                if (fieldValue != null)
                {
                    list.Add(fieldValue.ToString());
                }
            }
        }
        listForEnum = list.ToArray();
    }
}
