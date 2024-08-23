import json
from Objective import Objective
import websockets
import asyncio


class RequestHandler:
    def __init__(self, player, game_manager):
        self.player = player
        self.game_manager = game_manager
        self._queue = asyncio.Queue()

    async def handler(self, websocket):
        print(f'Connected!')
        while True:
            try:
                async for message in websocket:

                    # print(f'PLAYER: Receive message from server {message}')

                    if 'LOBBY_ID' in message:
                        # print(f'PLAYER: Receive request: LOBBY_ID {message}')
                        self.game_manager.set_lobby_id(message.replace('LOBBY_ID:', '').strip())

                    elif 'GAME_STARTED_BY_HOST' in message:
                        self.game_manager.set_game_waiting_to_start(False)

                    elif 'REQUEST_NAME_UPDATE_PLAYER_LIST' in message:
                        self.game_manager.reset_players_name()
                        names = message.replace('REQUEST_NAME_UPDATE_PLAYER_LIST: ', '').strip().split(" ")
                        for name in names:
                            clean_name = name.replace('[', '').replace(']', '').replace(',', '').replace("'", "")
                            self.game_manager.add_player_name(clean_name)

                    elif 'GAME_ORDER:' in message:
                        self.game_manager.set_game_order(message.replace('GAME_ORDER: ', '').strip())

                    elif 'ID_NAMES_DICT' in message:
                        pairs = message.replace('ID_NAMES_DICT: ', '').strip().split(", ")
                        for pair in pairs:
                            parts = pair.split("-")
                            self.game_manager.add_player_to_lobby_dict(parts[0], parts[1])

                    elif 'EXTRACTED_NUMBER:' in message:
                        request = message.replace('EXTRACTED_NUMBER: ', '').strip()
                        extracted_number = int(request)
                        self.game_manager.set_extracted_number(extracted_number)

                    elif 'GAME_ORDER_EXTRACTED_NUMBERS:' in message:
                        self.game_manager.set_game_order(message.replace('GAME_ORDER_EXTRACTED_NUMBERS: ', '').strip())

                    elif 'PLAYER_ID:' in message:
                        self.player.player_id = message.replace('PLAYER_ID: ', '').strip()

                    elif 'IS_YOUR_TURN:' in message:
                        request = message.replace('IS_YOUR_TURN: ', '').strip()
                        self.player.is_my_turn = request == 'TRUE'

                    elif 'PLAYER_TURN' in message:
                        self.game_manager.set_id_playing_player(message.replace('PLAYER_TURN: ', '').strip())

                    elif 'AVAILABLE_COLORS:' in message:
                        colors = message.replace('AVAILABLE_COLORS: ', '').strip().split(' ')
                        for color in colors:
                            clean_color = color.replace('[', '').replace('].', '').replace(',', '')
                            self.game_manager.add_available_color(clean_color)

                    elif 'ID_COLORS_DICT' in message:
                        colors = message.replace('ID_COLORS_DICT: ', '').strip().split(", ")
                        for color in colors:
                            parts = color.split('-')
                            self.game_manager.add_player_color(parts[0], parts[1])

                    elif 'INITIAL_ARMY_NUMBER:' in message:
                        request = message.replace('INITIAL_ARMY_NUMBER: ', '').strip()
                        army_number = int(request)
                        self.player.tanks_num = army_number
                        self.player.tanks_placed = len(self.player.territories)
                        self.player.tanks_available = self.player.tanks_placed

                    elif 'OBJECTIVE_CARD_ASSIGNED:' in message:
                        self.player.objective_card = Objective.from_dict(message.replace('OBJECTIVE_CARD_ASSIGNED: ', ''))

                    elif 'TERRITORIES_CARDS_ASSIGNED:' in message:
                        request = message.replace('TERRITORIES_CARDS_ASSIGNED: ', '').strip()
                        self.player.territories = json.loads(request)

                    elif 'NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN:' in message:
                        request = message.replace('NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: ', '')
                        army_number = int(request)
                        self.player.tanks_available = army_number
                        self.player.tanks_num += army_number

                    elif 'PREPARATION_PHASE_TERMINATED' in message:
                        self.game_manager.set_preparation_phase(False)
                        self.game_manager.set_game_phase(True)

                    elif 'SEND_TERRITORIES_TO_ALL' in message:
                        self.game_manager.set_im_under_attack(True)
                        parts = message.replace('SEND_TERRITORIES_TO_ALL: ', '').replace(' ', '').strip()
                        attacker_id = parts[0]

                        territory_ids = parts[1].split('-')
                        attacker_ter_id = territory_ids[0]
                        defender_ter_id = territory_ids[1]

                        army_nums = parts[2].split('-')
                        attacker_army_num = army_nums[0]
                        defender_army_num = army_nums[1]

                        self.game_manager.set_enemy_attacker_army_num(attacker_army_num)
                        self.game_manager.set_my_army_num_to_defend(defender_army_num)

                        for territory in self.game_manager.all_territories:
                            if attacker_ter_id == territory.id:
                                self.game_manager.get_enemy_attacker_territory().id = territory.id
                                self.game_manager.get_enemy_attacker_territory().name = territory.name
                                self.game_manager.get_enemy_attacker_territory().continent = territory.continent
                                self.game_manager.get_enemy_attacker_territory().node = territory.node
                                self.game_manager.get_enemy_attacker_territory().num_tanks = territory.num_tanks
                                self.game_manager.get_enemy_attacker_territory().description = territory.description
                                self.game_manager.get_enemy_attacker_territory().function = territory.function
                                self.game_manager.get_enemy_attacker_territory().image = territory.image
                                self.game_manager.get_enemy_attacker_territory().player_id = attacker_id
                            if defender_ter_id == territory.id:
                                self.game_manager.get_my_territory_under_attack().id = territory.id
                                self.game_manager.get_my_territory_under_attack().name = territory.name
                                self.game_manager.get_my_territory_under_attack().continent = territory.continent
                                self.game_manager.get_my_territory_under_attack().node = territory.node
                                self.game_manager.get_my_territory_under_attack().num_tanks = territory.num_tanks
                                self.game_manager.get_my_territory_under_attack().description = territory.description
                                self.game_manager.get_my_territory_under_attack().function = territory.function
                                self.game_manager.get_my_territory_under_attack().image = territory.image
                                self.game_manager.get_my_territory_under_attack().player_id = territory.player_id

                    elif 'ATTACK_FINISHED_FORCE_UPDATE' in message:
                        for territory in self.game_manager.all_territories:
                            if territory in self.player.territories and territory.player_id != self.player.player_id:
                                self.player.territories.remove(territory)

                            if territory not in self.player.territories and territory.player_id == self.player.player_id:
                                self.player.territories.append(territory)
                        self.game_manager.set_im_under_attack(False)
                        self.game_manager.set_im_attacking(True)

                    else:
                        print(f'HANDLER: request not manageable: {message}')

            except websockets.exceptions.ConnectionClosed:
                print(f'PLAYER: Connection closed')
            except Exception as e:
                print(f"Unexpected error: {e}")

    def add_request(self, client_id, message):
        self._queue.put_nowait((client_id, message))

    def remove_request(self, source, request):
        value = source.replace(request, "")
        return value
