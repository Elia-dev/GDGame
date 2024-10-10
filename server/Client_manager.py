import websockets
import json
import asyncio

from Player import Player
from Game_manager import GameManager
from RequestHandler import RequestHandler


class ClientManager:

    def __init__(self):
        self._connected = False
        self.player = None
        self._websocket = None
        self.game_manager = GameManager()

    async def start_client(self, ip):
        async with websockets.connect(f'ws://{ip}:12345', timeout=30) as websocket:
            try:
                self._connected = True
                self.player = Player(websocket)
                self._websocket = websocket
                request_handler = RequestHandler(self.player, self.game_manager)
                handler_task = asyncio.create_task(request_handler.handler(websocket))
                await asyncio.gather(handler_task)
            except Exception as e:
                await websocket.close()


    def is_connected(self):
        return self._connected

    async def send_message(self, message):
        if self._websocket is not None:
            await self._websocket.send(message)

    async def receive_message(self, websocket):
        async for message in websocket.messages:
            print(f"Received: {message}")
            # Process the message using a RequestHandler (not implemented here)
            # RequestHandler.add_request(websocket.remote_address, message)

    async def join_lobby_as_client(self, lobby_id):
        await self.send_message(f"JOIN_GAME: {lobby_id}")

    async def request_name_update_player_list(self):
        await self.send_message("REQUEST_NAME_UPDATE_PLAYER_LIST: ")

    async def send_name(self):
        await self.send_message(f"UPDATE_NAME: {self.player.player_id}-{self.player.name}")

    async def send_chosen_army_color(self):
        await self.send_message(f"CHOSEN_ARMY_COLOR: {self.player.player_id}-{self.player.army_color}")

    async def update_territories_state(self):
        territories_dict = [terr.to_dict() for terr in self.player.territories]
        self.game_manager.all_territories = []
        await self.send_message(f"UPDATE_TERRITORIES_STATE: {self.player.player_id}, {json.dumps(territories_dict)}")

    async def attack_enemy_territory(self, my_territory, enemy_territory, my_num_army):
        enemy_num_army = min(3, enemy_territory.num_tanks)
        self.game_manager.all_territories = []
        await self.send_message(f"ATTACK_TERRITORY: {self.player.player_id}-{enemy_territory.player_id}, "
                                f"{my_territory.id}-{enemy_territory.id}, {my_num_army}-{enemy_num_army}")
        GameManager.get_instance().set_im_attacking(True)

    async def request_shortest_path(self, my_territories, enemies_territories):
        self.game_manager.shortest_paths = []
        terr_to_send = my_territories + enemies_territories
        territories_dict_list = [territory.to_dict() for territory in terr_to_send]
        await self.send_message(f"REQUEST_SHORTEST_PATH: {self.player.player_id}-" + json.dumps(territories_dict_list, indent=4))
