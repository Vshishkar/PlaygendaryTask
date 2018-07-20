using UnityEngine;
using System;


[AttributeUsage(AttributeTargets.Method)]
public sealed class PreProcessBuildAttribute : Attribute
{
    int m_callbackOrder;


    public int callbackOrder 
    {
        get
        {
            return this.m_callbackOrder;
        }
    }

    //
    // Constructors
    //
    public PreProcessBuildAttribute ()
    {
        this.m_callbackOrder = 1;
    }


    public PreProcessBuildAttribute (int callbackOrder)
    {
        this.m_callbackOrder = callbackOrder;
    }
}
