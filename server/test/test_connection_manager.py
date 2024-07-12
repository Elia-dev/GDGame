import json
import socket
import unittest

from Card import Card
from Player import Player
from Territory import Territory


class MyTestCase(unittest.TestCase):
    def test_receving_json_server(self):
        playerHost = Player("sock", "Player1", "lobby123", "player123")
        playerHost.objective_card = (Card(1, "image1.png", "obj", "description1", "player123"))
        playerHost.territories.append(
            Territory(2, "image2.png", "ter", "description2", "player123", "Territory1", 5, "Continent1"))

        host = '0.0.0.0'
        port = 1234
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind((host, port))
        server.listen(5)
        print("waiting client")

        client_socket, client_address = server.accept()
        data = client_socket.recv(4096)
        player_dict = json.loads(data.decode('utf-8'))
        playerClient = Player.from_dict(player_dict)
        client_socket.close()
        self.assertEqual(playerHost.__repr__(), playerClient.__repr__(), "Json received does not match the one sent")


if __name__ == '__main__':
    unittest.main()
