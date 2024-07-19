using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;



public class WebSocketClient
{
    private static readonly RequestHandler requestHandler = new RequestHandler();

    private static async Task SendMessages(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("Enter message to send: ");
            var message = await Task.Run(() => Console.ReadLine(), cancellationToken);
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    private static async Task ReceiveMessages(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received from server: {response}");
            await requestHandler.AddRequest(webSocket.Options.ClientCertificates.ToString(), response);
        }
    }

    public static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var handlerTask = requestHandler.HandleRequests(cancellationTokenSource.Token);
        var uri = new Uri("ws://localhost:8766");

        using (var webSocket = new ClientWebSocket())
        {
            await webSocket.ConnectAsync(uri, cancellationTokenSource.Token);

            var sendTask = SendMessages(webSocket, cancellationTokenSource.Token);
            var receiveTask = ReceiveMessages(webSocket, cancellationTokenSource.Token);

            await Task.WhenAll(sendTask, receiveTask);
        }

        await handlerTask;
    }
}
