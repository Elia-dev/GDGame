import asyncio
import websockets


class Game:
    def __init__(self, game_id):
        self.game_id = game_id
        self.players = []
        self.game_running = True
        self.host_player = None
        self.queue = asyncio.Queue()

    def add_player(self, player):
        self.players.append(player)
        print(f"Player {player.id} added to game {self.game_id}")

    def remove_player(self, player):
        self.players.remove(player)
        print(f"Player {player} removed from game {self.game_id}")

    async def broadcast(self, message):
        for player in self.players:
            player.sock.send(message)

    async def create_game(self, player):
        self.host_player = player
        self.players.append(player)

        asyncio.create_task(self.handle_game())
        #asyncio.create_task(self.listen_to_request())
        #Tenere la riga sopra commentata solo se nel server manager sono attive le righe
        #await game.listen_to_request()
        #break
        #Nella scelta dell'host


    async def handle_game(self):
        while self.game_running:
            try:
                player, message = await self.queue.get()
                print(f"GAME: handling request from {player}: {message}")
                if "prova" in message:
                    print("Bella prova compà")
                if "cane" in message:
                    print("I love dogs, doesn't everyone?")
                # Qui puoi aggiungere la logica per gestire il messaggio
                # Ad esempio, rispondere al client, fare una richiesta ad un altro servizio, ecc.
                await asyncio.sleep(5)  # Simula il tempo di gestione della richiesta
                self.queue.task_done()
            except Exception as e:
                print(f"Error in handle_game: {e}")

    async def listen_to_request(self):
        while self.game_running:
            for player in self.players:
                try:
                    async for message in player.sock:
                        await self.queue.put((player, message))
                except websockets.exceptions.ConnectionClosed:
                    print(f"Client {player} disconnected")
                    self.remove_player(player)

                '''try:
                    async for message in player.sock:
                        await self.queue.put((player, message))
                except websockets.exceptions.ConnectionClosed:
                    print(f"Client {player} disconnected")
                    '''

    def end_game(self):
        self.game_running = False
        print(f"Game {self.game_id} is ending.")
