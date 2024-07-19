using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

public class ClientManager_old
{
    private static ClientManager_old _instance;
    private static readonly object Lock = new object();
    private ClientManager_old() // Private constructor to allow instantiation using singleton only
    {
    }
    public static ClientManager_old Instance // Implementing singleton pattern
    {
        get
        {
            // Using lock to manage concurrency
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new ClientManager_old();
                }
                return _instance;
            }
        }
    }
    
    private string server = "127.0.0.1";
    private int port = 1234;
    private TcpClient client;

    private string lobby_id;
    public string getLobbyId()
    {
        return lobby_id;
    }
    public void setLobbyId(string lobby_id)
    {
        this.lobby_id = lobby_id;
    }
    public TcpClient Client
    {get { return client; } }

    public void StartClient()
    {
        client = new TcpClient();
        client.Connect(this.server, this.port);
        
        //Console.WriteLine("Type 'exit' anytime to quit");

        Thread receiverThread = new Thread(new ParameterizedThreadStart(ReceiveMessages));
        receiverThread.Start(client);
        
    }

    public void CreateLobbyAsHost()
    {
        Send("HOST: " + Player.Instance.PlayerId); // Telling the server that I will be the host
    }

    public void JoinLobbyAsClient(string LobbyID)
    {
        Send("CLIENT - LOBBY_ID: " + LobbyID);
    }

    public void Send(string message)
    {
        NetworkStream stream = this.client.GetStream();
        while ((message = Console.ReadLine()) != null)
        {
            /*
            if (message.ToLower() == "exit")
            {
                break;
            }
            */
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        stream.Close();
    }

    static void ReceiveMessages(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                /*if (message == "START")
                {
                    byte[] startMessage = Encoding.UTF8.GetBytes("START");
                    stream.Write(startMessage, 0, startMessage.Length);
                }*/
                //RequestHandler_old.FunctionHandler(message); // Andrebbe messo qui o dopo il while? Da testare/ragionarci
            }
        }
        catch (SocketException)
        {
            // Handle the connection reset error
        }
        finally
        {
            client.Close();
        }
    }
}
