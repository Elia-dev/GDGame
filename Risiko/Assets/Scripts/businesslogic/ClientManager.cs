using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

public class ClientManager
{
    private string host = "127.0.0.1";
    private int port = 1234;
    private TcpClient client;
    public TcpClient Client
    {get { return client; } }

    public void StartClient()
    {
        client = new TcpClient();
        client.Connect(this.host, this.port);

        Console.WriteLine("Type 'exit' anytime to quit");

        Thread receiverThread = new Thread(new ParameterizedThreadStart(ReceiveMessages));
        receiverThread.Start(client);
    }

    public string StartHost(string name)
    {
        return "HOST ID";
    }

    public void JoinHost(string name, string LobbyID)
    {

    }

    public void Send(string message)
    {
        NetworkStream stream = this.client.GetStream();
        while ((message = Console.ReadLine()) != null)
        {
            if (message.ToLower() == "exit")
            {
                break;
            }

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
                if (message == "START")
                {
                    byte[] startMessage = Encoding.UTF8.GetBytes("START");
                    stream.Write(startMessage, 0, startMessage.Length);
                }
                RequestHandler.FunctionHandller(message);
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
