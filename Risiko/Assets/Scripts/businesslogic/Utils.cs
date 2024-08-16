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
            username.Contains("}") || username.Contains("_")) {
            return "Not allowed characters :;'-[],{}_";
        }
        
        if (username.Length > 16) {
            // POPUP errore nome troppo lungo
            GameObject.Find("PopUpContainer").GetComponent<PopUpBadNameUI>()
                .SetErrorText("MAX characters number: 16");
            return "MAX characters number: 16";
        }
        
        return "OK";
    }
}
