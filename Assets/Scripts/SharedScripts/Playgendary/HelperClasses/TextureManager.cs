using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;


public interface ITextureLoader
{
    void Init(System.Action<IntPtr, IntPtr> callbackLoad, System.Action<IntPtr, IntPtr> callbackFail);
    TextureManager.TextureInfo GetInfo(string imagePath);
    IntPtr LoadTexture(string imagePath, bool doMipMaps);
    IntPtr LoadTextureAsync(string imagePath, bool doMipMaps);
    void ReleaseTexture(IntPtr texturePointer);
    void Deinit();
}


public static class TextureManager
{
    #region Variables

    public struct TextureInfo
    {
        public bool    exist;
        public int     width;
        public int     height;
    }

    static bool initialized = false;
    static Dictionary<IntPtr, Texture2D> loadingTextures = new Dictionary<IntPtr, Texture2D>();

    static bool isExistExternalLoader = false;
    static ITextureLoader externalLoader = null;

    #endregion


    #region Public methods

    public static void RegisterLoader(ITextureLoader newLoader)
    {
        if (newLoader != null)
        {
            if (isExistExternalLoader)
            {
                externalLoader.Deinit();
            }
            externalLoader = newLoader;
            initialized = false;
        }
    }


    public static Texture2D LoadTexture(string imagePath, bool doMipMaps)
    {
        Initialize();

        Texture2D resultTexture = null;

        if (isExistExternalLoader)
        {
            TextureInfo infoResult = externalLoader.GetInfo(imagePath);
            if (infoResult.exist)
            {
                IntPtr texture = externalLoader.LoadTexture(imagePath, doMipMaps);
                if (texture != IntPtr.Zero)
                {
                    resultTexture = Texture2D.CreateExternalTexture(infoResult.width, infoResult.height, TextureFormat.RGBA32, doMipMaps, false, texture);
                }
            }
        }
        else
        {
            #if !UNITY_EDITOR
            resultTexture = LoadTextureInternal(imagePath, doMipMaps);
            #else
            resultTexture = LoadTextureEditor(imagePath, doMipMaps);
            #endif
        }

        resultTexture = SetupTexture(resultTexture, doMipMaps);

        return resultTexture;
    }


    public static Texture2D LoadTextureAsync(string imagePath, bool doMipMaps, int preloadWidth = 4, int preloadHeight = 4)
    {
        Initialize();

        Texture2D resultTexture = null;

        if (isExistExternalLoader)
        {
            TextureInfo infoResult = externalLoader.GetInfo(imagePath);
            if (infoResult.exist)
            {
                lock (loadingTextures)
                {                   
                    IntPtr texture = externalLoader.LoadTextureAsync(imagePath, doMipMaps);
                    if (texture != IntPtr.Zero)
                    {
                        resultTexture = Texture2D.CreateExternalTexture(infoResult.width, infoResult.height, TextureFormat.RGBA32, doMipMaps, false, texture);
                        loadingTextures.Add(texture, resultTexture);
                    }
                }
            }
        }
        else
        {
            #if !UNITY_EDITOR
            resultTexture = LoadTextureInternalAsync(imagePath, doMipMaps);
            #else
            resultTexture = LoadTextureEditor(imagePath, doMipMaps);
            #endif
        }

        resultTexture = SetupTexture(resultTexture, doMipMaps);

        return resultTexture;
    }


    public static void UnloadTexture(Texture2D texture)
    {
        if (texture)
        {
            IntPtr texturePointerUnity = texture.GetNativeTexturePtr();

            if (texturePointerUnity != IntPtr.Zero)
            {
                lock (loadingTextures)
                {                   
                    Texture2D result = null;
                    if (loadingTextures.TryGetValue(texturePointerUnity, out result))
                    {
                        loadingTextures.Remove(texturePointerUnity);
                    }

                    if (isExistExternalLoader)
                    {
                        externalLoader.ReleaseTexture(texturePointerUnity);
                    }
                }
            }
        }
    }

    #endregion


    #region Private methods

    static void Initialize()
    {
        if (!initialized)
        {
            if (externalLoader != null)
            {
                isExistExternalLoader = true;
            }
            if (isExistExternalLoader)
            {
                externalLoader.Init(OnTextureLoad, OnTextureFail);
            }

            initialized = true;
        }
    }  


   static Texture2D LoadTextureInternal(string imagePath, bool doMipMaps)
    {
        Texture2D resultTexture = null;

        IntPtr texture = Texture2D.blackTexture.GetNativeTexturePtr();
        resultTexture = Texture2D.CreateExternalTexture(4, 4, TextureFormat.RGBA32, doMipMaps, false, texture);
        using (AssetStreaming streamTexture = new AssetStreaming(imagePath))
        {
            WWW wwwTexture = streamTexture.GetWWW();
            if (wwwTexture != null)
            {
                wwwTexture.LoadImageIntoTexture(resultTexture);
            }
        }

        return resultTexture;
    }


    static Texture2D LoadTextureInternalAsync(string imagePath, bool doMipMaps, int preloadWidth = 4, int preloadHeight = 4)
    {
        Texture2D resultTexture = null;

        IntPtr texture = UnityEngine.GameObject.Instantiate<Texture2D>(Texture2D.blackTexture).GetNativeTexturePtr();
        if (texture != IntPtr.Zero)
        {
            lock (loadingTextures)
            {                
                resultTexture = Texture2D.CreateExternalTexture(preloadWidth, preloadHeight, TextureFormat.RGBA32, doMipMaps, false, texture);
                loadingTextures.Add(texture, resultTexture);

                AssetStreaming streamTexture = new AssetStreaming(imagePath);
                streamTexture.GetWWWAsync((WWW wwwTexture) =>
                    {
                        if (wwwTexture != null)
                        {
                            Texture2D result = null;
                            if (loadingTextures.TryGetValue(texture, out result))
                            {
                                loadingTextures.Remove(texture);
                                if (result)
                                {
                                    wwwTexture.LoadImageIntoTexture(result);
                                }
                            }

                            wwwTexture.Dispose();
                        }
                        else
                        {
                            loadingTextures.Remove(texture);
                        }
                        streamTexture.Unload();
                    });
            }
        }

        return resultTexture;
    }


    static Texture2D LoadTextureEditor(string imagePath, bool doMipMaps)
    {
        Texture2D resultTexture = new Texture2D(4, 4, TextureFormat.RGBA32, doMipMaps);

        byte[] curImageBytes = null;
        if (File.Exists(imagePath))
        {
            curImageBytes = File.ReadAllBytes(imagePath);
        }
        else
        {
            resultTexture = null;
            CustomDebug.LogError("File <" + imagePath + "> does not exist");
        }

        if (curImageBytes == null || !resultTexture.LoadImage(curImageBytes))
        {
            if (curImageBytes == null)
            {
                CustomDebug.LogError("Image bytes is null");
            }
            else
            {
                CustomDebug.Log("Can't load image");
            }
            resultTexture = null;
        }

        return resultTexture;
    }


    static Texture2D SetupTexture(Texture2D text, bool doMipMaps)
    {
        Texture2D resultTexture = text;

        if (resultTexture == null)
        {
            resultTexture = new Texture2D(4, 4, TextureFormat.RGBA32, doMipMaps, false);
        }

        if (resultTexture != null)
        {
            resultTexture.hideFlags = HideFlags.DontSave;
            resultTexture.wrapMode = TextureWrapMode.Clamp;
        }

        return resultTexture;
    }

    #endregion


    #region Events

    static void OnTextureLoad(IntPtr textureID, IntPtr newTexturePointer)
    {
        lock (loadingTextures)
        {
            if (newTexturePointer != IntPtr.Zero)
            {
                Texture2D result = null;
                if (loadingTextures.TryGetValue(textureID, out result))
                {
                    loadingTextures.Remove(textureID);
                    if (result)
                    {
                        result.UpdateExternalTexture(newTexturePointer);
                    }
                }
            }
        }
    }


    static void OnTextureFail(IntPtr textureID, IntPtr newTexturePointer)
    {
        lock (loadingTextures)
        {
            Texture2D result = null;
            if (loadingTextures.TryGetValue(textureID, out result))
            {
                loadingTextures.Remove(textureID);
            }
        }
    }

    #endregion
}
