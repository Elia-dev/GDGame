using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;

public class RequestHandler
{
    private readonly Channel<(string, string)> _queue = Channel.CreateUnbounded<(string, string)>();
    private string _request;
    ClientManager cm = ClientManager.Instance;
    private GameManager gm = GameManager.Instance;
    public async Task HandleRequests(CancellationToken cancellationToken)
    {
        await foreach (var (clientId, message) in _queue.Reader.ReadAllAsync(cancellationToken))
        {
            //Debug.Log($"Handling request from {clientId}: {message}");
            if (message.Contains("culo"))
            {
                Debug.Log("No culi allowed here");
            }
            else if (message.Contains("cane"))
            {
                Debug.Log("I love dogs, doesn't everyone?");
            }
            else if(message.Contains("LOBBY_ID:")) // Manage lobby_id request
            {
                Debug.Log("Ricevuta richiesta: LOBBY_ID " + message);
                _request = RemoveRequest(message, "LOBBY_ID:");
                //Debug.Log("INFO ESTRAPOLATA:" + _request);
                ClientManager cm = ClientManager.Instance;
                cm.SetLobbyId(_request);
            }
            else if (message.Contains("REQUEST_NAME_UPDATE_PLAYER_LIST:"))
            {
                Debug.Log("Server_Request: REQUEST_NAME_UPDATE_PLAYER_LIST");
                
                gm.ResetPlayersName();
                _request = RemoveRequest(message, "REQUEST_NAME_UPDATE_PLAYER_LIST:");
                //Debug.Log("RIMOSSA RICHIESTA:" + _request);
                string[] str= _request.Split(" ");
                //Debug.Log("ESEGUITO SPLIT: ");
                foreach (var name in str)
                {
                    for (int i = 0; i < name.Length; i++)
                    {
                        if (name[i] == '[' || name[i] == ']' || name[i] == ',')
                        {
                            name.Remove(i);
                        }
                    }
                    
                    //Debug.Log(name);
                    gm.AddPlayerName(name);
                }
                //Debug.Log("Lista players: " + cm.NamePlayersTemporaneo.ToString());
            }
            else if (message.Contains("GAME_ORDER:"))
            {
                Debug.Log("Server_Request: GAME_ORDER");
                _request = RemoveRequest(message, "GAME_ORDER: ");
                gm.setGame_order(_request);
            }
            else if (message.Contains("EXTRACTED_NUMBER:"))
            {
                
                Debug.Log("Server_Request: EXTRACTED_NUMBER");
                _request = RemoveRequest(message, "EXTRACTED_NUMBER: ");
                int numero = int.Parse(_request);
                
                gm.SetExtractedNumber(numero);
            }
            else if (message.Contains("GAME_ORDER_EXTRACTED_NUMBERS:"))
            {
                Debug.Log("Server_Request: GAME_ORDER_EXTRACTED_NUMBERS");
                _request = RemoveRequest(message, "GAME_ORDER_EXTRACTED_NUMBERS: ");
                gm.SetGameOrderExtractedNumbers(_request);
            }
            else if (message.Contains("PLAYER_ID:"))
            {
                Debug.Log("Server_Request: PLAYER_ID");
                _request = RemoveRequest(message, "PLAYER_ID: ");
                Player.Instance.PlayerId = _request;
            }
            else
            {
                Debug.Log("HANDLER: Richiesta non gestibile" + message);
            }
            await Task.Delay(500);
        }
    }

    public void AddRequest(string clientId, string message)
    {
        _queue.Writer.TryWrite((clientId, message));
        //_queue.Writer.WriteAsync((clientId, message));
    }
    
    private string RemoveRequest(string source, string request)
    {
        string value = source.Replace(request, "");
        //Debug.Log("VALORE CALCOLATO:" + value);
        return value;
    }
}