using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public class WebSocketServer
{
    private static readonly ConcurrentDictionary<string, WebSocket> Clients = new ConcurrentDictionary<string, WebSocket>();
    private static readonly RequestHandler RequestHandler = new RequestHandler();

    private static async Task Handler(HttpListenerContext context, WebSocket webSocket)
    {
        var clientId = context.Request.RemoteEndPoint.ToString();
        Clients[clientId] = webSocket;
        Console.WriteLine($"Client {clientId} connected");

        try
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message from {clientId}: {message}");
                await RequestHandler.AddRequest(clientId, message);

                foreach (var client in Clients)
                {
                    if (client.Key != clientId)
                    {
                        await client.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Message from {clientId}: {message}")), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }
        catch (WebSocketException)
        {
            Console.WriteLine($"Client {clientId} disconnected");
        }
        finally
        {
            Clients.TryRemove(clientId, out _);
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    private static async Task SendMessagesToClients()
    {
        while (true)
        {
            Console.Write("Enter message to send to clients: ");
            var message = await Task.Run(() => Console.ReadLine());
            foreach (var client in Clients)
            {
                await client.Value.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Server: {message}")), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var handlerTask = RequestHandler.HandleRequests(cancellationTokenSource.Token);
        var inputTask = SendMessagesToClients();

        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:8765/");
        httpListener.Start();
        Console.WriteLine("Server started at ws://localhost:8765");

        while (true)
        {
            var context = await httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                _ = Handler(context, webSocketContext.WebSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }

        await handlerTask;
        await inputTask;
    }
}
