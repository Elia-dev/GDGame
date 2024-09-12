import asyncio
import websockets
import utils
from Player import Player
from Game import Game

games = []

async def handler(websocket):
    client_id = websocket.remote_address
    player = Player(websocket)
    print(f"Client {client_id} connected")

    try:
        async for message in websocket:
            remove_empty_games()

            if "HOST_GAME" in message:
                game_id = None
                while game_id is None: #Avoid duplicate game_id
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
                    print(f"Unexpected error in game: {e}")


            elif "JOIN_GAME" in message:
                joined = False
                foundGame = False

                player.player_id = utils.generate_player_id()
                await websocket.send("PLAYER_ID: " + player.player_id)
                game_id = message.split(": ")[1]
                for game in games:
                    if game.game_id == game_id:
                        foundGame = True
                        if len(game.players) < 6 and game.game_waiting_to_start is True:
                            game.add_player(player)
                            client_task = asyncio.create_task(game.listen_to_player_request(player))
                            joined = True
                            await player.sock.send("CONNECTED_TO_LOBBY")
                            await client_task
                        else:
                            joined = False
                            await player.sock.send("CONNECTION_REFUSED")
                if not foundGame:
                    await player.sock.send("CONNECTION_REFUSED")
                if foundGame and not joined:
                    await player.sock.send("CONNECTION_REFUSED")
            elif "SELECT_ALL_GAMES" in message:
                response = []
                for game in games:
                    if len(game.players) < 6 and game.game_waiting_to_start is True:
                        response.append(game.game_id)
                        response.append(game.host_player.name)
                        response.append(len(game.players))
                if response:
                    await websocket.send("SELECT_ALL_GAMES: " + response.__str__())

    except websockets.exceptions.ConnectionClosed as e:
        print(f"Client {player.name} disconnected")
        print(f"Connection closed: {e}")
    except asyncio.CancelledError as e:
        print(f"Unexpected error: {e}")
    except Exception as e:
        print(f"Unexpected error: {e}")
    finally:
        await websocket.close()
        print("Connection closed")

def remove_empty_games():
    games_to_remove = [game for game in games if game.game_id is None or len(game.players) == 0] #Check if there are empty lobby and delete them
    for game in games_to_remove:
        games.remove(game)

async def shutdown(server):
    print("Shutting down server...")
    server.close()
    try:
        await asyncio.wait_for(server.wait_closed(), 3)
    except asyncio.TimeoutError:
        print("Server closed due to timeout...")
    print("Server has been shut down.")

async def shutdown_all_games():
    print("Shutting down all games...")
    for game in games:
        try:
            await asyncio.wait_for(game.kill_game(), 3)
        except asyncio.TimeoutError:
            print("Closed game due to timeout...")
    print("All games have been shut down.")

async def shutdown_all_clients():
    print("Shutting down all clients...")
    for game in games:
        for player in game.players:
            try:
                await asyncio.wait_for(player.sock.close(), 3)
            except asyncio.TimeoutError:
                print("Closed client due to timeout...")
    games.clear()
    print("All clients have been disconnected.")

async def shutdown_all(server, input_task):
    await shutdown_all_games()
    await shutdown_all_clients()
    await shutdown(server)
    input_task.cancel()
    await input_task

async def handle_input(server, input_task):
    is_running = True
    print("Type 'help' for a list of commands")
    while is_running:
        user_input = await asyncio.get_event_loop().run_in_executor(None, input, "Enter command: ")
        if user_input == "quit":
            is_running = False
            await shutdown_all(server, input_task)

        elif user_input == "games":
            print("Games:")
            for game in games:
                print(f"lobby id:{game.game_id}, players: {len(game.players)}")
        elif user_input == "players":
            print("Players:")
            for game in games:
                for player in game.players:
                    print(f"Name: {player.name}, id lobby: {player.lobby_id}, player id: {player.player_id}")
        elif user_input == "force_remove_empty_games":
            count = 0
            for game in games:
                if len(game.players) == 0 or game.lobby_id is None:
                    count += 1
                    games.remove(game)
            print(f"Removed {count} empty games")
        elif user_input == "help":
            print("Commands: games, players, force_remove_empty_games, quit, help, test, kill_lobby <lobby_id>, kick_player <player_id>")
        elif "help" in user_input:
            command = user_input.split(" ")[1]
            if command == "games":
                print("Prints all the games, including their lobby id and number of players")
            elif command == "players":
                print("Prints all the players, including their lobby id and player id")
            elif command == "force_remove_empty_games":
                print("Force the server to remove all the empty games")
            elif command == "quit":
                print("Shuts down the server and disconnects all the clients, all the games will be ended")
            elif command == "help":
                print("Prints all the commands")
            elif command == "test":
                print("This is a test, it will print 'This is a test'")
            elif command == "kill_lobby":
                print("Kills a lobby with the specified id, all the players will be disconnected")
            elif command == "kick_player":
                print("Kicks a player with the specified id from the game, the player will be disconnected")
            else:
                print("Unknown command")
        elif user_input == "test":
            print("This is a test")
        elif "kill_lobby" in user_input:
            lobby_id = user_input.split(" ")[1]
            for game in games:
                if game.game_id == lobby_id:
                    await game.end_game()
        elif "kick_player" in user_input:
            player_id = user_input.split(" ")[1]
            for game in games:
                for player in game.players:
                    if player.player_id == player_id:
                        await player.sock.send("KICKED_FROM_GAME")
                        game.remove_player(player)
        else:
            print("Unknown command")

async def main():
    print("server started")

    async with websockets.serve(handler, "0.0.0.0", 12345, ping_interval=300, ping_timeout=300) as server:

      input_task = asyncio.create_task(handle_input(server, asyncio.current_task()))
      try:
        await asyncio.Future()
      except asyncio.CancelledError:
        print("Quit.")
      except Exception as e:
        print(f"Unexpected error: {e}")


if __name__ == "__main__":
    asyncio.run(main())
