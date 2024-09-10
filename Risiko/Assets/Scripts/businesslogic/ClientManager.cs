using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace businesslogic
{
    public class ClientManager
    {
        private static ClientManager _instance;
        private static readonly object Lock = new object();
        private bool _connected = false;
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
        //private string _server = "ws://150.217.51.105:12345"; // 105  
        //private string _server = "ws://93.57.245.63:12345"; // Elia
        //private string _server = "ws://101.58.64.113:12345"; // Filo
        //private string _server = "ws://128.116.252.173:12345"; // Franci
        private string _server = "ws://localhost:12345";
        private ClientWebSocket _webSocket = null;
        private CancellationToken _cancellationToken;
        private bool _isConnectedToLobby = false;
        private List<string> onlineServers = new List<string>();
        public List<string> GetOnlineServersList()
        {
            return onlineServers;
        }
        
        private static readonly List<string> ServersToCheck = new List<string>
        {
            "ws://150.217.51.105:12345", // 105
            "ws://93.57.245.63:12345", // Elia
            "ws://101.58.64.113:12345", // Filo
            "ws://128.116.252.173:12345", // Franci
            "ws://localhost:12345"
        };

        public async Task<List<string>> FetchOnlineServers()
        {
            onlineServers = new List<string>();
            foreach (var server in ServersToCheck)
            {
                if (await IsServerOnline(server))
                {
                    string cleanServer = server.Replace("ws://", "").Replace(":12345", "");
                    onlineServers.Add(cleanServer);
                }
            }
            
            return onlineServers;
        }

        private async Task<bool> IsServerOnline(string serverUri)
        {
            using ClientWebSocket webSocket = new ClientWebSocket();
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(4));
            try
            {
                await webSocket.ConnectAsync(new Uri(serverUri), cancellationTokenSource.Token);
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to connect to {serverUri}: {ex.Message}");
            }

            return false;
        }
        public void SetActiveServer(string server)
        {
            _server = "ws://" + server + ":12345";
        }

        public string GetActiveServer()
        {
            return _server;
        }

        public bool IsConnectedToLobby()
        {
            return _isConnectedToLobby;
        }

        public void setIsConnectedToLobby(bool isConnectedToLobby)
        {
            _isConnectedToLobby = isConnectedToLobby;
        }
        public bool IsConnected()
        {
            return _connected;
        }

        public void SetConnected(bool value)
        {
            _connected = value;
        }

        public async void ResetConnection()
        {
            //await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _connected = false;
            _webSocket = null;
        }

        public async Task StartClient()
        {
            if (_webSocket == null)
            {
                var cancellationTokenSourceFirstConnection = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                var cancellationTokenSource = new CancellationTokenSource();
                var uri = new Uri(_server);
            
                _webSocket = new ClientWebSocket();
            
                try
                {
                    //await _webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
                    _webSocket.Options.KeepAliveInterval = TimeSpan.FromMinutes(2.5); // invia un ping ogni 2.5 minuti
                    await _webSocket.ConnectAsync(uri, cancellationTokenSourceFirstConnection.Token);
            
                    if (_webSocket.State == WebSocketState.Open)
                    {
                        Debug.Log("Connected");
                        SetConnected(true);
                    
                        var handlerTask = RequestHandler.HandleRequests(cancellationTokenSource.Token);
                        var receiveTask = ReceiveMessage(_webSocket, cancellationTokenSource.Token);
                        await Task.WhenAll(handlerTask, receiveTask);
                    }
                    else
                    {
                        Debug.Log("[ClientManager] WebSocket connection could not be established.");
                        _webSocket = null;
                        SetConnected(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("WebSocket connection was canceled due to timeout.");
                    _webSocket = null;
                    SetConnected(false);
                }
                catch (WebSocketException ex)
                {
                    Debug.Log($"WebSocketException: {ex.Message}");
                    _webSocket = null;
                    SetConnected(false);
                }
                catch (Exception ex)
                {
                    Debug.Log($"Exception: {ex.Message}");
                    _webSocket = null;
                    SetConnected(false);
                }
            }
        }
    
        private static async Task SendMessage(ClientWebSocket webSocket, CancellationToken cancellationToken, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
        }
    
        private static async Task ReceiveMessage(ClientWebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 24];
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                var response = Encoding.UTF8.GetString(buffer, 0, result.Count);
                RequestHandler.AddRequest(webSocket.Options.ClientCertificates.ToString(), response);
            }
            Debug.Log("Uscito dal loop delle richieste");
        }

        public async void RequestAllGames()
        {
            await SendMessage(_webSocket, _cancellationToken, "SELECT_ALL_GAMES");
        }
        public async void CreateLobbyAsHost()
        {
            await SendMessage(_webSocket, _cancellationToken, "HOST_GAME:"); // Telling the server that I will be the host
        }

        public async void KillLobby()
        {
            setIsConnectedToLobby(false);
            await SendMessage(_webSocket, _cancellationToken, "GAME_KILLED_BY_HOST: " + Player.Instance.PlayerId);
        }

        public async void LeaveLobby()
        {
            setIsConnectedToLobby(false);
            await SendMessage(_webSocket, _cancellationToken, "PLAYER_HAS_LEFT_THE_LOBBY: " + Player.Instance.PlayerId);
        }
    
        public async void LeaveGame()
        {
            setIsConnectedToLobby(false);
            await SendMessage(_webSocket, _cancellationToken, "PLAYER_HAS_LEFT_THE_GAME: " + Player.Instance.PlayerId);
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
            await SendMessage(_webSocket, _cancellationToken, "UPDATE_NAME: " + Player.Instance.PlayerId + "-"+  Player.Instance.Name);
        }
    
        public async void StartHostGame()
        {
            await SendMessage(_webSocket, _cancellationToken, "GAME_STARTED_BY_HOST: ");
        }
    
        public async void SendChosenArmyColor()
        {
            await SendMessage(_webSocket, _cancellationToken, "CHOSEN_ARMY_COLOR: " + Player.Instance.PlayerId + "-" + Player.Instance.ArmyColor);
        }

        public async void UpdateTerritoriesState()
        {
            await SendMessage(_webSocket, _cancellationToken, "UPDATE_TERRITORIES_STATE: " + Player.Instance.PlayerId + ", " + JsonConvert.SerializeObject(Player.Instance.Territories));
        }

        public async void AttackEnemyTerritory(Territory myTerritory, Territory enemyTerritory, int myNumArmy)
        {
            int enemyArmyNum;
            if (enemyTerritory.num_tanks >= 3)
            {
                enemyArmyNum = 3;
            }
            else
            {
                enemyArmyNum = enemyTerritory.num_tanks;
            }
        
            GameManager.Instance.setEnemyArmyNum(enemyArmyNum);
            GameManager.Instance.setMyArmyNum(myNumArmy);
            GameManager.Instance.setImUnderAttack(false);
            GameManager.Instance.setMyTerritory(myTerritory);
            GameManager.Instance.setEnemyTerritoy(enemyTerritory);
        
            await SendMessage(_webSocket, _cancellationToken, "ATTACK_TERRITORY: " + 
                                                              Player.Instance.PlayerId + "-" + enemyTerritory.player_id + ", " 
                                                              + myTerritory.id + "-" + enemyTerritory.id + ", " + 
                                                              myNumArmy.ToString() + "-" + enemyArmyNum.ToString());
            //GameManager.Instance.setImAttacking(true); SPOSTATO IN requestHandler
        }
    
        public async void RequestTerritoryInfo(string Terr_id)
        {
            await SendMessage(_webSocket, _cancellationToken, "REQUEST_TERRITORY_INFO: " + Player.Instance.PlayerId + "-" + Terr_id);
    
        }

        public async void RequestAddBot()
        {
            await SendMessage(_webSocket, _cancellationToken, "ADD_BOT");
        }

        public async void RequestRemoveBot()
        {
            await SendMessage(_webSocket, _cancellationToken, "REMOVE_BOT");
        }

    }
}
