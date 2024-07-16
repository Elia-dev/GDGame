using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class HostMenu : MonoBehaviour
{
    ClientManager cm = ClientManager.Instance;
    Player player = Player.Instance;
    
    
    // Start is called before the first frame update
    void Start()
    {
        cm.StartClient();
        cm.Send(player.Name);
        cm.CreateLobbyAsHost(); // DAL SERVER DEVE PRIMA ESSERE ARRIVATO L'ID DEL PLAYER (da implementare)
    }

    private void Update()
    {
        cm.getLobbyId(); //Fare lo show di questo a manetta,
                         //prima o poi ci sarà qualcosa di settato in quanto il server prima o poi risponderà
        
                         // Appena viene premuto il bottone start cambiare scena per far cominciare la partita
                         // DUBBIO: Come aggiorno i giocatori nella lobby di attesa appena si connettono al server?
    }
}
