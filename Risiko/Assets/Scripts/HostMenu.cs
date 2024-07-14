using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class HostMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ClientManager cm = ClientManager.Instance;
        
        cm.StartClient();
        cm.Send("username"); // Qui deve prendere il nome dalla classe player, che l'avrà precedentemente presa dalla UI
        cm.StartHost();
        // Adesso deve fare lo show della UI della lobby di attesa in modo che l'host possa far cominciare la partita
        // appena il numero di player è sufficiente
    }

}
