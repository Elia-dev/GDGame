import asyncio
import json
import random
import time
import websockets

import Card
import Territory
import utils


class Game:
    def __init__(self, game_id):
        self.game_id = game_id
        self.players = []
        self.game_running = True
        self.game_waiting_to_start = True
        self.host_player = None
        self.queue = asyncio.Queue()
        self.army_colors = {
            'red': None,
            'blue': None,
            'green': None,
            'yellow': None,
            'purple': None,
            'black': None
        }
        self.event = asyncio.Event()

    def add_player(self, player):
        if player.name == None:
            player.name = "Computer"
        self.players.append(player)
        print(f"Player {player.player_id} with name {player.name} added to game {self.game_id}")

    def remove_player(self, player):
        self.players.remove(player)
        print(f"Player {player.player_id} with name {player.name} removed from game {self.game_id}")

    def remove_all_players(self):
        for player in self.players:
            self.remove_player(player)

    async def broadcast(self, message):
        print(f"sending broadcast message: {message}")
        for player in self.players:
            await player.sock.send(message)

    async def create_game(self, player):
        self.host_player = player
        self.players.append(player)
        print("Aggiunto nuovo HOST")
        tasks = [
            asyncio.create_task(self.listen_to_player_request(self.host_player)),
            asyncio.create_task(self.handle_requests()),
            asyncio.create_task(self.handle_game())
        ]
        await asyncio.gather(*tasks)

    async def handle_game(self):
        print("Aperto HANDLE_GAME\n")
        while self.game_waiting_to_start is True:
            print("ASPETTANDO CHE SI COLLEGHINO TUTTI\n")
            if self.game_id is None:
                return
            await asyncio.sleep(2)

        #Preparation phase
        await self.__game_order__()
        available_colors = [color for color, user_id in self.army_colors.items() if user_id is None]
        await self.players[0].sock.send("AVAILABLE_COLORS: " + ", ".join(available_colors))
        await self._give_territory_cards()
        initial_army_number = self.__army_start_num__(len(self.players))
        print("Initial army number: " + str(initial_army_number))
        for player in self.players:
            player.tanks_num = initial_army_number
            player.tanks_placed = len(player.territories)
            player.tanks_available = player.tanks_num - player.tanks_placed
        await self.broadcast("INITIAL_ARMY_NUMBER: " + str(initial_army_number))
        await self.broadcast("IS_YOUR_TURN: FALSE")
        await self.army_color_chose()
        await self._give_objective_cards()
        await self._assignDefaultArmiesOnTerritories()
        await self.broadcast("PREPARATION_PHASE_TERMINATED")
        print("PREPARATION_PHASE_TERMINATED")
        #Preparation phase terminated

        #Game loop TOBE TESTED
        print("---INIZIO FASE DI GIOCO---")
        while self.game_running:
            for player in self.players:
                print(f"Turno del player id: {player.player_id} con nome {player.name}")
                print("REINFORCE PHASE")
                # REINFORCE PHASE
                # CheckContinents
                # CheckArmy
                num_army_to_send = self.calculateArmyForThisTurn(player)
                print(f"Numero di armate ricevute nella fase di rinforzo: {num_army_to_send}")
                player.tanks_num += num_army_to_send
                player.tanks_available += num_army_to_send
                # SendArmy
                await player.sock.send("NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: " + str(num_army_to_send))
                # UnlockTurn
                await player.sock.send("IS_YOUR_TURN: TRUE")
                # Waiting for player to finish the turn and send updated territories
                print("Waiting for player to end the reinforce phase")
                await self.event.wait()
                self.event = asyncio.Event()  # Event reset
                player.tanks_available = 0
                player.tanks_placed += num_army_to_send
                print("Reinforced phase terminated")
                # REINFORCE PHASE TERMINATED

                # FIGHT PHASE

                print("[fake] Fight phase terminated")
                # FIGHT PHASE TERMINATED

                # STRATEGIC MOVEMENT
                #await self.event.wait()
                print("[fake] Strategic movement terminated")
                # STRATEGIC MOVEMENT TERMINATED

                # CHECK (card objective, number of tanks ecc...)
                print("[fake] Check objective card terminated")


    async def handle_requests(self):
        while self.game_running:
            try:
                player, message = await self.queue.get()
                print(f"GAME: handling request from client id - : {player.player_id} with name {player.name}: {message}")

                if "LOBBY_KILLED_BY_HOST" in message:
                    id = self._remove_request(message, "LOBBY_KILLED_BY_HOST: ")
                    for player in self.players:
                        if player.player_id != id:
                            await player.sock.send("LOBBY_KILLED_BY_HOST")
                    self.game_id = None
                    self.remove_all_players()
                    return

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
                    await self.broadcast("GAME_STARTED_BY_HOST")
                    self.game_waiting_to_start = False

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
                    self.event.set() #Setting event to True

                if "UPDATE_TERRITORIES_STATE:" in message:
                    message = self._remove_request(message, "UPDATE_TERRITORIES_STATE: ")
                    id = message.split(", ")[0]
                    print(f"ID del player che ha mandato l'update: {id}")
                    message = self._remove_request(message, (id + ", "))
                    print(f"Messaggio pulito da richiesta e ID: {message}")
                    territories_list_dict = json.loads(message)
                    print("Eseguito json.load sul messaggio")
                    territories = [Territory.Territory.from_dict(data) for data in territories_list_dict]
                    print("Eseguito from dict sulla lista dei dizionari sul messaggio")
                    for player in self.players:
                        if player.player_id == id:
                            player.territories = territories
                            print(f"Aggiornato lista stati del player con ID {player.player_id} con nome {player.name}")
                    print("Fine aggiornamento territori su server")

                    territories_list = []
                    for player in self.players:
                        for territory in player.territories:
                            territories_list.append(territory.to_dict())

                    await self.broadcast("SEND_TERRITORIES_TO_ALL: " + json.dumps(territories_list, indent=4))
                    print("Fine aggiornamento territori")
                    self.event.set()

                if "REQUEST_TERRITORY_INFO:" in message: #TOBE TESTED
                    message = self._remove_request(message, "REQUEST_TERRITORY_INFO: ")
                    playerId, territoryId = message.split("-")
                    tempPlayer = None
                    for player in self.players:
                        if player.player_id == playerId:
                            tempPlayer = player

                    for player in self.players:
                        for territory in player.territories:
                            if territory.id == territoryId:
                                await tempPlayer.sock.send("RECEIVED_REQUEST_TERRITORY_INFO: " + json.dumps(territory.to_dict()))

                self.queue.task_done()
            except Exception as e:
                print(f"Error in handle_game: {e}")


    async def listen_to_request(self):
        while self.game_running:
            for player in self.players:
                try:
                    async for message in player.sock:
                        await self.queue.put((player, message))
                        if "LOBBY_KILLED_BY_HOST" in message:
                            return
                except websockets.exceptions.ConnectionClosed:
                    print(f"Client {player.player_id} disconnected")
                    self.remove_player(player)

    async def listen_to_player_request(self, player):
        while True:  # Da cambiare mettendo finché il giocatore può giocare/è ancora in gioco
            try:
                async for message in player.sock:
                    await self.queue.put((player, message))
                    if "LOBBY_KILLED_BY_HOST" in message:
                        return
                    if "PLAYER_HAS_LEFT_THE_LOBBY" in message:
                        return
            except websockets.exceptions.ConnectionClosed:
                print(f"Client {player} disconnected")
                self.remove_player(player)

    def end_game(self):
        self.game_running = False
        print(f"Game {self.game_id} is ending.")

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

        print(f"Game order: {''.join(game_order)}")
        fgame_order = {''.join(game_order)}
        print(f"GAME_ORDER_EXTRACTED_NUMBERS: {str(game_order_extracted_numbers)}")
        for player in self.players:
            print(f"For player {player.name} extracted number {str(response[player])} ")
            await player.sock.send("EXTRACTED_NUMBER: " + str(response[player]))
        await self.broadcast("GAME_ORDER: " + str(fgame_order))
        await self.broadcast("GAME_ORDER_EXTRACTED_NUMBERS: " + str(game_order_extracted_numbers))
        print("done")

    def _remove_request(self, source, request):
        value = source.replace(request, "")
        return value

    async def army_color_chose(self):
        for player in self.players:
            available_colors = [color for color, user_id in self.army_colors.items() if user_id is None]
            print("Available color in this turn: " + available_colors.__str__())
            await player.sock.send("AVAILABLE_COLORS: " + ", ".join(available_colors))
            await player.sock.send("IS_YOUR_TURN: TRUE")
            print("TURNO DI " + player.name)
            await self.event.wait() # Waiting for player choice
            await player.sock.send("IS_YOUR_TURN: FALSE")
            self.event = asyncio.Event() # Event reset
            print("Turn over for player " + player.name + "chosen color: " + player.army_color)

    def __army_start_num__(self, num_player):
        switcher = {
            3: 35,
            4: 30,
            5: 25,
            6: 20
        }
        return switcher.get(num_player)

    async def _give_objective_cards(self):
        cards = utils.read_objects_cards()
        print("read all the objectives card from xml")
        for player in self.players:
            card_drawn = cards[random.randint(0, len(cards) - 1)]
            card_drawn.player_id = player.player_id
            player.objective_card = card_drawn
            cards.remove(card_drawn)
            print("Extracted OBJECTIVE " + card_drawn.id + " for player " + player.name)
            await player.sock.send("OBJECTIVE_CARD_ASSIGNED: " + json.dumps(Card.Card.to_dict(player.objective_card)))
            print("sent")
            #Per ricevere dalla socket e trasformarlo in oggetto:
            #received_dict = json.loads(received_data)
            #received_card = Card.from_dict(received_dict)


    async def _give_territory_cards(self):
        cards = utils.read_territories_cards()
        print("read all the territory card from xml")
        while cards:
            for player in self.players:
                if cards:
                    card_drawn = cards[random.randint(0, len(cards) - 1)]
                    card_drawn.player_id = player.player_id
                    player.addTerritory(card_drawn)
                    cards.remove(card_drawn)
                    print("Extracted TERRITORY card " + card_drawn.id + " for player " + player.name)
        for player in self.players:
            territories_list = [terr.to_dict() for terr in player.territories]
            await player.sock.send("TERRITORIES_CARDS_ASSIGNED: " + json.dumps(territories_list, indent=4)) # Indent only for better readable
        print("sent")

    async def _assignDefaultArmiesOnTerritories(self):
        print("Fase iniziale assegnazione armate default")
        control = 0
        while control < len(self.players):
            for player in self.players:
                if player.tanks_available > 0:
                    await player.sock.send("IS_YOUR_TURN: TRUE")
                    print(f"Turno del player id: {player.player_id} con nome {player.name}")
                    print(f"Armate totali: {player.tanks_num}")
                    print(f"Armate piazzate: {player.tanks_placed}")
                    print(f"Armate ancora da piazzare: {player.tanks_available}")
                    await self.event.wait() # Waiting for player choice
                    await player.sock.send("IS_YOUR_TURN: FALSE")
                    if player.tanks_available >= 3:
                        player.tanks_available -= 3
                        player.tanks_placed += 3
                    else:
                        player.tanks_placed += player.tanks_available
                        player.tanks_available = 0
                    print(f"Turno del player id: {player.player_id} con nome {player.name} TERMINATO")
                    self.event = asyncio.Event() # Event reset
                else:
                    control += 1


    def calculateArmyForThisTurn(self, player):
        #Continent name: NA SA EU AF AS OC
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

