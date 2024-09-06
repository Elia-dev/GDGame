import asyncio
import websockets
import utils
from Player import Player
from Game import Game

# 01/09/2024 gioco funzionante con fase di rinforzo eseguibile singolarmente in funzione del turno (NO SIMULTANEO)
#Creazione di un nuovo branch per migrare su quella versione del gioco

games = []
'''
La logica è questa:
Mi collego al server, il collegamento fa si che il client decida se joinare od hostare una lobby
Il main del server avrà l'elenco di tutte le partite in corso
Ogni volta che il client hosta viene creato un game ed aggiunto alla lista
Ogni game sarà un task a sé che gestisce i messaggi tramite il proprio handler che andrà messo 
dentro la classe Game e lo stato del gioco, 
La classe Game avrà la lista dei giocatori che fanno parte di quella partita, ogni player avrà salvato la propria websocket
'''


async def handler(websocket):
    client_id = websocket.remote_address
    player = Player(websocket)
    print(f"Client {client_id} connected")

    try:
        async for message in websocket:
            print(f"SERVER: Received message from {client_id}: {message}")
            games_to_remove = [game for game in games if game.game_id is None or len(game.players) == 0] #Check if there are empty lobby and delete them
            for game in games_to_remove:
                games.remove(game)
            print("Pulizia completata")

            if "HOST_GAME" in message:
                game_id = None
                while game_id is None: #Evita duplicati
                    game_id = utils.generate_game_id()
                    for game in games:
                        if game.game_id == game_id:
                            game_id = None
                player.lobby_id = game_id
                player.player_id = utils.generate_player_id()
                game = Game(game_id)
                games.append(game)
                await websocket.send("LOBBY_ID: " + game_id)
                await websocket.send("PLAYER_ID: " + player.player_id)
                print(f"Game {game_id} created")
                game_task = asyncio.create_task(game.create_game(player))

                try:
                    await game_task
                except Exception as e:
                    print(f"Errore durante l'esecuzione del task del gioco: {e}")


            elif "JOIN_GAME" in message:
                joined = False
                foundGame = False
                #Prendo l'id della lobby, se esiste aggiungo il player alla lobby aggiungendolo alla lista contenuta in Game

                player.player_id = utils.generate_player_id()
                await websocket.send("PLAYER_ID: " + player.player_id)
                game_id = message.split(": ")[1]
                for game in games:
                    if game.game_id == game_id:
                        foundGame = True
                        if len(game.players) < 6 and game.game_waiting_to_start is True:
                            print(f"Added player {player} to lobby {game_id}")
                            game.add_player(player)
                            client_task = asyncio.create_task(game.listen_to_player_request(player))
                            joined = True
                            await player.sock.send("CONNECTED_TO_LOBBY")
                            await client_task
                        else:
                            joined = False
                            await player.sock.send("CONNECTION_REFUSED")
                            print("Lobby is full or the game is already started")
                if not foundGame:
                    print("Unable to find the lobby")
                    await player.sock.send("CONNECTION_REFUSED")
                if foundGame and not joined:
                    await player.sock.send("CONNECTION_REFUSED")
            elif "SELECT_ALL_GAMES" in message:
                response = []
                #Voglio mandare id lobby, numPlayers, hostName
                for game in games:
                    if len(game.players) < 6 and game.game_waiting_to_start is True:
                        response.append(game.game_id)
                        response.append(game.host_player.name)
                        response.append(len(game.players))
                print("Mi è stata chiesta la lista di tutte le lobby attive, rispondo con: " + response.__str__())
                if response:
                    await websocket.send("SELECT_ALL_GAMES: " + response.__str__())
                else:
                    print("Non mando niente tanto non c'è nessuna partita")
                


    except websockets.exceptions.ConnectionClosed as e:
        print(f"Client {player.name} disconnected")
        print(f"Connection closed: {e}")
    except asyncio.CancelledError as e:
        print(f"Task cancellato {e}")
    except Exception as e:
        print(f"Unexpected error: {e}")
    finally:
        await websocket.close()  # Chiusura esplicita della WebSocket
        print("WebSocket chiusa e risorse liberate")

async def shutdown(server):
    server.close()
    await server.wait_closed()

async def shutdown_all_games():
    for game in games:
        game.end_game()
        for player in game.players:
            await player.sock.close()
    games.clear()
    print("All games have been shut down.")

async def shutdown_all_clients():
    for game in games:
        for player in game.players:
            await player.sock.close()
    games.clear()
    print("All clients have been disconnected.")

async def shutdown_all(server, input_task):
    await shutdown_all_games()
    await shutdown(server)
    input_task.cancel()
    await input_task
    print("Server and all tasks have been shut down.")


async def handle_input(server, input_task):
    while True:
        user_input = await asyncio.get_event_loop().run_in_executor(None, input, "Enter command: ")
        print(f"Received input: {user_input}")
        if user_input == "quit":
            await shutdown_all(server, input_task)
            break

        elif user_input == "list":
            print("Games:")
            for game in games:
                print(game.game_id)
        elif user_input == "players":
            print("Players:")
            for game in games:
                for player in game.players:
                    print(f"{player.name}-{player.lobby_id}-{player.player_id}")
        elif user_input == "remove":
            for game in games:
                if len(game.players) == 0:
                    games.remove(game)
        elif user_input == "help":
            print("Commands: list, players, remove, exit, help, test, kill_lobby <lobby_id>, kick_player <player_id>")
        elif user_input == "test":
            print("This is a test")
        elif "kill_lobby" in user_input:
            lobby_id = user_input.split(" ")[1]
            for game in games:
                if game.game_id == lobby_id:
                    game.end_game()
        elif "kick_player" in user_input:
            player_id = user_input.split(" ")[1]
            for game in games:
                for player in game.players:
                    if player.player_id == player_id:
                        await player.sock.send("GAME_KILLED_BY_HOST")
                        game.remove_player(player)

        else:
            print("Unknown command")

async def main():
    print("server started")

    # Gestisce il timeout della connessione mandando ogni 5 minuti un ping e aspettando il pong di risposta entro altri 5 minuti
    async with websockets.serve(handler, "0.0.0.0", 12345, ping_interval=300, ping_timeout=300) as server:

      # await asyncio.Future()  # Run forever
      input_task = asyncio.create_task(handle_input(server, asyncio.current_task()))

      try:
          await asyncio.Future()  # Run forever
      except asyncio.CancelledError:
          await shutdown(server)
          input_task.cancel()
          await input_task


if __name__ == "__main__":
    asyncio.run(main())
