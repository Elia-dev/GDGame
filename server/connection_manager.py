import json


def update_state(players):
    for player in players:
        # Converti lo stato dell'oggetto Player in JSON
        player_json = json.dumps(player.to_dict())

        player.sock.sendall(player_json.encode('utf-8'))
