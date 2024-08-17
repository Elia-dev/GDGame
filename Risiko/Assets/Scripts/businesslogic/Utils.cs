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

    public static Color32 ColorCode(string color, byte alpha) {
        switch (color) {
            case "red":
                return new Color32(255, 0, 0, alpha);
            case "green":
                return new Color32(0, 190, 0, alpha);
            case "blue":
                return new Color32(0, 0, 255, alpha);                break;
            case "yellow":
                return new Color32(226, 230, 30, alpha);                break;
            case "purple":
                return new Color32(124, 33, 239, alpha);                break;
            case "black":
                return new Color32(0, 0, 0, alpha);      
            default:
                return new Color32(0, 0, 0, alpha);
        }
    }
}
