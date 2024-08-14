using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utils 
{
    public static bool CheckNickname(string username)
    {
        if (username.Contains("-") || username.Contains(":") ||
            username.Contains(";") ||username.Contains("'") ||
            username.Contains("[") || username.Contains("]") || 
            username.Contains(",") || username.Contains("{") || 
            username.Contains("}")) {
            // POPUP errore caratteri non consentiti:  :;'-[],{}
            GameObject.Find("PopUpContainer").GetComponent<PopUpBadNameUI>()
                .SetErrorText("Not allowed characters :;'-[],{}");
            return false;
        }
        if (username.Length > 16) {
            // POPUP errore nome troppo lungo
            GameObject.Find("PopUpContainer").GetComponent<PopUpBadNameUI>()
                .SetErrorText("MAX characters number: 16");
            return false;
        }
        // TODO - Exception
        return true;
    }
}
