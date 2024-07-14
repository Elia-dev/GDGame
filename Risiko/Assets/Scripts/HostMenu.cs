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
        cm.Send(player.Name); // Qui deve prendere il nome dalla classe player, che l'avrà precedentemente presa dalla UI
        cm.CreateLobbyAsHost();
    }

    private void Update()
    {
        cm.getLobbyId(); //Fare lo show di questo a manetta,
                         //prima o poi ci sarà qualcosa di settato in quanto il server prima o poi risponderà
        
                         // Appena viene premuto il bottone start cambiare scena per far cominciare la partita
                         // DUBBIO: Come aggiorno i giocatori nella lobby di attesa apena si connettono al server?
    }
}
