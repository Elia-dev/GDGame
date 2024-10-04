import asyncio
import json
import random
import time
import websockets
import os
import platform
import subprocess
import Card
import Territory
import utils
from main_bot_controller import main


class Game:
    def __init__(self, game_id):
        self.game_id = game_id
        self.players = []
        self.players_alive = []
        self.dead_players = []
        self.game_running = True
        self.game_waiting_to_start = True
        self.host_player = None
        self.adj_matrix = utils.get_adj_matrix(os.getcwd() + '/assets/adj_matrix.npy')
        self.queue = asyncio.Queue()
        self.army_colors = {
            'red': None,
            'blue': None,
            'green': None,
            'yellow': None,
            'purple': None,
            'brown': None
        }
        self.event = asyncio.Event()
        self.event_strategic_movement = asyncio.Event()
        self.firstRound = True
        self.bots_pid = []

    def add_player(self, player):
        if player.name is None:
            player.name = "Computer"
        self.players.append(player)
        print(f"Player {player.name} added to game {self.game_id}")

    def remove_player(self, player):
        player.lobby_id = None
        print(f"Player {player.name} removed from game {self.game_id}")
        self.players.remove(player)

    def remove_all_players(self):
        for player in self.players:
            player.lobby_id = None
            self.remove_player(player)

    async def broadcast(self, message):
        for player in self.players:
            await player.sock.send(message)

    async def create_game(self, player):
        self.host_player = player
        self.players.append(player)
        tasks = [
            asyncio.create_task(self.listen_to_player_request(self.host_player)),
            asyncio.create_task(self.handle_requests()),
            asyncio.create_task(self.handle_game())
        ]
        await asyncio.gather(*tasks)

    async def handle_game(self):
        print("Waiting for players to start the game...\n")
        while self.game_waiting_to_start is True:
            if self.game_id is None:
                return
            await asyncio.sleep(1)

        # Preparation phase
        print("Game started\n")
        await self.__game_order__()
        name_id_dict = "ID_NAMES_DICT: " + ", ".join(f"{player.player_id}-{player.name}" for player in self.players)
        await self.broadcast(name_id_dict)
        available_colors = [color for color, user_id in self.army_colors.items() if user_id is None]
        await self.players[0].sock.send("AVAILABLE_COLORS: " + ", ".join(available_colors))
        await self._give_territory_cards()

        territories_list = []
        for player in self.players:
            for territory in player.territories:
                territories_list.append(territory.to_dict())
        await self.broadcast("SEND_TERRITORIES_TO_ALL: " + json.dumps(territories_list, indent=4))

        initial_army_number = self.__army_start_num__(len(self.players))
        for player in self.players:
            player.tanks_num = initial_army_number
            player.tanks_placed = len(player.territories)
            player.tanks_available = player.tanks_num - player.tanks_placed
        await self.broadcast("INITIAL_ARMY_NUMBER: " + str(initial_army_number))
        await self.broadcast("IS_YOUR_TURN: FALSE")
        await self.army_color_chose()
        dict_id_color = "ID_COLORS_DICT: "
        dict_id_color += ", ".join([f"{player.player_id}-{player.army_color}" for player in self.players])
        await self.broadcast(dict_id_color)
        await self._give_objective_cards()
        await self._assignDefaultArmiesOnTerritories()
        await self.broadcast("PREPARATION_PHASE_TERMINATED")
        # Preparation phase terminated
        self.players_alive = self.players.copy()
        # Game loop
        while self.game_running:
            for player in self.players_alive:
                await self.broadcast("PLAYER_TURN: " + player.player_id)
                if not self.firstRound:
                    # REINFORCE PHASE
                    num_army_to_send = self.calculateArmyForThisTurn(player)
                    player.tanks_num += num_army_to_send
                    player.tanks_available += num_army_to_send
                    await player.sock.send("NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: " + str(num_army_to_send))
                    await player.sock.send("IS_YOUR_TURN: TRUE")
                    await self.event.wait()  # Wait for update territories state client-side
                    self.event = asyncio.Event()
                    player.tanks_available = 0
                    player.tanks_placed += num_army_to_send
                    # REINFORCE PHASE TERMINATED
                else:
                    await player.sock.send("IS_YOUR_TURN: TRUE")

                # FIGHT PHASE OR STRATEGIC MOVEMENT PHASE
                self.event_strategic_movement = asyncio.Event()
                # Wait for strategic movement or attack
                while not self.event_strategic_movement.is_set():
                    self.event = asyncio.Event()
                    await self.event.wait()

                #print("Fight phase terminated")
                # FIGHT PHASE AND STRATEGIC MOVEMENT TERMINATED

                self.event = asyncio.Event()
                self.event_strategic_movement = asyncio.Event()
                #print("Strategic movement terminated")
                await player.sock.send("IS_YOUR_TURN: FALSE")

                # CHECK for victory
                if self.check_for_victory(player) is True:
                    await self.broadcast("WINNER: " + player.player_id)
                    print(f"Player {player.name} has won the game")
                    self.game_running = False
            self.firstRound = False
        await asyncio.sleep(1)
        self.kill_all_bots()

    async def handle_requests(self):
        while self.game_running:
            try:
                player, message = await self.queue.get()
                if "ADD_BOT" in message:
                    host_id = self.game_id.replace(' ', '_')
                    bot_name = f'Computer{len(self.bots_pid)}'
                    if platform.system() == 'Windows':
                        script_path = os.path.join(os.getcwd(), 'run_bot.bat')
                    else:
                        script_path = os.path.join(os.getcwd(), 'run_bot.sh')

                    bot_pid = subprocess.check_output(f"{script_path} %s %s" % (host_id, bot_name), shell=True, text=True).strip()
                    self.bots_pid.append(bot_pid)
                if "REMOVE_BOT" in message:
                    bot_pid = self.bots_pid.pop()
                    kill_command = ['kill', '-9', str(bot_pid)]
                    subprocess.run(kill_command)
                if "REQUEST_BOT_NUMBER" in message:
                    await player.sock.send("BOT_NUMBER: " + str(len(self.bots_pid)))
                if "LOBBY_KILLED_BY_HOST" in message:
                    self.game_id = None
                    self.game_running = False
                    self.remove_all_players()
                    return
                if "GAME_KILLED_BY_HOST" in message:
                    id = self._remove_request(message, "GAME_KILLED_BY_HOST: ")
                    for player in self.players:
                        if player.player_id != id:
                            await player.sock.send("GAME_KILLED_BY_HOST")
                    self.game_id = None
                    self.remove_all_players()
                    return
                if "PLAYER_HAS_LEFT_THE_GAME" in message:
                    # TODO: IMPLEMENTARE RIMPIAZZO CON BOT
                    # Implementazione temporanea sotto, chiude la partita se un giocatore esce
                    id = self._remove_request(message, "PLAYER_HAS_LEFT_THE_GAME: ")
                    await self.broadcast("GAME_KILLED_BY_HOST")
                    self.remove_all_players()
                    self.game_id = None
                    self.game_running = False

                if "PLAYER_HAS_LEFT_THE_LOBBY" in message:
                    id = self._remove_request(message, "PLAYER_HAS_LEFT_THE_LOBBY: ")
                    for player in self.players:
                        if player.player_id == id:
                            self.remove_player(player)

                if "REQUEST_NAME_UPDATE_PLAYER_LIST" in message:
                    player_names = []
                    for p in self.players:
                        player_names.append(p.name)
                    await player.sock.send("REQUEST_NAME_UPDATE_PLAYER_LIST: " + str(player_names))

                if "GAME_STARTED_BY_HOST" in message:
                    self.game_waiting_to_start = False
                    await self.broadcast("GAME_STARTED_BY_HOST")

                if "UPDATE_NAME:" in message:
                    message = self._remove_request(message, "UPDATE_NAME: ")
                    id, name = message.split("-")
                    for player in self.players:
                        if player.player_id == id:
                            player.name = name

                if "CHOSEN_ARMY_COLOR:" in message:
                    message = self._remove_request(message, "CHOSEN_ARMY_COLOR: ")
                    id, color = message.split("-")
                    for player in self.players:
                        if player.player_id == id:
                            player.army_color = color
                    self.army_colors[color] = id
                    self.event.set()

                if "UPDATE_TERRITORIES_STATE:" in message:
                    message = self._remove_request(message, "UPDATE_TERRITORIES_STATE: ")
                    id = message.split(", ")[0]
                    message = self._remove_request(message, (id + ", "))
                    territories_list_dict = json.loads(message)
                    territories = [Territory.Territory.from_dict(data) for data in territories_list_dict]
                    for player in self.players:
                        if player.player_id == id:
                            player.territories = territories
                    territories_list = []
                    for player in self.players:
                        for territory in player.territories:
                            territories_list.append(territory.to_dict())

                    await self.broadcast("SEND_TERRITORIES_TO_ALL: " + json.dumps(territories_list, indent=4))
                    self.event_strategic_movement.set()
                    self.event.set()
                    self.adj_matrix = utils.update_adjacent_matrix(self.players, self.adj_matrix)

                if "ATTACK_TERRITORY:" in message:
                    random.seed(time.time())
                    attacker_player = None
                    defender_player = None
                    attacker_territory = None
                    defender_territory = None
                    message = self._remove_request(message, "ATTACK_TERRITORY: ")
                    clean_message = [segmento.strip() for segmento in message.split(",")]
                    attacker_id, defender_id = clean_message[0].split("-")
                    for player in self.players_alive:
                        if player.player_id == attacker_id:
                            attacker_player = player
                        if player.player_id == defender_id:
                            defender_player = player
                    attacker_ter_id, defender_ter_id = clean_message[1].split("-")
                    for terr in attacker_player.territories:
                        if terr.id == attacker_ter_id:
                            attacker_territory = terr
                    for terr in defender_player.territories:
                        if terr.id == defender_ter_id:
                            defender_territory = terr

                    attacker_army_num, defender_army_num = clean_message[2].split("-")
                    attacker_army_num = int(attacker_army_num)
                    defender_army_num = int(defender_army_num)

                    # Generate n random numbers, whit n number of army
                    extracted_numbers_attacker = [random.randint(1, 6) for _ in range(attacker_army_num)]
                    extracted_numbers_defender = [random.randint(1, 6) for _ in range(defender_army_num)]
                    extracted_numbers_attacker.sort(reverse=True)
                    extracted_numbers_defender.sort(reverse=True)

                    await attacker_player.sock.send(
                        "ATTACKER_ALL_EXTRACTED_DICE: " + extracted_numbers_attacker.__str__() + ", " + extracted_numbers_defender.__str__())

                    await defender_player.sock.send(
                        "UNDER_ATTACK: " + attacker_id + ", " + attacker_ter_id + "-" + defender_ter_id + ", "
                        + str(attacker_army_num) + "-" + str(
                            defender_army_num) + ", " + extracted_numbers_attacker.__str__() + ", " + extracted_numbers_defender.__str__())

                    attacker_wins = 0
                    defender_wins = 0

                    # Evaluate the result of the fight comparing the extracted numbers in order
                    # with the biggest number of the attacker with the biggest number of the defender
                    for attacker_num, defender_num in zip(extracted_numbers_attacker, extracted_numbers_defender):
                        if attacker_num > defender_num:
                            attacker_wins += 1
                        else:
                            defender_wins += 1

                    # Update the number of tanks in the territories
                    attacker_territory.num_tanks -= defender_wins
                    defender_territory.num_tanks -= attacker_wins

                    if defender_territory.num_tanks == 0:  # Evaluate if the defender has lost the territory
                        defender_territory.player_id = attacker_id
                        defender_player.removeTerritory(defender_territory)
                        defender_territory.num_tanks = attacker_army_num - defender_wins
                        attacker_territory.num_tanks -= attacker_army_num - defender_wins
                        attacker_player.addTerritory(defender_territory)

                    # updateAllTerritories in broadcast and check client-side if the territory has changed owner
                    territories_list = []
                    for player in self.players_alive:
                        for territory in player.territories:
                            territories_list.append(territory.to_dict())

                    await self.broadcast("SEND_TERRITORIES_TO_ALL: " + json.dumps(territories_list, indent=4))

                    if len(defender_player.territories) == 0:
                        await defender_player.sock.send("PLAYER_KILLED_BY: " + attacker_player.player_id)
                        defender_player.killed_by = attacker_player
                        self.dead_players.append(defender_player)
                        self.players_alive.remove(defender_player)
                    await self.broadcast("ATTACK_FINISHED_FORCE_UPDATE")
                    self.event.set()

                if 'REQUEST_SHORTEST_PATH' in message:
                    message = self._remove_request(message, "REQUEST_SHORTEST_PATH: ")
                    player_id, territories_json = message.split("-")
                    territories_list_dict = json.loads(territories_json)
                    territories = [Territory.Territory.from_dict(data) for data in territories_list_dict]
                    friends_territories = list(
                        filter(lambda terr: terr.player_id == player_id, territories)
                    )
                    enemies_territories = list(
                        filter(lambda terr: terr.player_id != player_id, territories)
                    )
                    paths = []
                    for friend in friends_territories:
                        for enemy in enemies_territories:
                            path = utils.get_shortest_path(friend.node, enemy.node, self.adj_matrix)
                            paths.append(path)
                    requesting_player = None
                    for player in self.players:
                        if player.player_id == player_id:
                            requesting_player = player
                    if requesting_player:
                        await requesting_player.sock.send(f"SHORTEST_PATH: " + json.dumps(paths))

                self.queue.task_done()
            except Exception as e:
                print(f"Error in handle_game: {e}")

    async def listen_to_request(self):
        while self.game_running:
            for player in self.players:
                try:
                    async for message in player.sock:
                        await self.queue.put((player, message))
                        if "GAME_KILLED_BY_HOST" in message:
                            return
                except websockets.exceptions.ConnectionClosed:
                    print(f"Client {player.name} disconnected")
                    self.remove_player(player)

    async def listen_to_player_request(self, player):
        while self.game_running:
            try:
                async for message in player.sock:
                    await self.queue.put((player, message))
                    if "GAME_KILLED_BY_HOST" in message:
                        return
                    if "PLAYER_HAS_LEFT_THE_LOBBY" in message:
                        return
            except websockets.exceptions.ConnectionClosed:
                print(f"Client {player.name} disconnected")
                self.remove_player(player)

    async def end_game(self):
        self.game_running = False
        await self.broadcast("GAME_KILLED_BY_HOST")
        self.remove_all_players()
        print(f"Game {self.game_id} is terminated.")
        self.kill_all_bots()
        print(f"Game {self.game_id} terminated by host")
        return

    async def kill_game(self):
        self.game_running = False
        await self.broadcast("GAME_KILLED_BY_HOST")
        print(f"Game {self.game_id} killed by server.")
        return

    async def __game_order__(self):
        response = {}
        random.seed(time.time())
        for player in self.players:
            gaming_dice = random.randint(1, 6)
            response[player] = gaming_dice
        sorted_players = [item[0] for item in sorted(response.items(), key=lambda item: item[1])]
        sorted_players.reverse()

        for i in range(len(sorted_players)):
            self.players[i] = sorted_players[i]

        game_order = []
        game_order_extracted_numbers = []
        for i in range(len(self.players)):
            if self.players[i].name == "Computer":
                self.players[i].name = "Computer" + str(i)  # Solo per debug
            game_order.append(self.players[i].name + "-" + str(i) + ", ")
            game_order_extracted_numbers.append(self.players[i].name + "-" + str(response[self.players[i]]) + ", ")

        fgame_order = {''.join(game_order)}
        for player in self.players:
            await player.sock.send("EXTRACTED_NUMBER: " + str(response[player]))
        await self.broadcast("GAME_ORDER: " + str(fgame_order))
        await self.broadcast("GAME_ORDER_EXTRACTED_NUMBERS: " + str(game_order_extracted_numbers))

    def _remove_request(self, source, request):
        value = source.replace(request, "")
        return value

    async def army_color_chose(self):
        for player in self.players:
            available_colors = [color for color, user_id in self.army_colors.items() if user_id is None]
            #print("Available color in this turn: " + available_colors.__str__())
            await player.sock.send("AVAILABLE_COLORS: " + ", ".join(available_colors))
            await player.sock.send("IS_YOUR_TURN: TRUE")
            await self.event.wait()  # Waiting for player choice
            await player.sock.send("IS_YOUR_TURN: FALSE")
            self.event = asyncio.Event()

    def __army_start_num__(self, num_player):
        switcher = {
            2: 24,  #Togliere
            3: 35,
            4: 30,
            5: 25,
            6: 20
        }
        return switcher.get(num_player)

    async def _give_objective_cards(self):
        cards = utils.read_objects_cards()
        card_drawn = None
        color_list = [player.army_color for player in self.players]

        for player in self.players:
            control = True
            while control:
                card_drawn = cards[random.randint(0, len(cards) - 1)]
                if (player.army_color == "red" and card_drawn.id == "obj9") or (
                        player.army_color == "blue" and card_drawn.id == "obj10") or (
                        player.army_color == "green" and card_drawn.id == "obj11"):
                    print("")
                elif ("red" not in color_list and card_drawn.id == "obj9") or (
                        "blue" not in color_list and card_drawn.id == "obj10") or (
                        "green" not in color_list and card_drawn.id == "obj11"):
                    print("")
                else:
                    control = False
            card_drawn.player_id = player.player_id
            player.objective_card = card_drawn
            cards.remove(card_drawn)
            await player.sock.send("OBJECTIVE_CARD_ASSIGNED: " + json.dumps(Card.Card.to_dict(player.objective_card)))

    async def _give_territory_cards(self):
        cards = utils.read_territories_cards()
        while cards:
            for player in self.players:
                if cards:
                    card_drawn = cards[random.randint(0, len(cards) - 1)]
                    card_drawn.player_id = player.player_id
                    player.addTerritory(card_drawn)
                    cards.remove(card_drawn)

        for player in self.players:
            territories_list = [terr.to_dict() for terr in player.territories]
            await player.sock.send("TERRITORIES_CARDS_ASSIGNED: " + json.dumps(territories_list,
                                                                               indent=4))  # Indent only for better readable

    async def _assignDefaultArmiesOnTerritories(self):
        control = 0
        while control < len(self.players):
            for player in self.players:
                if player.tanks_available > 0:
                    await self.broadcast("PLAYER_TURN: " + player.player_id)
                    await player.sock.send("IS_YOUR_TURN: TRUE")
                    await self.event.wait()  # Waiting for player choice
                    await player.sock.send("IS_YOUR_TURN: FALSE")
                    if player.tanks_available >= 3:
                        player.tanks_available -= 3
                        player.tanks_placed += 3
                    else:
                        player.tanks_placed += player.tanks_available
                        player.tanks_available = 0
                    self.event = asyncio.Event()  # Event reset
                else:
                    control += 1

    def calculateArmyForThisTurn(self, player):
        # Continent name: NA SA EU AF AS OC
        armyForContinent = 0
        NA_count = 0
        SA_count = 0
        EU_count = 0
        AF_count = 0
        AS_count = 0
        OC_count = 0
        armyForTerritories = len(player.territories) // 3
        for territory in player.territories:
            if territory.continent == "NA":
                NA_count += 1
            elif territory.continent == "SA":
                SA_count += 1
            elif territory.continent == "EU":
                EU_count += 1
            elif territory.continent == "AF":
                AF_count += 1
            elif territory.continent == "AS":
                AS_count += 1
            elif territory.continent == "OC":
                OC_count += 1

        if NA_count == 9:
            armyForContinent += 5
        if SA_count == 4:
            armyForContinent += 2
        if EU_count == 7:
            armyForContinent += 5
        if AF_count == 6:
            armyForContinent += 3
        if AS_count == 12:
            armyForContinent += 7
        if OC_count == 4:
            armyForContinent += 2

        totalArmyToAssing = armyForTerritories + armyForContinent
        return totalArmyToAssing

    def check_for_victory(self, player):
        NA_count = 0
        SA_count = 0
        EU_count = 0
        AF_count = 0
        AS_count = 0
        OC_count = 0
        if player.objective_card.id == "obj1":
            army_count = 0
            if len(player.territories) >= 18:
                for terr in player.territories:
                    if terr.num_tanks >= 2:
                        army_count += 1
                if army_count >= 18:
                    return True
            return False

        if player.objective_card.id == "obj2":
            if len(player.territories) >= 24:
                return True
            return False

        if player.objective_card.id == "obj3":
            for territory in player.territories:
                if territory.continent == "NA":
                    NA_count += 1
                elif territory.continent == "AF":
                    AF_count += 1
            if NA_count == 9 and AF_count == 6:
                return True
            return False

        if player.objective_card.id == "obj4":
            for territory in player.territories:
                if territory.continent == "NA":
                    NA_count += 1
                elif territory.continent == "OC":
                    OC_count += 1
            if NA_count == 9 and OC_count == 4:
                return True
            return False

        if player.objective_card.id == "obj5":
            for territory in player.territories:
                if territory.continent == "AS":
                    AS_count += 1
                elif territory.continent == "OC":
                    OC_count += 1
            if AS_count == 12 and OC_count == 4:
                return True
            return False

        if player.objective_card.id == "obj6":
            for territory in player.territories:
                if territory.continent == "AS":
                    AS_count += 1
                elif territory.continent == "AF":
                    AF_count += 1
            if AS_count == 12 and AF_count == 6:
                return True
            return False

        if player.objective_card.id == "obj7":
            for territory in player.territories:
                if territory.continent == "EU":
                    EU_count += 1
                elif territory.continent == "SA":
                    SA_count += 1
            if EU_count == 7 and SA_count == 4:
                for territory in player.territories:
                    if territory.continent == "NA":
                        NA_count += 1
                    elif territory.continent == "AF":
                        AF_count += 1
                    elif territory.continent == "AS":
                        AS_count += 1
                    elif territory.continent == "OC":
                        OC_count += 1
                if NA_count == 9 or AF_count == 6 or AS_count == 12 or OC_count == 4:
                    return True
            return False

        if player.objective_card.id == "obj8":
            for territory in player.territories:
                if territory.continent == "EU":
                    EU_count += 1
                elif territory.continent == "OC":
                    OC_count += 1
            if EU_count == 7 and OC_count == 4:
                for territory in player.territories:
                    if territory.continent == "NA":
                        NA_count += 1
                    elif territory.continent == "AF":
                        AF_count += 1
                    elif territory.continent == "AS":
                        AS_count += 1
                    elif territory.continent == "SA":
                        SA_count += 1
                if NA_count == 9 or AF_count == 6 or AS_count == 12 or SA_count == 4:
                    return True
            return False

        if player.objective_card.id == "obj9":
            control = 0
            killer = None
            enemy_player = next((player for player in self.players_alive if player.army_color == "red"), None)
            if enemy_player is None:
                for dead_player in self.dead_players:
                    if dead_player.army_color == "red":
                        if dead_player.killed_by.player_id == player.player_id:
                            return True
                        else:
                            killer = dead_player.killed_by
            else:
                return False
            while control < (len(self.players_alive) + len(self.dead_players)):
                if killer in self.players_alive:
                    return False
                else:
                    if killer.killed_by.player_id == player.player_id:
                        return True
                    else:
                        killer = killer.killed_by
                control += 1
            return False

        if player.objective_card.id == "obj10":
            control = 0
            killer = None
            enemy_player = next((player for player in self.players_alive if player.army_color == "blue"), None)
            if enemy_player is None:
                for dead_player in self.dead_players:
                    if dead_player.army_color == "blue":
                        if dead_player.killed_by.player_id == player.player_id:
                            return True
                        else:
                            killer = dead_player.killed_by
            else:
                return False
            while control < (len(self.players_alive) + len(self.dead_players)):
                if killer in self.players_alive:
                    return False
                else:
                    if killer.killed_by.player_id == player.player_id:
                        return True
                    else:
                        killer = killer.killed_by
                control += 1
            return False

        if player.objective_card.id == "obj11":
            control = 0
            killer = None
            enemy_player = next((player for player in self.players_alive if player.army_color == "green"), None)
            if enemy_player is None:
                for dead_player in self.dead_players:
                    if dead_player.army_color == "green":
                        if dead_player.killed_by.player_id == player.player_id:
                            return True
                        else:
                            killer = dead_player.killed_by
            else:
                return False
            while control < (len(self.players_alive) + len(self.dead_players)):
                if killer in self.players_alive:
                    return False
                else:
                    if killer.killed_by.player_id == player.player_id:
                        return True
                    else:
                        killer = killer.killed_by
                control += 1
            return False

    def kill_all_bots(self):
        for bot_pid in self.bots_pid:
            kill_command = ['kill', '-9', str(bot_pid)]
            subprocess.run(kill_command)



async def _run_script(host_id, bot_name):
    await asyncio.gather(main(host_id, bot_name))
    #subprocess.run(["python3", script_name, host_id, bot_name])