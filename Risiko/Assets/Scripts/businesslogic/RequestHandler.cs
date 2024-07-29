using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public class RequestHandler
{
    private readonly Channel<(string, string)> _queue = Channel.CreateUnbounded<(string, string)>();
    private string request;
    public async Task HandleRequests(CancellationToken cancellationToken)
    {
        await foreach (var (clientId, message) in _queue.Reader.ReadAllAsync(cancellationToken))
        {
            Console.WriteLine($"Handling request from {clientId}: {message}");
            if (message.Contains("culo"))
            {
                Console.WriteLine("No culi allowed here");
            }
            if (message.Contains("cane"))
            {
                Console.WriteLine("I love dogs, doesn't everyone?");
            }
            if(message.Contains("LOBBY_ID:")) // Manage lobby_id request
            {
                request = removeRequest(message, "Lobby_id:");
                ClientManager.Instance.setLobbyId(request);
            
                // Setta l'id della lobby nella grafica
                // e nello stato del giocatore, ma ci serve davvero nello stato del giocatore se già ce l'abbiamo nel client manager?
                // forse no, si risolve qualche problema di concorrenza
                //someClass.updateUI()
            }
            await Task.Delay(5000);
        }
    }

    public async Task AddRequest(string clientId, string message)
    {
        await _queue.Writer.WriteAsync((clientId, message));
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