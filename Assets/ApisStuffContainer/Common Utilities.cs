using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CommonUtilities
{
    public static bool IsValidEmail(string email)
    {
        // Simple pattern check – you might use a more sophisticated regex
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}

public enum LogInType
{
    google,
    twitter,
    email,
}