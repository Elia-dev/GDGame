import asyncio
import json
import random
import time
import websockets
import os

import Card
import Territory
import utils


class Game:
    def __init__(self, game_id):
        self.game_id = game_id
        self.players = []
        self.dead_players = []
        self.game_running = True
        self.game_waiting_to_start = True
        self.host_player = None
        self.adj_matrix = utils.get_adj_matrix(os.getcwd() + '/assets/adj_matrix.npy')
        print('SERVER: ' + os.getcwd() + '/assets/adj_matrix.npy')
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
        self.event_strategic_movement = asyncio.Event()
        self.firstRound = True

    def add_player(self, player):
        if player.name is None:
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
        #print(f"sending broadcast message: {message}")
        for player in self.players:
            await player.sock.send(message)

    async def create_game(self, player):
        self.host_player = player
        self.players.append(player)
        #print("Aggiunto nuovo HOST")
        tasks = [
            asyncio.create_task(self.listen_to_player_request(self.host_player)),
            asyncio.create_task(self.handle_requests()),
            asyncio.create_task(self.handle_game())
        ]
        await asyncio.gather(*tasks)

    async def handle_game(self):
        #print("Aperto HANDLE_GAME\n")
        print("ASPETTANDO CHE SI COLLEGHINO TUTTI\n")
        while self.game_waiting_to_start is True:
            if self.game_id is None:
                return
            await asyncio.sleep(1)

        # Preparation phase
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
        #print("Initial army number: " + str(initial_army_number))
        for player in self.players:
            player.tanks_num = initial_army_number
            player.tanks_placed = len(player.territories)
            player.tanks_available = player.tanks_num - player.tanks_placed
        await self.broadcast("INITIAL_ARMY_NUMBER: " + str(initial_army_number))
        await self.broadcast("IS_YOUR_TURN: FALSE")
        await self.army_color_chose()
        dict_id_color = "ID_COLORS_DICT: "
        dict_id_color += ", ".join([f"{player.player_id}-{player.army_color}" for player in self.players])
        #print("DICT ID COLOR DA MANDARE: " + dict_id_color)
        await self.broadcast(dict_id_color)
        await self._give_objective_cards()
        await self._assignDefaultArmiesOnTerritories()
        await self.broadcast("PREPARATION_PHASE_TERMINATED")
        print("PREPARATION_PHASE_TERMINATED")
        # Preparation phase terminated

        # Game loop TOBE TESTED
        print("---INIZIO FASE DI GIOCO---")
        while self.game_running:
            for player in self.players:
                await self.broadcast("PLAYER_TURN: " + player.player_id)
                print(f"Turno del player id: {player.player_id} con nome {player.name}")
                if not self.firstRound:
                    print("REINFORCE PHASE")
                    # REINFORCE PHASE
                    # CheckContinents
                    # CheckArmy
                    num_army_to_send = self.calculateArmyForThisTurn(player)  #
                    #print(f"Numero di armate ricevute nella fase di rinforzo: {num_army_to_send}")
                    player.tanks_num += num_army_to_send
                    player.tanks_available += num_army_to_send
                    # SendArmy
                    await player.sock.send("NUMBER_OF_ARMY_TO_ASSIGN_IN_THIS_TURN: " + str(num_army_to_send))
                    # UnlockTurn
                    await player.sock.send("IS_YOUR_TURN: TRUE")
                    # Waiting for player to finish the turn and send updated territories
                    print("Waiting for player to end the reinforce phase")
                    await self.event.wait()  # Aggiorna lo stato di tutti i territori ai client
                    self.event = asyncio.Event()  # Event reset
                    player.tanks_available = 0
                    player.tanks_placed += num_army_to_send
                    print("Reinforced phase terminated")
                    # REINFORCE PHASE TERMINATED
                else:
                    await player.sock.send("IS_YOUR_TURN: TRUE")

                # FIGHT PHASE OR STRATEGIC MOVEMENT PHASE
                print("Fight phase started")
                self.event_strategic_movement = asyncio.Event()
                '''
                 Finché non fa il movimento strategico aspetto che attacchi
                 appena attacca eseguo l'attacco e mi rimetto in attesa di un nuovo attacco
                '''
                while not self.event_strategic_movement.is_set():
                    self.event = asyncio.Event()
                    await self.event.wait()  # Attendo un attacco o un movimento strategico

                print("Fight phase terminated")
                # FIGHT PHASE AND STRATEGIC MOVEMENT TERMINATED

                self.event = asyncio.Event()
                self.event_strategic_movement = asyncio.Event()
                print("Strategic movement terminated")
                await player.sock.send("IS_YOUR_TURN: FALSE")

                # CHECK (card objective, number of tanks ecc...)
                if self.check_for_victory(player) is True:
                    await self.broadcast("WINNER: " + player.player_id)
                    print(f"Il player {player.name} ha vinto")
                    self.game_running = False
                print(" Check objective card terminated")
            self.firstRound = False

    async def handle_requests(self):
        while self.game_running:
            try:
                player, message = await self.queue.get()
                #print(
                #    f"GAME: handling request from client id - : {player.player_id} with name {player.name}: {message}")

                if "LOBBY_KILLED_BY_HOST" in message:
                    id = self._remove_request(message, "LOBBY_KILLED_BY_HOST: ")
                    for player in self.players:
                        if player.player_id != id:
                            await player.sock.send("LOBBY_KILLED_BY_HOST")
                    self.game_id = None
                    self.remove_all_players()
                    return
                if "PLAYER_HAS_LEFT_THE_GAME" in message:
                    # TODO: IMPLEMENTARE RIMPIAZZO CON BOT
                    # Implementazione temporanea sotto, chiude la partita
                    print("Ricevuto PLAYER_HAS_LEFT_THE_GAME, chiusura della partita in corso...")
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
                    self.event.set()  # Setting event to True

                if "UPDATE_TERRITORIES_STATE:" in message:
                    message = self._remove_request(message, "UPDATE_TERRITORIES_STATE: ")
                    id = message.split(", ")[0]
                    message = self._remove_request(message, (id + ", "))
                    territories_list_dict = json.loads(message)
                    territories = [Territory.Territory.from_dict(data) for data in territories_list_dict]
                    for player in self.players:
                        if player.player_id == id:
                            player.territories = territories
                            #print(f"Aggiornato lista stati del player con ID {player.player_id} con nome {player.name}")
                    #print("Fine aggiornamento territori su server")

                    territories_list = []
                    for player in self.players:
                        for territory in player.territories:
                            territories_list.append(territory.to_dict())

                    # Invece del broadcast potrei mandare la lista a tutti i player eccetto il player che ha appena effettuato il turno

                    await self.broadcast("SEND_TERRITORIES_TO_ALL: " + json.dumps(territories_list, indent=4))
                    print("Fine aggiornamento territori, mandati al client")
                    #print("PASSATO ASSEGNAZIONE TERRITORI OPPURE MOVIMENTO STRATEGICO")
                    self.event_strategic_movement.set()  # Sfrutto la stessa funzione per controllare se il giocatore effettua il movimento strategico
                    # durante la fase di gioco
                    self.event.set()
                    # Aggiorno la matrice di adiacenza
                    self.adj_matrix = utils.update_adjacent_matrix(self.players, self.adj_matrix)

                if "REQUEST_TERRITORY_INFO:" in message:  # TOBE TESTED
                    message = self._remove_request(message, "REQUEST_TERRITORY_INFO: ")
                    playerId, territoryId = message.split("-")
                    tempPlayer = None
                    for player in self.players:
                        if player.player_id == playerId:
                            tempPlayer = player

                    for player in self.players:
                        for territory in player.territories:
                            if territory.id == territoryId:
                                await tempPlayer.sock.send(
                                    "RECEIVED_REQUEST_TERRITORY_INFO: " + json.dumps(territory.to_dict()))

                if "ATTACK_TERRITORY:" in message:
                    # (parte animazione su clientAttaccante con messaggio C->S) TERRITORY_ATTACK: idPlayerAttaccante-idPlayerDifensore, idTerrAttaccante-idTerrDifensore, numArmateAttaccante-numArmateDifensore

                    random.seed(time.time())
                    attacker_player = None
                    defender_player = None
                    attacker_territory = None
                    defender_territory = None
                    # Received TERRITORY_ATTACK: idPlayerAttaccante-idPlayerDifensore, idTerrAttaccante-idTerrDifensore, numArmateAttaccante-numArmateDifensore
                    message = self._remove_request(message, "ATTACK_TERRITORY: ")

                    # Split the message in segments using comma as a separator removing blank space at the start and the end of the message
                    clean_message = [segmento.strip() for segmento in message.split(",")]
                    # Extract values separated by "-" removing extra blank spaces
                    attacker_id, defender_id = clean_message[0].split("-")
                    for player in self.players:
                        if player.player_id == attacker_id:
                            attacker_player = player
                        if player.player_id == defender_id:
                            defender_player = player
                    # print(f"!!!OH MY GOD, {attacker_player.name} IS TRYING TO FUCK {defender_player.name}")
                    #print(f"id attaccante: {attacker_player.player_id}  id difensore: {defender_player.player_id}")
                    attacker_ter_id, defender_ter_id = clean_message[1].split("-")
                    for terr in attacker_player.territories:
                        if terr.id == attacker_ter_id:
                            attacker_territory = terr
                    for terr in defender_player.territories:
                        if terr.id == defender_ter_id:
                            defender_territory = terr

                    print(
                        f"{attacker_player.name} IS USING {attacker_territory.name} TO FUCK {defender_territory.name} OWNED BY {defender_player.name}")
                    attacker_army_num, defender_army_num = clean_message[2].split("-")
                    attacker_army_num = int(attacker_army_num)
                    defender_army_num = int(defender_army_num)
                    print(
                        f"{attacker_player.name} WILL USE {str(attacker_army_num)} AGAINST {str(defender_army_num)} ARMY OWNED BY {defender_player.name}")
                    #print(f"Prima di attaccare {attacker_territory.name} aveva {attacker_territory.num_tanks} armate")
                    #print(f"Prima di essere attaccato {defender_territory.name} aveva {defender_territory.num_tanks} armate")

                    # genera n numeri casuali, con n numero di armate
                    extracted_numbers_attacker = [random.randint(1, 6) for _ in range(attacker_army_num)]
                    extracted_numbers_defender = [random.randint(1, 6) for _ in range(defender_army_num)]
                    extracted_numbers_attacker.sort(reverse=True)  # Sort in descending order
                    extracted_numbers_defender.sort(reverse=True)

                    await attacker_player.sock.send(
                        "ATTACKER_ALL_EXTRACTED_DICE: " + extracted_numbers_attacker.__str__() + ", " + extracted_numbers_defender.__str__())
                    #print("Server ha avvisato l'attaccante che sta per essere scassato")
                    # Tell the defender it's under attack
                    await defender_player.sock.send(
                        "UNDER_ATTACK: " + attacker_id + ", " + attacker_ter_id + "-" + defender_ter_id + ", "
                        + str(attacker_army_num) + "-" + str(
                            defender_army_num) + ", " + extracted_numbers_attacker.__str__() + ", " + extracted_numbers_defender.__str__())
                    #print("Server ha avvisato il difensore che sta per essere scassato")

                    #await asyncio.sleep(0.5) # Diamo il tempo di fare "l'animazione" al client

                    attacker_wins = 0
                    defender_wins = 0
                    #print(
                    #   f"Generati {len(extracted_numbers_attacker)} numeri per l'attaccante, {len(extracted_numbers_defender)} per il difensore")
                    print("Numeri dell'attaccante: ", extracted_numbers_attacker)
                    print("Numeri del difensore: ", extracted_numbers_defender)
                    #print("Inizio confronto:")
                    # Confronto, in ordine, del più grande dell'attaccante con il più piccolo dell'attaccante
                    for attacker_num, defender_num in zip(extracted_numbers_attacker, extracted_numbers_defender):
                        #print(f"Num att: {attacker_num} num def: {defender_num}")
                        if attacker_num > defender_num:
                            # print("L'attaccante vince il confronto")
                            attacker_wins += 1
                        else:
                            # print("Il difensore vince il confronto")
                            defender_wins += 1
                    # print(f"ATTACCANTE VINCE {attacker_wins} CONFRONTI, DIFENSORE VINCE {defender_wins} CONFRONTI")
                    # Rimuove i carri in funzione del risultato precedente
                    attacker_territory.num_tanks -= defender_wins
                    defender_territory.num_tanks -= attacker_wins

                    print(
                        f"Armate rimaste per l'attaccante {attacker_territory.name} posseduto da {attacker_player.name}: {attacker_territory.num_tanks}")
                    print(
                        f"Armate rimaste per il difensore {defender_territory.name} posseduto da {defender_player.name}: {defender_territory.num_tanks}")

                    #print(f"Prima {attacker_player.name} aveva {len(attacker_player.territories)} territori")
                    #print(f"Sempre prima {defender_player.name} aveva {len(defender_player.territories)} territori")
                    if defender_territory.num_tanks == 0:  # Capisce se il territorio attaccato è stato conquistato oppure no
                        print(f"OH NO! {defender_player.name} È STATO SCOPATO ED HA PERSO IL TERRITORIO!!")
                        defender_territory.player_id = attacker_id
                        defender_player.removeTerritory(defender_territory)
                        defender_territory.num_tanks = attacker_army_num - defender_wins
                        attacker_player.addTerritory(defender_territory)
                        print(
                            f"Adesso {defender_territory.name} appartinene a {defender_territory.player_id} con {defender_territory.num_tanks} armate")

                    #print(f"Adesso {attacker_player.name} ha {len(attacker_player.territories)} territori")
                    #print(f"Sempre adesso {defender_player.name} ha {len(defender_player.territories)} territori")
                    # updateAllTerritories in broadcast e controllo lato client della vittoria/sconfitta
                    territories_list = []
                    for player in self.players:
                        for territory in player.territories:
                            territories_list.append(territory.to_dict())

                    await self.broadcast("SEND_TERRITORIES_TO_ALL: " + json.dumps(territories_list, indent=4))
                    #await attacker_player.sock.send("ATTACK_RESULT_ATTACKER: ")
                    #await defender_player.sock.send("ATTACK_RESULT_DEFENDER: ")
                    # Mandare un messaggio all'attaccante e all'attaccato per dirgli che l'attacco è finito?
                    #print("Aggiornati tutti i client sul risultato del combattimento")
                    if len(defender_player.territories) == 0:
                        defender_player.killed_by = attacker_player
                        self.dead_players.append(defender_player)
                        self.players.remove(defender_player)
                        print(
                            f"Il brother {defender_player.name} è stato scopato così forte che è morto nell'atto e adesso verrà rimosso dalla lista dei players vivi per essere messo in quella dei players scassati...")
                        #print("done")
                    await self.broadcast("ATTACK_FINISHED_FORCE_UPDATE")
                    self.event.set()

                if 'REQUEST_SHORTEST_PATH' in message:
                    message = self._remove_request(message, "REQUEST_SHORTEST_PATH: ")
                    player_id, territories_json = message.split("-")
                    territories_list_dict = json.loads(territories_json)
                    territories = [Territory.Territory.from_dict(data) for data in territories_list_dict]
                    print(f'Player: {player_id} request shortest paths between: {len(territories)} territories')
                    friends_territories = list(
                        filter(lambda terr: terr.player_id == player_id, territories)
                    )
                    print(f'Friends territories: {len(friends_territories)}')
                    enemies_territories = list(
                        filter(lambda terr: terr.player_id != player_id, territories)
                    )
                    print(f'Enemy territories: {len(enemies_territories)}')
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
                        print(f'Send {len(paths)} to {player.name}')
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
        print(f"Game {self.game_id} is terminated.")

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

        #print(f"Game order: {''.join(game_order)}")
        fgame_order = {''.join(game_order)}
        #print(f"GAME_ORDER_EXTRACTED_NUMBERS: {str(game_order_extracted_numbers)}")
        for player in self.players:
            #print(f"For player {player.name} extracted number {str(response[player])} ")
            await player.sock.send("EXTRACTED_NUMBER: " + str(response[player]))
        await self.broadcast("GAME_ORDER: " + str(fgame_order))
        await self.broadcast("GAME_ORDER_EXTRACTED_NUMBERS: " + str(game_order_extracted_numbers))
        print("Game order terminated")

    def _remove_request(self, source, request):
        value = source.replace(request, "")
        return value

    async def army_color_chose(self):
        for player in self.players:
            available_colors = [color for color, user_id in self.army_colors.items() if user_id is None]
            #print("Available color in this turn: " + available_colors.__str__())
            await player.sock.send("AVAILABLE_COLORS: " + ", ".join(available_colors))
            await player.sock.send("IS_YOUR_TURN: TRUE")
            #print("TURNO DI " + player.name)
            await self.event.wait()  # Waiting for player choice
            await player.sock.send("IS_YOUR_TURN: FALSE")
            self.event = asyncio.Event()  # Event reset
            #print("Turn over for player " + player.name + "chosen color: " + player.army_color)
        print("Color chose terminated")

    def __army_start_num__(self, num_player):
        switcher = {
            2: 24,
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

        #print("read all the objectives card from xml")
        for player in self.players:
            control = True
            while control:
                card_drawn = cards[random.randint(0, len(cards) - 1)]
                if (player.army_color == "red" and card_drawn.id == "obj9") or (
                        player.army_color == "blue" and card_drawn.id == "obj10") or (
                        player.army_color == "green" and card_drawn.id == "obj11"):
                    print("!!!!!CARTA OBIETTIVO ESTRATTA NON VALIDA!!!!!")
                #print(f"Estratto {card_drawn.id} con colore armata  {player.army_color}")
                elif ("red" not in color_list and card_drawn.id == "obj9") or (
                        "blue" not in color_list and card_drawn.id == "obj10") or (
                        "green" not in color_list and card_drawn.id == "obj11"):
                    print("!!!!!CARTA OBIETTIVO ESTRATTA NON VALIDA!!!!!")
                    print(f"Estratto {card_drawn.id} con colori armate presenti in gioco  {color_list}")
                else:
                    control = False
            card_drawn.player_id = player.player_id
            player.objective_card = card_drawn
            cards.remove(card_drawn)
            #print("Extracted OBJECTIVE " + card_drawn.id + " for player " + player.name)
            await player.sock.send("OBJECTIVE_CARD_ASSIGNED: " + json.dumps(Card.Card.to_dict(player.objective_card)))
        print("Objective cards distribuited")
        # Per ricevere dalla socket e trasformarlo in oggetto:
        # received_dict = json.loads(received_data)
        # received_card = Card.from_dict(received_dict)

    async def _give_territory_cards(self):
        cards = utils.read_territories_cards()
        #print("read all the territory card from xml")
        while cards:
            for player in self.players:
                if cards:
                    card_drawn = cards[random.randint(0, len(cards) - 1)]
                    card_drawn.player_id = player.player_id
                    player.addTerritory(card_drawn)
                    cards.remove(card_drawn)
                    #print("Extracted TERRITORY card " + card_drawn.id + " for player " + player.name)

        for player in self.players:
            territories_list = [terr.to_dict() for terr in player.territories]
            await player.sock.send("TERRITORIES_CARDS_ASSIGNED: " + json.dumps(territories_list,
                                                                               indent=4))  # Indent only for better readable
        print("Territory cards distribuited")

    async def _assignDefaultArmiesOnTerritories(self):
        #print("Fase iniziale assegnazione armate default")
        control = 0
        while control < len(self.players):
            for player in self.players:
                if player.tanks_available > 0:
                    await self.broadcast("PLAYER_TURN: " + player.player_id)
                    await player.sock.send("IS_YOUR_TURN: TRUE")
                    #print(f"Turno del player id: {player.player_id} con nome {player.name}")
                    #print(f"Armate totali: {player.tanks_num}")
                    #print(f"Armate piazzate: {player.tanks_placed}")
                    # print(f"Armate ancora da piazzare: {player.tanks_available}")
                    await self.event.wait()  # Waiting for player choice
                    await player.sock.send("IS_YOUR_TURN: FALSE")
                    if player.tanks_available >= 3:
                        player.tanks_available -= 3
                        player.tanks_placed += 3
                    else:
                        player.tanks_placed += player.tanks_available
                        player.tanks_available = 0
                    print(f"Turno del player id: {player.player_id} con nome {player.name} TERMINATO")
                    self.event = asyncio.Event()  # Event reset
                else:
                    control += 1
        print("Assign default armies terminated")

    def calculateArmyForThisTurn(self, player):  # TOBE TESTED
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
        #print("Armate bonus per questo turno: " + str(totalArmyToAssing))
        return totalArmyToAssing

    def check_for_victory(self, player):  # TOBE TESTED
        NA_count = 0
        SA_count = 0
        EU_count = 0
        AF_count = 0
        AS_count = 0
        OC_count = 0
        print(f"Il brother {player.name} ha l'obiettivo {player.objective_card.id}")
        if player.objective_card.id == "obj1":
            army_count = 0
            if len(player.territories) >= 18:
                for terr in player.territories:
                    if terr.num_tanks >= 2:
                        army_count += 1
                if army_count >= 18:
                    return True
            print("Il fratello deve ancora conquistare 18 stati e metterci 2 carri ognuno")
            return False

        if player.objective_card.id == "obj2":
            if len(player.territories) >= 24:
                return True
            print("Il fratello deve ancora conquistare 24 stati")
            return False

        if player.objective_card.id == "obj3":
            for territory in player.territories:
                if territory.continent == "NA":
                    NA_count += 1
                elif territory.continent == "AF":
                    AF_count += 1
            if NA_count == 9 and AF_count == 6:
                return True
            print("Il fratello deve ancora conquistare il Nord America e l'Africa")
            return False

        if player.objective_card.id == "obj4":
            for territory in player.territories:
                if territory.continent == "NA":
                    NA_count += 1
                elif territory.continent == "OC":
                    OC_count += 1
            if NA_count == 9 and OC_count == 4:
                return True
            print("Il fratello deve ancora conquistare il Nord America e l'Oceania")
            return False

        if player.objective_card.id == "obj5":
            for territory in player.territories:
                if territory.continent == "AS":
                    AS_count += 1
                elif territory.continent == "OC":
                    OC_count += 1
            if AS_count == 12 and OC_count == 4:
                return True
            print("Il fratello deve ancora conquistare l'Asia e l'Oceania")
            return False

        if player.objective_card.id == "obj6":
            for territory in player.territories:
                if territory.continent == "AS":
                    AS_count += 1
                elif territory.continent == "AF":
                    AF_count += 1
            if AS_count == 12 and AF_count == 6:
                return True
            print("Il fratello deve ancora conquistare l'Asia e l'Africa")
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
            print("Il fratello deve ancora conquistare l'Europa, il Sud America e un terzo continente")
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
            print("Il fratello deve ancora conquistare l'Europa, l'Oceania e un terzo continente")
            return False

        if player.objective_card.id == "obj9":  # DA IMPLEMENTARE IL SALVATAGGIO DEL KILLER DENTRO AGLI ATTACCHI
            control = 0
            killer = None
            enemy_player = next((player for player in self.players if player.army_color == "red"), None)
            if enemy_player is None:
                for dead_player in self.dead_players:
                    if dead_player.army_color == "red":
                        if dead_player.killed_by.player_id == player.player_id:
                            return True
                        else:
                            killer = dead_player.killed_by
            else:
                return False
            while control < (len(self.players) + len(self.dead_players)):
                if killer in self.players:
                    print(
                        "Il fratello deve ancora uccidere il king con l'armata rossa o quello che gli ha rubato la kill")
                    return False
                else:
                    if killer.killed_by.player_id == player.player_id:
                        return True
                    else:
                        killer = killer.killed_by
                control += 1
            print("Il fratello deve ancora uccidere il king con l'armata rossa o quello che gli ha rubato la kill")
            return False

        if player.objective_card.id == "obj10":
            control = 0
            killer = None
            enemy_player = next((player for player in self.players if player.army_color == "blue"), None)
            if enemy_player is None:
                for dead_player in self.dead_players:
                    if dead_player.army_color == "blue":
                        if dead_player.killed_by.player_id == player.player_id:
                            return True
                        else:
                            killer = dead_player.killed_by
            else:
                return False
            while control < (len(self.players) + len(self.dead_players)):
                if killer in self.players:
                    print(
                        "Il fratello deve ancora uccidere il king con l'armata blu o quello che gli ha rubato la kill")
                    return False
                else:
                    if killer.killed_by.player_id == player.player_id:
                        return True
                    else:
                        killer = killer.killed_by
                control += 1
            print("Il fratello deve ancora uccidere il king con l'armata blu o quello che gli ha rubato la kill")
            return False

        if player.objective_card.id == "obj11":
            control = 0
            killer = None
            enemy_player = next((player for player in self.players if player.army_color == "green"), None)
            if enemy_player is None:
                for dead_player in self.dead_players:
                    if dead_player.army_color == "green":
                        if dead_player.killed_by.player_id == player.player_id:
                            return True
                        else:
                            killer = dead_player.killed_by
            else:
                return False
            while control < (len(self.players) + len(self.dead_players)):
                if killer in self.players:
                    print(
                        "Il fratello deve ancora uccidere il king con l'armata verde o quello che gli ha rubato la kill")
                    return False
                else:
                    if killer.killed_by.player_id == player.player_id:
                        return True
                    else:
                        killer = killer.killed_by
                control += 1
            print("Il fratello deve ancora uccidere il king con l'armata verde o quello che gli ha rubato la kill")
            return False
