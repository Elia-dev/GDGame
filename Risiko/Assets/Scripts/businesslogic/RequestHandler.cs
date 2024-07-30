using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;

public class RequestHandler
{
    private readonly Channel<(string, string)> _queue = Channel.CreateUnbounded<(string, string)>();
    private string _request;
    public async Task HandleRequests(CancellationToken cancellationToken)
    {
        await foreach (var (clientId, message) in _queue.Reader.ReadAllAsync(cancellationToken))
        {
            Debug.Log($"Handling request from {clientId}: {message}");
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
                //Debug.Log("ARRIVATO MESSAGGIO LOBBY: " + message);
                _request = RemoveRequest(message, "LOBBY_ID:");
                //Debug.Log("INFO ESTRAPOLATA:" + _request);
                ClientManager cm = ClientManager.Instance;
                cm.setLobbyId(_request);
            }
            else if (message.Contains("REQUEST_NAME_UPDATE_PLAYER_LIST:"))
            {
                Debug.Log("Ricevuta richiesta: REQUEST_NAME_UPDATE_PLAYER_LIST");
                ClientManager cm = ClientManager.Instance;
                cm.NamePlayersTemporaneo.Clear();
                _request = RemoveRequest(message, "REQUEST_NAME_UPDATE_PLAYER_LIST:");
                Debug.Log("RIMOSSA RICHIESTA:" + _request);
                string[] str= _request.Split(" ");
                Debug.Log("ESEGUITO SPLIT: ");
                foreach (var name in str)
                {
                    for (int i = 0; i < name.Length; i++)
                    {
                        if (name[i] == '[' || name[i] == ']' || name[i] == ',')
                        {
                            name.Remove(i);
                        }
                    }
                    
                    Debug.Log(name);
                    cm.NamePlayersTemporaneo.Add(name);
                }
                Debug.Log("Lista players: " + cm.NamePlayersTemporaneo.ToString());
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