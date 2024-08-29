from client_request_handler import RequestHandler
import asyncio
import websockets


request_handler = RequestHandler()

async def send_messages(websocket):
    while True:
        try:
            message = await asyncio.to_thread(input, "Enter message to send: ")
            await websocket.send(message)
        except Exception as e:
            print(f"Error sending message: {e}")
            break

async def receive_messages(websocket):
    while True:
        try:
            response = await websocket.recv()
            print(f"Received from server: {response}")
            await request_handler.add_request(websocket.remote_address, response)
        except Exception as e:
            print(f"Error receiving message: {e}")
            break

async def main():
    handler_task = asyncio.create_task(request_handler.handle_requests())
   # uri = "ws://localhost:8766" #150.217.51.105
    #uri = "ws://93.57.245.63:12345"
    #uri = "ws://localhost:12345"
    uri = "ws://101.58.64.113:12345"
    async with websockets.connect(uri) as websocket:
        send_task = asyncio.create_task(send_messages(websocket))
        receive_task = asyncio.create_task(receive_messages(websocket))
        await asyncio.gather(send_task, receive_task)
    await handler_task  # Assicura che il task del request handler venga atteso

# Avvia il client
asyncio.run(main())

