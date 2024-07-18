using System;
using WebSocketSharp;

public class Client
{
    public static void Main(string[] args)
    {
        using var ws = new WebSocket("ws://localhost:9006/Test");
        ws.OnMessage += (sender, e) => Console.WriteLine("Received: " + e.Data);

        ws.Connect();
        ws.Send("Data from client");

        Console.ReadKey(true);
        ws.Close();
    }
}