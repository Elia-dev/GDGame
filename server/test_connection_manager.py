import json
import socket
import unittest

from Card import Card
from Player import Player
from Territory import Territory
import game_manager as gm


class MyTestCase(unittest.TestCase):
    '''
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
'''

    def test_receving_json_request_tank_assignment(self):
        playerHost = Player("sock", "Player1", "lobby123", "player123")
        playerHost.objective_card = (Card(1, "image1.png", "obj", "description1", "player123"))
        playerHost.territories.append(
            Territory(2, "image2.png", "ter", "description2", "player123", "Territory1", 0, "Continent1"))

        host = '0.0.0.0'
        port = 1234
        server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server.bind((host, port))
        server.listen(5)
        print("waiting client")

        client_socket, client_address = server.accept()
        playerHost.sock = client_socket
        playerClient = gm._request_tank_assignment(playerHost, 3)

        '''
        data = client_socket.recv(4096)
        player_dict = json.loads(data.decode('utf-8'))
        playerClient = Player.from_dict(player_dict)
        '''

        client_socket.close()
        self.assertEqual(playerClient.tanks_placed, playerHost.tanks_placed + 3,
                         "Error initial tank assignment")
        self.assertEqual(playerClient.tanks_available, playerHost.tanks_available - 3,
                         "Error initial tank assignment")
        self.assertEqual(playerClient.territories[0].num_tanks, playerHost.territories[0].num_tanks + 3,
                         "Error initial tank assignment")


if __name__ == '__main__':
    unittest.main()
