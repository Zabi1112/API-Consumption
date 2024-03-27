using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsModel
{
    // Fields
    public int userId;
    public string totalKills;
    public string totalDeaths;
    public int totalGames;
    public string totalWins;
    public string username;
    public string avatarPath;
    public int friendId; // Added field

    // Properties
    public int UserId
    {
        get => userId;
        set => userId = value;
    }

    public string TotalKills
    {
        get => totalKills;
        set => totalKills = value;
    }

    public string TotalDeaths
    {
        get => totalDeaths;
        set => totalDeaths = value;
    }

    public int TotalGames
    {
        get => totalGames;
        set => totalGames = value;
    }

    public string TotalWins
    {
        get => totalWins;
        set => totalWins = value;
    }

    public string Username
    {
        get => username;
        set => username = value;
    }

    public string AvatarPath
    {
        get => avatarPath;
        set => avatarPath = value;
    }

    public int FriendId
    {
        get => friendId;
        set => friendId = value;
    } // Added property
}
[System.Serializable]
public class StatsModelDataModel
{
    public int code = 0;
    public string message;
    public StatsModel content;
}