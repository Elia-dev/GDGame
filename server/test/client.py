from client_request_handler import RequestHandler
import asyncio
import websockets


request_handler = RequestHandler()

async def send_messages(websocket):
    while True:
        message = await asyncio.to_thread(input, "Enter message to send: ")
        await websocket.send(message)

async def receive_messages(websocket):
    while True:
        response = await websocket.recv()
        print(f"Received from server: {response}")
        await request_handler.add_request(websocket.remote_address, response)

async def main():
    handler_task = asyncio.create_task(request_handler.handle_requests())
    uri = "ws://localhost:8765"
    async with websockets.connect(uri) as websocket:
        send_task = asyncio.create_task(send_messages(websocket))
        receive_task = asyncio.create_task(receive_messages(websocket))
        await asyncio.gather(send_task, receive_task)
    await handler_task  # Assicura che il task del request handler venga atteso

# Avvia il client
asyncio.run(main())

