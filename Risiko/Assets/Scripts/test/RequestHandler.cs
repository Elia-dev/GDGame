using System;
using System.Threading;
//using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;
//using WebSocketSharp;
public class RequestHandler
{/*
    private readonly Channel<(string, string)> _queue = Channel.CreateUnbounded<(string, string)>();

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
            // Qui puoi aggiungere la logica per gestire il messaggio
            // Ad esempio, rispondere al client, fare una richiesta ad un altro servizio, ecc.
            await Task.Delay(5000); // Simula il tempo di gestione della richiesta
        }
    }

    public async Task AddRequest(string clientId, string message)
    {
        await _queue.Writer.WriteAsync((clientId, message));
    }*/
}
