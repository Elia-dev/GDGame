using System;
using System.Threading.Channels;
using System.Threading.Tasks;


public class RequestHandler
{
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
            await Task.Delay(5000);
        }
    }

    public async Task AddRequest(string clientId, string message)
    {
        await _queue.Writer.WriteAsync((clientId, message));
    }
}