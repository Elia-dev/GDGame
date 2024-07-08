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
            player.send(f"You got {gaming_dice}\n".encode("utf-8"))

    sorted_player = [item[0] for item in sorted(response.items(), key=lambda item: item[1])]
    sorted_player.reverse()
    for i, player in enumerate(sorted_player):
        player.send(f"You are the {i + 1}° player".encode("utf-8"))
    return sorted_player


def game_main(players, host_id):

    # Define the game order
    for player in players:
        player.send(f"Game {host_id} started!".encode("utf-8"))
    players = _game_order(players)


