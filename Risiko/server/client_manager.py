import socket
import random

def registration_form(client_socket):
    client_socket.send("Enter the name you will be displayed with online.".encode("utf-8"))
    user = client_socket.recv(1024).decode('utf-8')
    message = ""
    while message.lower() != "y" and message.lower() != "n":
        client_socket.send(f"Confirm {user} as your online name? [Y/n]".encode("utf-8"))
        message = client_socket.recv(1024).decode('utf-8')
    if message.lower() == "n":
        return registration_form(client_socket)
    client_socket.send(f"Hello {user}! Welcome to Risiko!!\n".encode("utf-8"))
    return user


def host_creation(client_socket):    
    game_id = _generate_game_id()
    client_socket.send(f"Match {game_id} correctly generated\n".encode('utf-8'))
    return game_id

def choose_game(client_socket, game_hosts):
    while True:
        client_socket.send("Enter a valid game id".encode('utf-8'))
        response = client_socket.recv(1024).decode('utf-8')
        if response in game_hosts:
            break
        client_socket.send("This game does not exist\n".encode('utf-8'))
    return response


def _generate_game_id():
    return ' '.join([str(random.randint(0, 999)).zfill(3) for _ in range(2)])

