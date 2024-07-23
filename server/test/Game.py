import asyncio

class Game:
    def __init__(self, game_id):
        self.game_id = game_id
        self.players = {}
        self.game_running = True
        # Other game properties like game state, turns, etc.

    def add_player(self, player_id, websocket):
        self.players[player_id] = websocket
        print(f"Player {player_id} added to game {self.game_id}")

    def remove_player(self, player_id):
        if player_id in self.players:
            del self.players[player_id]
            print(f"Player {player_id} removed from game {self.game_id}")

    async def broadcast(self, message):
        for player_id, websocket in self.players.items():
            await websocket.send(message)

    async def handle_game(self):
        while self.game_running:
            await asyncio.sleep(1)
            # Example: broadcasting a message to all players every second
            await self.broadcast(f"Update from game {self.game_id}")

    def end_game(self):
        self.game_running = False
        print(f"Game {self.game_id} is ending.")
