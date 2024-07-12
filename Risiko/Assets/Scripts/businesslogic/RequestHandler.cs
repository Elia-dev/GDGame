using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RequestHandler
{
    public static void FunctionHandller(string request)
    {
        
        if(request.Contains("Lobby_id:")) // Manage lobby_id request
        {
            request = removeRequest(request, "Lobby_id:");
            ClientManager.Instance.setLobbyId(request);
            // Setta l'id della lobby nella grafica e nello stato del giocatore
        }
        switch(request)
        {
             //case "UserRegistration": GameManager.UserRegistration();
                 //break;
        }
    }

    private static string removeRequest(string source, string input)
    {
        string value = null;
        // Trova la posizione di "input:" e calcola l'inizio del valore
        int startIndex = input.IndexOf(source) + source.Length;

        // Estrai il valore e rimuovi eventuali spazi
        value = input.Substring(startIndex).Trim();
        
        return value;
    }
    
}
