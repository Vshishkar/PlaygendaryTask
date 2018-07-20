using UnityEngine;
using System;

public abstract class Asset : IDisposable
{
    #region Properties

    public abstract bool IsLoaded
    {
        get;
    }

    #endregion


    #region Public methods

    public abstract void        Load();
    public abstract void        LoadAsync(System.Action callback);
    public abstract void        Unload();


    public virtual Asset Copy()
    {
        return this;
    }
        
    public void Dispose()
    {
        Unload();
    }

    #endregion
}