import asyncio
import random

import websockets


class Game:
    def __init__(self, game_id):
        self.game_id = game_id
        self.players = []
        self.game_running = True
        self.game_waiting_to_start = True
        self.host_player = None
        self.queue = asyncio.Queue()

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

        tasks = [
            asyncio.create_task(self.listen_to_player_request(self.host_player)),
            asyncio.create_task(self.handle_requests),
            asyncio.create_task(self.handle_game)
        ]
        await asyncio.gather(*tasks)

    async def handle_game(self):
        while self.game_waiting_to_start is True:
            await asyncio.sleep(1)
        await self.__game_order__()




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
                # Qui puoi aggiungere la logica per gestire il messaggio
                # Ad esempio, rispondere al client, fare una richiesta ad un altro servizio, ecc.
                await asyncio.sleep(5)  # Simula il tempo di gestione della richiesta
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
        response = {}
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
            game_order.append(self.players[i].player_id + "-" + str(i) + ", ")
            game_order_extracted_numbers.append(self.players[i].player_id + "-" + str(response[self.players[i]]) + ", ")

        # Notify positions
        #Game order e GameOrderExtractedNumbers dovrebbero essere sostituiti da un broadcast
        for player in self.players:
            await player.sock.send("GAME_ORDER: " + str(*game_order))
            await player.sock.send("EXTRACTED_NUMBER: " + str(response[player]))
            await player.sock.send("GAME_ORDER_EXTRACTED_NUMBERS: " + str(game_order_extracted_numbers))
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

