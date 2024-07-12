using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void ReceiveMessages(TcpClient client)
    {
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
                Console.WriteLine($"SERVER: {message}");
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

    static void StartClient(string host = "127.0.0.1", int port = 1234)
    {
        TcpClient client = new TcpClient();
        client.Connect(host, port);

        Console.WriteLine("Type 'exit' anytime to quit");

        Thread receiverThread = new Thread(() => ReceiveMessages(client));
        receiverThread.Start();

        NetworkStream stream = client.GetStream();
        string message;
        
        while ((message = Console.ReadLine()) != null)
        {
            if (message.ToLower() == "exit")
            {
                break;
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        client.Close();
    }

    static void Main(string[] args)
    {
        StartClient();
    }
}