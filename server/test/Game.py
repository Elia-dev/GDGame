import asyncio
import random

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
        self.players.append(player)
        print(f"Player {player.player_id} added to game {self.game_id}")

    def remove_player(self, player):
        self.players.remove(player)
        print(f"Player {player} removed from game {self.game_id}")

    async def broadcast(self, message):
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
            await asyncio.sleep(2)

        #Preparation phase
        await self.__game_order__()
        await self.broadcast("IS_YOUR_TURN: false") #TOBE Tested
        await self.army_color_chose() #TOBE Tested
        await self.broadcast("INITIAL_ARMY_NUMBER: " + str(self.__army_start_num__(len(self.players)))) #TOBE Tested
        await self._give_objective_cards() # TOBE Tested
        await self._give_territory_cards() #TOBE Tested


    async def handle_requests(self):
        while self.game_running:
            try:
                player, message = await self.queue.get()
                print(f"GAME: handling request from client id - : {player.player_id}: {message}")
                if "prova" in message:
                    print("Bella prova compà")
                    await player.sock.send("HAI MANDATO PROVA")
                    await player.sock.send("Bravo coglione")
                if "cane" in message:
                    print("I love dogs, doesn't everyone?")
                if "REQUEST_NAME_UPDATE_PLAYER_LIST" in message:
                    player_names = []
                    for p in self.players:
                        player_names.append(p.name)
                    await player.sock.send("REQUEST_NAME_UPDATE_PLAYER_LIST: " + str(player_names))
                if "GAME_STARTED_BY_HOST" in message:
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
                    print(f"Client {player.player_id} disconnected")
                    self.remove_player(player)

                '''try:
                    async for message in player.sock:
                        await self.queue.put((player, message))
                except websockets.exceptions.ConnectionClosed:
                    print(f"Client {player} disconnected")
                    '''

    async def listen_to_player_request(self, player):
        while True:  # Da cambiare mettendo finché il giocatore può giocare/è ancora in gioco
            try:
                async for message in player.sock:
                    await self.queue.put((player, message))
            except websockets.exceptions.ConnectionClosed:
                print(f"Client {player} disconnected")
                self.remove_player(player)

    def end_game(self):
        self.game_running = False
        print(f"Game {self.game_id} is ending.")

    async def __game_order__(self):
        print("!!! ENTRATO IN GAME ORDERD !!!")
        response = {}
        for player in self.players:
            gaming_dice = random.randint(1, 6)
            response[player] = gaming_dice
        print("|| LANCIATI DADI||")
        sorted_players = [item[0] for item in sorted(response.items(), key=lambda item: item[1])]
        sorted_players.reverse()

        for i in range(len(sorted_players)):
            self.players[i] = sorted_players[i]
        print("|| SORTATI PLAYERS||")

        game_order = []
        game_order_extracted_numbers = []
        for i in range(len(self.players)):
            game_order.append(self.players[i].player_id + "-" + str(i) + ", ")
            game_order_extracted_numbers.append(self.players[i].player_id + "-" + str(response[self.players[i]]) + ", ")

        # Notify positions
        #Game order e GameOrderExtractedNumbers dovrebbero essere sostituiti da un broadcast
        print(f"Game order: {''.join(game_order)}")
        fgameOrder = {''.join(game_order)}
        print(f"GAME_ORDER_EXTRACTED_NUMBERS: {str(game_order_extracted_numbers)}")
        for player in self.players:
            print(f"For player {player.name} extracted number {str(response[player])} ")
            await player.sock.send("GAME_ORDER: " + str(fgameOrder))
            await player.sock.send("GAME_ORDER_EXTRACTED_NUMBERS: " + str(game_order_extracted_numbers))
            await player.sock.send("EXTRACTED_NUMBER: " + str(response[player]))
        print("done")

        '''
        Struttura messaggi:
        GAME_ORDER: idPlayer-position, idPlayer-position
        EXTRACTED_NUMBER: number
        GAME_ORDER_EXTRACTED_NUMBERS: idPlayer-extracted_number, idPlayer-extracted_number
        
        players = []
        for i, player in enumerate(sorted_players):
            # player.send(f"You are the {i + 1}° player".encode("utf-8"))
            players.append(Player(player))'''

    def _remove_request(self, source, request):
        value = source.replace(request, "")
        # print(f"VALORE CALCOLATO: {value}")
        return value

    async def army_color_chose(self):
        for player in self.players:
            available_colors = [color for color, user_id in self.army_colors.items() if user_id is None]
            await player.sock.send("AVAILABLE_COLORS: " + ", ".join(available_colors))
            await player.sock.send("IS_YOUR_TURN: true")
            await self.event.wait() # Waiting for player choice
            await player.sock.send("IS_YOUR_TURN: false")
            self.event = asyncio.Event() # Event reset

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
        for player in self.players:
            card_drawn = cards[random.randint(0, len(cards) - 1)]
            card_drawn.player_id = player.player_id
            player.objective_card = card_drawn
            cards.remove(card_drawn)
            await player.sock.send("OBJECTIVE_CARD_ASSIGNED: " + Card.Card.to_dict(player.objective_card))

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
            await player.sock.send("TERRITORIES_CARDS_ASSIGNED: " + Territory.Territory.to_dict(player.territories))
