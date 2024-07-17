import asyncio
import websockets
from server_request_handler import RequestHandler


clients = {}
request_handler = RequestHandler()


async def handler(websocket, path):
    client_id = websocket.remote_address
    clients[client_id] = websocket
    print(f"Client {client_id} connected")

    try:
        async for message in websocket:
            print(f"Received message from {client_id}: {message}")
            await request_handler.add_request(client_id, message)
            # Invia il messaggio a tutti i client connessi (broadcast)
            for client, conn in clients.items():
                if conn != websocket:
                    await conn.send(f"Message from {client_id}: {message}")
    except websockets.exceptions.ConnectionClosed:
        print(f"Client {client_id} disconnected")
    finally:
        del clients[client_id]


async def send_messages_to_clients():
    while True:
        message = await asyncio.to_thread(input, "Enter message to send to clients: ")
        # Invia il messaggio a tutti i client connessi
        for client, conn in clients.items():
            await conn.send(f"Server: {message}")


async def main():
    handler_task = asyncio.create_task(request_handler.handle_requests())
    input_task = asyncio.create_task(send_messages_to_clients())
    async with websockets.serve(handler, "localhost", 8765):
        await asyncio.Future()  # Run forever
    await handler_task  # Assicura che il task del request handler venga atteso
    await input_task  # Assicura che il task dell'input venga atteso


# Avvia il server
asyncio.run(main())
