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
Mi collego al server, il collegamento fa si che il client decida se joinare od hostare una lobby
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
            print(f"SERVER: Received message from {client_id}: {message}")
            if "HOST_GAME" in message:
                game_id = utils.generate_game_id()
                player.lobby_id = game_id
                player.player_id = utils.generate_player_id()
                game = Game(game_id)
                games.append(game)
                await websocket.send("LOBBY_ID: " + game_id)
                await websocket.send("PLAYER_ID: " + player.player_id)
                print(f"Game {game_id} created")
                game_task = asyncio.create_task(game.create_game(player))

                await game_task


            elif "JOIN_GAME" in message:

                #Prendo l'id della lobby, se esiste aggiungo il player alla lobby aggiungendolo alla lista contenuta in Game

                player.player_id = utils.generate_player_id()
                await websocket.send("PLAYER_ID: " + player.player_id)
                game_id = message.split(": ")[1]
                for game in games:
                    if game.game_id == game_id:
                        if len(game.players) < 6:
                            print(f"Added player {player} to lobby {game_id}")
                            game.add_player(player)
                            client_task = asyncio.create_task(game.listen_to_player_request(player))
                            await client_task
                        else:
                            print("Lobby is full")


    except websockets.exceptions.ConnectionClosed:
        print(f"Client {player} disconnected")
        clients.remove(player)
    except Exception as e:
        print(f"Unexpected error: {e}")


async def main():
    print("server started")
    #async with websockets.serve(handler, "0.0.0.0", 8766):
    async with websockets.serve(handler, "localhost", 8766):
        await asyncio.Future()  # Run forever


if __name__ == "__main__":
    asyncio.run(main())
