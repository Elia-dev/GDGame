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
        cm.Send("username");
        //cm.StartHost("USERNNAME");
        
    }

}
