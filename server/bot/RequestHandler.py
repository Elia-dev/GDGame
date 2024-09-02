import json
from Objective import Objective
import websockets
import asyncio
import re
from Territory import Territory


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
                        self.player.tanks_available = army_number - self.player.tanks_placed

                    elif 'OBJECTIVE_CARD_ASSIGNED:' in message:
                        request = message.replace('OBJECTIVE_CARD_ASSIGNED: ', '').strip()
                        print(f'SERVER: {request}')
                        objective_dict = json.loads(request)
                        self.player.objective_card = Objective.from_dict(objective_dict)
                        print(f'CARD: {self.player.objective_card.description}')

                    elif 'TERRITORIES_CARDS_ASSIGNED:' in message:
                        request = message.replace('TERRITORIES_CARDS_ASSIGNED: ', '').strip()
                        print(f'SERVER: {request}')
                        territory_dict = json.loads(request)
                        self.player.territories = [Territory.from_dict(data) for data in territory_dict]

                    elif 'NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN:' in message:
                        request = message.replace('NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: ', '')
                        army_number = int(request)
                        self.player.tanks_available = army_number
                        self.player.tanks_num += army_number

                    elif 'PREPARATION_PHASE_TERMINATED' in message:
                        self.game_manager.set_preparation_phase(False)
                        self.game_manager.set_game_phase(True)

                    elif 'SEND_TERRITORIES_TO_ALL' in message:
                        request = message.replace('SEND_TERRITORIES_TO_ALL: ', '').strip()
                        territory_dict = json.loads(request)
                        territories = [Territory.from_dict(data) for data in territory_dict]
                        self.game_manager.all_territories = territories

                    elif 'UNDER_ATTACK' in message:
                        self.game_manager.set_im_under_attack(True)
                        request = message.replace('UNDER_ATTACK: ', '').replace(' ', '')
                        matches = re.findall(r'\[(.*?)]', request)

                        self.game_manager.extracted_enemy_numbers = list(map(int, matches[0].split(',')))

                        self.game_manager.extracted_my_numbers = list(map(int, matches[1].split(',')))

                        request = request.replace(' ', '')
                        parts = request.split(',')
                        attacker_id = parts[0]

                        ter_ids = parts[1].split('-')
                        attacker_ter_id = ter_ids[0]
                        defender_ter_id = ter_ids[1]

                        army_nums = parts[2].split('-')
                        attacker_army_num = int(army_nums[0])
                        defender_army_num = int(army_nums[1])

                        self.game_manager.enemy_army_num = attacker_army_num
                        self.game_manager.my_army_num = defender_army_num

                        attacker_territory = list(
                            filter(lambda terr: terr.id == attacker_ter_id, self.game_manager.all_territories)
                        ).pop()
                        self.game_manager.enemy_attacker_territory = attacker_territory
                        defender_territory = list(
                            filter(lambda terr: terr.id == defender_ter_id, self.player.territories)
                        ).pop()
                        self.game_manager.my_territory_under_attack = defender_territory

                        self.game_manager.set_im_under_attack(True)
                        self.game_manager.set_im_attacking(False)

                    elif 'ATTACKER_ALL_EXTRACTED_DICE' in message:
                        request = message.replace('ATTACKER_ALL_EXTRACTED_DICE: ', '').strip()
                        matches = re.findall(r'\[(.*?)]', request)

                        self.game_manager.extracted_my_numbers = list(map(int, matches[0].split(',')))
                        self.game_manager.extracted_enemy_numbers = list(map(int, matches[1].split(',')))

                        print(f'EXTRACTED_MY_NUMBERS: {self.game_manager.extracted_my_numbers}')
                        print(f'EXTRACTED_ENEMY_NUMBERS: {self.game_manager.extracted_enemy_numbers}')

                    elif 'ATTACK_FINISHED_FORCE_UPDATE' in message:

                        # Se non funziona probabilmente è perché pythin confronta l'indirizzo di memoria e non l'oggetto
                        # quando verifica che il territorio sia contenuto in all_territories.
                        for territory in self.game_manager.all_territories:

                            if territory in self.player.territories and territory.player_id != self.player.player_id:
                                self.player.territories.remove(territory)

                            elif territory not in self.player.territories and territory.player_id == self.player.player_id:
                                self.player.territories.append(territory)

                            elif territory not in self.player.territories and territory.player_id != self.player.player_id:
                                print('nothing to do')

                            else:
                                player_terr = list(filter(lambda x: x.id == territory.id, self.player.territories))
                                player_terr.num_tanks = territory.num_tanks

                        self.game_manager.set_im_under_attack(False)
                        self.game_manager.set_im_attacking(False)

                    if "SHORTEST_PATH" in message:
                        request = message.replace('SHORTEST_PATH: ', '').strip()
                        nodes = request.split('-')
                        self.game_manager.shortest_path = nodes

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
