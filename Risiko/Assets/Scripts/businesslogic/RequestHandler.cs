using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;

public class RequestHandler
{
    private readonly Channel<(string, string)> _queue = Channel.CreateUnbounded<(string, string)>();
    private string request;
    public async Task HandleRequests(CancellationToken cancellationToken)
    {
        await foreach (var (clientId, message) in _queue.Reader.ReadAllAsync(cancellationToken))
        {
            Debug.Log($"Handling request from {clientId}: {message}");
            if (message.Contains("culo"))
            {
                Debug.Log("No culi allowed here");
            }
            if (message.Contains("cane"))
            {
                Debug.Log("I love dogs, doesn't everyone?");
            }
            if(message.Contains("LOBBY_ID:")) // Manage lobby_id request
            {
                Debug.Log("Arrivato messaggio: " + message);
                request = removeRequest(message, "LOBBY_ID:");
                Debug.Log("Info estrapolata:" + request);
                ClientManager cm = ClientManager.Instance;
                cm.setLobbyId(request);
                
                // Setta l'id della lobby nella grafica
                // e nello stato del giocatore, ma ci serve davvero nello stato del giocatore se già ce l'abbiamo nel client manager?
                // forse no, si risolve qualche problema di concorrenza
                //someClass.updateUI()
            }
            await Task.Delay(500);
        }
    }

    public void AddRequest(string clientId, string message)
    {
        _queue.Writer.TryWrite((clientId, message));
        //_queue.Writer.WriteAsync((clientId, message));
    }
    
    private static string removeRequest(string source, string input)
    {
        string value = null;
        // Trova la posizione di "input:" e calcola l'inizio del valore
        int startIndex = input.IndexOf(source) + source.Length;

        // Estrai il valore e rimuovi eventuali spazi
        value = input.Substring(startIndex).Trim();
        
        return value;
    }
}