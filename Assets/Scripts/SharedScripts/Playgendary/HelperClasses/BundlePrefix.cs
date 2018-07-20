using UnityEngine;
using System.Collections;
using System;

public static class BundlePrefix
{
    #region Variables

    private const string DOT_SUFFIX = ".";
    private const string IAP_SUFFIX = "iap.";
    private const string LEADERBOARD_SUFFIX = "gc.lb.";
    private const string ACHIEVEMENT_SUFFIX = "gc.ach.";

    static string cachedBundleCompanyID;
    static string cachedBundleAppID;
    static string cachedBundleID;
    static string cachedPrefixBundleID;
    static string cachedPrefixLeaderboardID;
    static string cachedPrefixAchievementID;
    static string cachedPrefixInAppPurchaseID;

    #endregion


    #region Properties

    static string CachedBundleCompanyID
    {
        get
        {
            if (cachedBundleCompanyID == null)
            {
                cachedBundleCompanyID = string.Empty;
                int indexSeparator = CachedBundleID.IndexOf(DOT_SUFFIX);
                if ((indexSeparator > 0) && (indexSeparator + 1 < CachedBundleID.Length))
                {
                    indexSeparator = CachedBundleID.IndexOf(DOT_SUFFIX, indexSeparator + 1);
                    if (indexSeparator > 0)
                    {
                        cachedBundleCompanyID = CachedBundleID.Substring(0, indexSeparator);
                    }
                }
            }
            return cachedBundleCompanyID;
        }
    }

    static string CachedBundleAppID
    {
        get
        {
            if (cachedBundleAppID == null)
            {
                cachedBundleAppID = string.Empty;
                int indexSeparator = CachedBundleID.IndexOf(DOT_SUFFIX);
                if ((indexSeparator > 0) && (indexSeparator + 1 < CachedBundleID.Length))
                {
                    indexSeparator = CachedBundleID.IndexOf(DOT_SUFFIX, indexSeparator + 1);
                    if (indexSeparator > 0)
                    {
                        cachedBundleAppID = CachedBundleID.Substring(indexSeparator + 1);
                    }
                }
            }
            return cachedBundleAppID;
        }
    }

    static string CachedBundleID 
    { 
        get 
        {
            if (cachedBundleID == null)
            {
                cachedBundleID = string.IsNullOrEmpty(Application.identifier) ? (string.Empty) : (Application.identifier);
            } 
            return cachedBundleID; 
        } 
    }
    
    static string CachedPrefixBundleID 
    { 
        get 
        {
            if (cachedPrefixBundleID == null)
            {
                cachedPrefixBundleID = CachedBundleID + DOT_SUFFIX;
            }
            return cachedPrefixBundleID;
        } 
    }

    static string CachedPrefixLeaderboardID 
    { 
        get
        {
            if (cachedPrefixLeaderboardID == null)
            {
                cachedPrefixLeaderboardID = CachedPrefixBundleID + LEADERBOARD_SUFFIX;
            }
            return cachedPrefixLeaderboardID;
        }
    }

    static string CachedPrefixAchievementID
    { 
        get
        {
            if (cachedPrefixAchievementID == null)
            {
                cachedPrefixAchievementID = CachedPrefixBundleID + ACHIEVEMENT_SUFFIX;
            }
            return cachedPrefixAchievementID;
        }
    }

    static string CachedPrefixInAppPurchaseID 
    { 
        get
        {
            if (cachedPrefixInAppPurchaseID == null)
            {
                cachedPrefixInAppPurchaseID = CachedPrefixBundleID + IAP_SUFFIX;
            }
            return cachedPrefixInAppPurchaseID;
        }
    }

    #endregion


    #region Public methods

    public static string BundleCompanyID()
    {
        return CachedBundleCompanyID;
    }

    public static string BundleAppID()
    {
        return CachedBundleAppID;
    }

    public static string BundleID()
    {
        return CachedBundleID;
    }

    public static string PrefixBundleID()
    {
        return CachedPrefixBundleID;
    }
    
    public static string PrefixLeaderboardID()
    {
        return CachedPrefixLeaderboardID;
    }

    public static string PrefixAchievementID()
    {
        return CachedPrefixAchievementID;
    }
    
    public static string PrefixInAppPurchaseID()
    {
        return CachedPrefixInAppPurchaseID;
    }
    
    #endregion
}