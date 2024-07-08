import random
from collections import OrderedDict


def _game_order(players):
    response = {}
    while len(response) < 4:
        for player in players:
            player.send(f"Press any key to throw a gaming dice!".encode("utf-8"))
            player.recv(1024).decode("utf-8")
            gaming_dice = random.randint(1, 6)
            response[player] = gaming_dice
            player.send(f"You got {gaming_dice}")

    sorted_player = OrderedDict(sorted(response.items(), key=lambda item: item[1]))
    return sorted_player


def game_main(players, host_id):
    for player in players:
        player.send(f"Game {host_id} started!".encode("utf-8"))

    players = _game_order(players)
    

