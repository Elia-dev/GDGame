import threading
import Territory


class GameManager:
    _instance = None
    _lock = threading.Lock()

    def __init__(self):
        if GameManager._instance is not None:
            raise Exception("This class is a singleton!")

        self.enemy_attacker_territory = None
        self.my_territory_under_attack = None
        self._players_dict = {}
        self._colors_dict = {}
        self.all_territories = []  # List of all territories in the game
        self.players_name = []
        self.available_colors = []
        self._game_order = ""
        self._extracted_number = 0
        self._game_order_extracted_numbers = ""
        self._game_waiting_to_start = True
        self._game_running = True
        self._preparation_phase = True
        self._game_phase = False
        self._im_under_attack = False
        self._id_playing_player = ""
        self._enemy_attacker_army_num = 0
        self._my_army_num_to_defend = 0
        self._lobby_id = ""
        self._im_attacking = False
        self.extracted_enemy_numbers = [0, 0, 0]
        self.extracted_my_numbers = [0, 0, 0]
        self.enemy_army_num = 0
        self.my_army_num = 0
        self.force_update_after_attack = False
        self.shortest_paths = []

    @classmethod
    def get_instance(cls):
        with cls._lock:
            if cls._instance is None:
                cls._instance = cls()
            return cls._instance

    def get_my_army_num_to_defend(self):
        return self._my_army_num_to_defend

    def set_my_army_num_to_defend(self, num_army):
        self._my_army_num_to_defend = num_army

    def reset_my_army_num_to_defend(self):
        self._my_army_num_to_defend = 0

    def get_enemy_attacker_army_num(self):
        return self._enemy_attacker_army_num

    def set_enemy_attacker_army_num(self, num_army):
        self._enemy_attacker_army_num = num_army

    def reset_enemy_attacker_army_num(self):
        self._enemy_attacker_army_num = 0

    def add_player_color(self, player_id, color):
        if player_id not in self._colors_dict:
            self._colors_dict[player_id] = color
            print(f"Color added: ID = {player_id}, Color = {color}")
        else:
            print(f"Color with ID = {player_id} already exists.")

    def remove_player_color(self, player_id):
        if player_id in self._colors_dict:
            del self._colors_dict[player_id]
            print(f"Color with ID = {player_id} removed.")
        else:
            print(f"Color with ID = {player_id} does not exist.")

    def get_player_color(self, player_id):
        color = self._colors_dict.get(player_id)
        if color:
            print(f"Color found: ID = {player_id}, Color = {color}")
            return color
        print(f"Color with ID = {player_id} not found.")
        return "Player not found"

    def get_id_playing_player(self):
        return self._id_playing_player

    def set_id_playing_player(self, player):
        self._id_playing_player = player

    def reset_id_playing_player(self):
        self._id_playing_player = ""

    def get_enemy_name_by_id(self, player_id):
        name = self._players_dict.get(player_id)
        if name:
            return name
        return "This player doesn't exist"

    def add_player_to_lobby_dict(self, player_id, name):
        if player_id not in self._players_dict:
            self._players_dict[player_id] = name
            print(f"Player added: ID = {player_id}, Name = {name}")
        else:
            print(f"Player with ID = {player_id} already exists.")

    def remove_player_from_lobby_dict(self, player_id):
        if player_id in self._players_dict:
            del self._players_dict[player_id]
            print(f"Player with ID = {player_id} removed.")
        else:
            print(f"Player with ID = {player_id} does not exist.")

    def get_my_territory_under_attack(self):
        if self._my_territory_under_attack is None:
            self._my_territory_under_attack = Territory.empty_territory()
        return self._my_territory_under_attack

    def delete_my_territory_under_attack(self):
        self._my_territory_under_attack = None

    def get_enemy_attacker_territory(self):
        if self._enemy_attacker_territory is None:
            self._enemy_attacker_territory = Territory.empty_territory()
        return self._enemy_attacker_territory

    def delete_attacker_territory(self):
        self._enemy_attacker_territory = None

    def set_im_under_attack(self, value):
        self._im_under_attack = value

    def get_im_under_attack(self):
        return self._im_under_attack

    def reset_game_manager(self):
        GameManager._instance = None

    def get_preparation_phase(self):
        return self._preparation_phase

    def set_preparation_phase(self, value):
        self._preparation_phase = value

    def get_game_phase(self):
        return self._game_phase

    def set_game_phase(self, value):
        self._game_phase = value

    def get_extracted_number(self):
        return self._extracted_number

    def set_extracted_number(self, value):
        self._extracted_number = value

    def get_game_order_extracted_numbers(self):
        return self._game_order_extracted_numbers

    def set_game_order_extracted_numbers(self, value):
        self._game_order_extracted_numbers = value

    def get_game_order(self):
        return self._game_order

    def set_game_order(self, value):
        self._game_order = value

    def get_lobby_id(self):
        return self._lobby_id

    def set_lobby_id(self, lobby_id):
        self._lobby_id = lobby_id

    def reset_players_name(self):
        self.players_name.clear()

    def add_player_name(self, name):
        self.players_name.append(name)

    def get_players_number(self):
        return len(self.players_name)

    def add_available_color(self, color):
        self.available_colors.append(color)

    def get_available_colors(self):
        return self.available_colors

    def get_game_waiting_to_start(self):
        return self._game_waiting_to_start

    def set_game_waiting_to_start(self, value):
        self._game_waiting_to_start = value

    def get_game_running(self):
        return self._game_running

    def set_game_running(self, value):
        self._game_running = value

    def set_im_attacking(self, value):
        self._im_attacking = value

    def get_im_attacking(self):
        return self._im_attacking

    def reset_enemy_extracted_numbers(self):
        self.extracted_enemy_numbers = None

    def reset_my_extracted_numbers(self):
        self.extracted_my_numbers = None

    def reset_enemy_army_num(self):
        self.enemy_army_num = 0

    def reset_my_army_num(self):
        self.my_army_num = 0
