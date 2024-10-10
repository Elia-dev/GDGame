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
        self.extracted_enemy_numbers = None
        self.extracted_my_numbers = None
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

    def add_player_color(self, player_id, color):
        if player_id not in self._colors_dict:
            self._colors_dict[player_id] = color
            print(f"Color added: ID = {player_id}, Color = {color}")
        else:
            print(f"Color with ID = {player_id} already exists.")

    def get_player_color(self, player_id):
        color = self._colors_dict.get(player_id)
        if color:
            print(f"Color found: ID = {player_id}, Color = {color}")
            return color
        print(f"Color with ID = {player_id} not found.")
        return "Player not found"

    def set_id_playing_player(self, player):
        self._id_playing_player = player

    def add_player_to_lobby_dict(self, player_id, name):
        if player_id not in self._players_dict:
            self._players_dict[player_id] = name
            print(f"Player added: ID = {player_id}, Name = {name}")
        else:
            print(f"Player with ID = {player_id} already exists.")

    def set_im_under_attack(self, value):
        self._im_under_attack = value

    def set_preparation_phase(self, value):
        self._preparation_phase = value

    def set_game_phase(self, value):
        self._game_phase = value

    def get_extracted_number(self):
        return self._extracted_number

    def set_extracted_number(self, value):
        self._extracted_number = value

    def get_game_order(self):
        return self._game_order

    def set_game_order(self, value):
        self._game_order = value

    def set_lobby_id(self, lobby_id):
        self._lobby_id = lobby_id

    def reset_players_name(self):
        self.players_name.clear()

    def add_player_name(self, name):
        self.players_name.append(name)

    def add_available_color(self, color):
        self.available_colors.append(color)

    def get_available_colors(self):
        return self.available_colors

    def set_game_waiting_to_start(self, value):
        self._game_waiting_to_start = value

    def set_im_attacking(self, value):
        self._im_attacking = value

    def reset_enemy_extracted_numbers(self):
        self.extracted_enemy_numbers = None

    def reset_my_extracted_numbers(self):
        self.extracted_my_numbers = None
