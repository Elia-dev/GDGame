using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class ClientManager
{
    private static ClientManager _instance;
    private static readonly object Lock = new object();
    public List<string> NamePlayersTemporaneo = new List<string>(); 
    
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
    //private string _server = "ws://150.217.51.105:8766";
    //private string _server = "ws://91.80.76.119:12345";
    private string _server = "ws://localhost:8766";
    private ClientWebSocket _webSocket = null;
    private CancellationToken _cancellationToken;
    private string _lobbyID;
    private Player player = Player.Instance;
    

    public bool IsConnected()
    {
        if (_webSocket != null)
        {
            return true;
        }

        return false;
    }
    
    public string GetLobbyId()
    {
        return _lobbyID;
    }
    public void SetLobbyId(string lobbyID)
    {
        _lobbyID = lobbyID;
    }

    public async Task StartClient()
    {
        if (_webSocket == null)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            
            var uri = new Uri(_server);
            
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
            var handlerTask = RequestHandler.HandleRequests(cancellationTokenSource.Token);
            var receiveTask = ReceiveMessage(_webSocket, cancellationTokenSource.Token);
            await Task.WhenAll(handlerTask, receiveTask);
        }
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
            Debug.Log("Received from server: " + response);
            RequestHandler.AddRequest(webSocket.Options.ClientCertificates.ToString(), response);
        }
        Debug.Log("Uscito dal loop delle richieste");
    }
    
    public async void CreateLobbyAsHost()
    {
        await SendMessage(_webSocket, _cancellationToken, "HOST_GAME:"); // Telling the server that I will be the host
    }
    
    public async void JoinLobbyAsClient(string lobbyID)
    {
        await SendMessage(_webSocket, _cancellationToken, "JOIN_GAME: " + lobbyID);
    }
    
    public async void RequestNameUpdatePlayerList()
    {
        await SendMessage(_webSocket, _cancellationToken, "REQUEST_NAME_UPDATE_PLAYER_LIST: ");
    }

    public async void SendName()
    {
        await SendMessage(_webSocket, _cancellationToken, "UPDATE_NAME: " + player.PlayerId + "-"+  player.Name);
    }
    
    public async void StartHostGame()
    {
        await SendMessage(_webSocket, _cancellationToken, "GAME_STARTED_BY_HOST: ");
    }
    
    public async void SendChosenArmyColor()
    {
        await SendMessage(_webSocket, _cancellationToken, "CHOSEN_ARMY_COLOR: " + player.PlayerId + "-" + player.ArmyColor);
    }

    public async void UpdateTerritoriesState()
    {
        await SendMessage(_webSocket, _cancellationToken, "UPDATE_TERRITORIES_STATE: " + player.PlayerId + ", " + JsonConvert.SerializeObject(player.Territories));
    }

    public async void RequestTerritoryInfo(string id)
    {
        await SendMessage(_webSocket, _cancellationToken, "REQUEST_TERRITORY_INFO: " + player.PlayerId + "-" + id);
    
    }
}
