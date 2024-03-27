using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager
{
    const string prefKey_UserId = "UserIdKey";
    const string prefKey_AccessTokenKey = "AccessToken";
    const string prefKey_RefreshTokenKey = "RefreshTokenKey";

    //auth

    const string privacyPolicyKey = "PrivacyPolicy";


    public const string firstName = "firstName";
    public const string lastName = "lastName";


    // const string SkipBoosterKey = "SkipBooster";

    public static bool HasPrefsKey(string _key)
    {
        return PlayerPrefs.HasKey(_key);
    }

    public static string GetPlayerPrefs(string _key, string _value = "")
    {
        return PlayerPrefs.GetString(_key, _value);
    }

    public static void SetPlayerPrefs(string _key, string _value)
    {
        PlayerPrefs.SetString(_key, _value);
        PlayerPrefs.Save();
    }

    public static void SetPlayerPrefsInt(string _key, int _value)
    {
        PlayerPrefs.SetInt(_key, _value);
        PlayerPrefs.Save();
    }

    public static int GetPlayerPrefsInt(string _key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(_key, defaultValue);
    }

    public static void DeletePrefs(string _key)
    {
        PlayerPrefs.DeleteKey(_key);
        PlayerPrefs.Save();
    }

    public static string AccessToken
    {
        get { return GetPlayerPrefs(prefKey_AccessTokenKey, ""); }
        set { SetPlayerPrefs(prefKey_AccessTokenKey, value); }
    }

    public static string RefreshToken
    {
        get { return GetPlayerPrefs(prefKey_RefreshTokenKey, ""); }
        set { SetPlayerPrefs(prefKey_RefreshTokenKey, value); }
    }

    public static string UserId
    {
        get { return GetPlayerPrefs(prefKey_UserId, String.Empty); }
        set { SetPlayerPrefs(prefKey_UserId, value); }
    }
}