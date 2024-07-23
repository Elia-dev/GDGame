import asyncio
import websockets
from server_request_handler import RequestHandler
import utils
from Player import Player
from Game import Game
clients = []
games = []
'''
La logica è questa:
Mi collego al server, il collegamento fa si che il client chieda se joinare od hostare una lobby
Il main del server avrà l'elenco di tutti i client connessi e di tutte le partite in corso
Ogni volta che il client hosta viene creato un game ed aggiunto alla lista
Ogni game sarà un thread a sé che gestisce i messaggi tramite il proprio handler che andrà messo 
dentro la classe Game e lo stato del gioco, 
La classe Game a sua volta avrà la lista dei giocatori che fanno parte di quella partita, ogni player avrà salvato la propria websocket
'''

async def handler(websocket, path):

    client_id = websocket.remote_address
    player = Player(websocket)
    clients.append(player)
    print(f"Client {client_id} connected")

    try:
        async for message in websocket:
            print(f"Received message from {client_id}: {message}")
            if message.contains("HOST_GAME"):
                game_id = utils._generate_game_id()
                player.lobby_id = game_id
                game = Game(game_id)
                games.append(game)
                await asyncio.create_task(games.index(game).handle_game())
                #await games[game_id].add_request(client_id, "create_game")
                print(f"Game {game_id} created")
            elif message.startswith("JOIN_GAME"):
                #Join game ancora da fare
                game_id = message.split()[1]
                if game_id in games:
                    await games[game_id].add_request(client_id, "join_game")
                    print(f"Client {client_id} joined game {game_id}")
                else:
                    print(f"Game {game_id} not found")
    except websockets.exceptions.ConnectionClosed:
        print(f"Client {client_id} disconnected")
    finally:
        #Da finire
        del clients[client_id]

async def main():
    async with websockets.serve(handler, "localhost", 8766):
        await asyncio.Future()  # Run forever

if __name__ == "__main__":
    asyncio.run(main())
