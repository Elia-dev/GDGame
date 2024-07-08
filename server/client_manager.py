import socket
import random
import threading
import game_manager as gm


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
            if len(game_hosts[response]) > 6:
                client_socket.send("Full match making. Try another game id.\n".encode('utf-8'))
            break
        else:
            client_socket.send("This game does not exist\n".encode('utf-8'))
    return response


def match_making_joiner(client_socket, game_hosts, host_id, user):
    match_owner = game_hosts[host_id][0]
    match_owner.send(f"{user} join the game!\n".encode('utf-8'))
    players_ready = len(game_hosts[host_id])
    updated_players_ready = 0
    while True:
        if players_ready != updated_players_ready:
            updated_players_ready = players_ready
            if players_ready < 3:
                client_socket.send(f"{players_ready}/6 players. Waiting for others...\n".encode('utf-8'))
            else:
                client_socket.send(f"All players ready. Waiting for host to start the match.\n".encode('utf-8'))
                response = client_socket.recv(1024).decode('utf-8')
                if response == 'START':
                    break
        else:
            players_ready = len(game_hosts[host_id])


def match_making_owner(client_socket, game_hosts, host_id):
    players_ready = len(game_hosts[host_id])
    updated_players_ready = 0
    while True:
        if players_ready != updated_players_ready:
            updated_players_ready = players_ready
            if players_ready < 3:
                client_socket.send(f"{players_ready}/6 players. Waiting for others...\n".encode('utf-8'))
            else:
                while True:
                    client_socket.send(
                        f"{players_ready}/6 players.\n- 's': Start the match\n- 'u': Check for other players".encode(
                            'utf-8'))
                    message = client_socket.recv(1024).decode('utf-8')
                    if message.lower() == 's':
                        for i, player in enumerate(game_hosts[host_id]):
                            if i > 0:
                                player.send("START".encode('utf-8'))
                        break
                    else:
                        players_ready = len(game_hosts[host_id])
                break
        else:
            players_ready = len(game_hosts[host_id])


def send_miao(client_socket):
    client_socket.send(
        "Hey! If you have come this far, it means that the code I programmed works well\n".encode('utf-8'))
    while True:
        try:
            client_socket.send(
                "If you are proud of me type 'miao' to get a gift\nElse type 'exit' to close connection".encode(
                    'utf-8'))
            message = client_socket.recv(1024).decode('utf-8')
            if not message:
                break
            if (message.lower() == 'miao'):
                cat = """
 _._     _,-'""`-._
(,-.`._,'(       |\`-/|
    `-.-' \ )-`( , o o)
          `-    \`_`"'-\n"""
                client_socket.send(cat.encode('utf-8'))

        except ConnectionResetError:
            break


def _generate_game_id():
    return ' '.join([str(random.randint(0, 999)).zfill(3) for _ in range(2)])


def create_game(game_hosts, host_id):
    players = game_hosts[host_id]
    game_thread = threading.Thread(target=gm.game_main, args=(players, host_id))
    game_thread.start()
