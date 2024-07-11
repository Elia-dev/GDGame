import json
import socket

from Player import Player


def mock_start_server(host='0.0.0.0', port=1234):
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((host, port))
    server.listen(5)
    print("waiting client")

    client_socket, client_address = server.accept()
    data = client_socket.recv(4096)
    player_dict = json.loads(data.decode('utf-8'))
    player = Player.from_dict(player_dict)
    print(player)
    client_socket.close()


mock_start_server()