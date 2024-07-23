import asyncio
import websockets
from server_request_handler import RequestHandler

clients = {}
games = {}

async def handler(websocket, path):
    client_id = websocket.remote_address
    clients[client_id] = websocket
    print(f"Client {client_id} connected")

    try:
        async for message in websocket:
            print(f"Received message from {client_id}: {message}")
            if message.startswith("create_game"):
                game_id = message.split()[1]
                if game_id not in games:
                    games[game_id] = RequestHandler(game_id)
                    asyncio.create_task(games[game_id].handle_requests())
                    await games[game_id].add_request(client_id, "create_game")
                    print(f"Game {game_id} created")
            elif message.startswith("join_game"):
                game_id = message.split()[1]
                if game_id in games:
                    await games[game_id].add_request(client_id, "join_game")
                    print(f"Client {client_id} joined game {game_id}")
                else:
                    print(f"Game {game_id} not found")
            elif message.startswith("end_game"):
                game_id = message.split()[1]
                if game_id in games:
                    await games[game_id].add_request(client_id, "end_game")
                    print(f"Client {client_id} requested to end game {game_id}")
            else:
                # Forward message to the appropriate game handler
                for game_id, handler in games.items():
                    await handler.add_request(client_id, message)
    except websockets.exceptions.ConnectionClosed:
        print(f"Client {client_id} disconnected")
    finally:
        del clients[client_id]

async def main():
    async with websockets.serve(handler, "localhost", 8766):
        await asyncio.Future()  # Run forever

if __name__ == "__main__":
    asyncio.run(main())
