using System;

[Serializable]
public class UserModel
{
    // Fields
    public int userId;
    public string username;
    public string password;
    public string email;
    public string country;
    public string loginType;
    public string avatarPath;
    public int friendId = -1;
    public DateTime createdAt;
    public bool isNewUser;
    public string accessToken;
    public string refreshToken;
    public int xp;
    public int level;

    // Properties
    public int UserId { get => userId; set => userId = value; }
    public string Username { get => username; set => username = value; }
    public string Password { get => password; set => password = value; }
    public string Email { get => email; set => email = value; }
    public string Country { get => country; set => country = value; }
    public string LoginType { get => loginType; set => loginType = value; }
    public string AvatarPath { get => avatarPath; set => avatarPath = value; }
    public int FriendId { get => friendId; set => friendId = value; }
    public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
    public bool IsNewUser { get => isNewUser; set => isNewUser = value; }
    public string AccessToken { get => accessToken; set => accessToken = value; }
    public string RefreshToken { get => refreshToken; set => refreshToken = value; }
    public int Xp { get => xp; set => xp = value; }
    public int Level { get => level; set => level = value; }
}
[System.Serializable]
public class UserDataModel
{
    public int code = 0;
    public string message;
    public UserModel content;
}