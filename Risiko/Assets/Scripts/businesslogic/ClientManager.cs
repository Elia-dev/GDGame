using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;


public class ClientManager
{
    private static ClientManager _instance;
    private static readonly object Lock = new object();
    
    private ClientManager() // Private constructor to allow instantiation using singleton only
    {
    }
    public static ClientManager Instance // Implementing singleton pattern
    {
        get
        {
            // Using lock to manage concurrency
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new ClientManager();
                }
                return _instance;
            }
        }
    }
    private static readonly RequestHandler RequestHandler = new RequestHandler();
    private string _server = "ws://localhost:8766";
    private ClientWebSocket _webSocket = null;
    private CancellationToken _cancellationToken;
    private string lobby_id;

    public bool IsConnected()
    {
        if (_webSocket != null)
        {
            return true;
        }

        return false;
    }
    public string getLobbyId()
    {
        return lobby_id;
    }
    public void setLobbyId(string lobby_id)
    {
        this.lobby_id = lobby_id;
    }

    public async Task StartClient()
    {
        
        var cancellationTokenSource = new CancellationTokenSource();

        var handlerTask = RequestHandler.HandleRequests(cancellationTokenSource.Token);
        var uri = new Uri(_server);

        using ( _webSocket = new ClientWebSocket())
        {
            
            await _webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
            
            var receiveTask = ReceiveMessage(_webSocket, cancellationTokenSource.Token);

            await Task.WhenAll(receiveTask);
        }

        await handlerTask;
    }
    
    private static async Task SendMessage(ClientWebSocket webSocket, CancellationToken cancellationToken, string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
    }

    
    private static async Task ReceiveMessage(ClientWebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received from server: {response}");
            await RequestHandler.AddRequest(webSocket.Options.ClientCertificates.ToString(), response);
        }
    }
    
    
    public void CreateLobbyAsHost()
    {
        SendMessage(_webSocket, _cancellationToken, "HOST: " + Player.Instance.PlayerId); // Telling the server that I will be the host
    }
    /*
    public void JoinLobbyAsClient(string LobbyID)
    {
        Send("CLIENT - LOBBY_ID: " + LobbyID);
    }
    */
}
