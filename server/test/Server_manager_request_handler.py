import asyncio
import Game
import client

class RequestHandler:
    def __init__(self, game):
        self.queue = asyncio.Queue()
        self.game = game

    async def handle_requests(self):
        while True:
            client_id, message = await self.queue.get()
            print(f"Handling request from {client_id}: {message}")
            # Logica per creare e gestire le partite
            if message.startswith("create_game"):
                game_id = message.split()[1]
                self.games[game_id] = Game(game_id)
                print(f"Game {game_id} created")
            elif message.startswith("join_game"):
                game_id = message.split()[1]
                if game_id in self.games:
                    self.games[game_id].add_player(client_id)
                    print(f"Client {client_id} joined game {game_id}")
                else:
                    print(f"Game {game_id} not found")

            await asyncio.sleep(1)  # Simula il tempo di gestione della richiesta
            self.queue.task_done()

    async def add_request(self, client_id, message):
        await self.queue.put((client_id, message))
