using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utils 
{
    public static string CheckNickname(string username)
    {
        if (username.Contains("-") || username.Contains(":") ||
            username.Contains(";") ||username.Contains("'") ||
            username.Contains("[") || username.Contains("]") || 
            username.Contains(",") || username.Contains("{") || 
            username.Contains("}") || username.Contains("_") || 
            username.Contains(" "))
        {
            return "Not allowed characters :;' -[],{}_";
        }
        
        if (username.Length > 16) {
            return "MAX characters number: 16";
        }
        
        return string.IsNullOrWhiteSpace(username) ? "Username can not be empty" : "OK";
    }
}
