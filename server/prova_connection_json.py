import json
import socket

from Card import Card
from Player import Player
from Territory import Territory


def mock_client_conn(player):
    host = '127.0.0.1'
    port = 1234
    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client.connect((host, port))
    player.sock = client
    player_json = json.dumps(player.to_dict())

    player.sock.send(player_json.encode('utf-8'))
    player.sock.close()


player = Player("Player1", "socket_placeholder", "lobby123", "player123")
# Aggiungi una Card e un Territory per esempio
player.objective_card.append(Card(1, "image1.png", "obj", "description1", "player123"))
player.territories.append(Territory(2, "image2.png", "ter", "description2", "player123", "Territory1", 5, "Continent1"))

# Converti l'oggetto Player in JSON

mock_client_conn(player)