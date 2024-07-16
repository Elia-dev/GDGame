import socket
import threading
import client_manager as cm
import logging

game_hosts = {}
hosts_lock = threading.Lock()


# Function to handle a single client connection
def handle_client(client_socket, client_address):
    print(f"Connection from {client_address} has been established.")

    # Start User Registration
    user = cm.registration_form(client_socket)
    print(f"Client {client_address} is registered as {user}")

    # Ask for hoster or joiner
    response = ""
    while response.lower() != "h" and response.lower() != "j":
        client_socket.send(
            "Choose if you want to host a game or join in an existing game\nHost -> \"H\" \nJoin -> \"J\"".encode(
                "utf-8"))

        response = client_socket.recv(1024).decode('utf-8')

    # Create or join the match
    if response.lower() == "j":
        host_id = cm.choose_game(client_socket, game_hosts)
        with hosts_lock:
            game_hosts[host_id].append(client_socket)
        client_socket.send(f"Correctly joined on match: {host_id}".encode('utf-8'))
        cm.match_making_joiner(client_socket, game_hosts, host_id, user)
    else:
        host_id = cm.host_creation(client_socket)
        with hosts_lock:
            game_hosts[host_id] = [client_socket]
        cm.match_making_owner(client_socket, game_hosts, host_id)

    # Create the game
    if client_socket == game_hosts[host_id][0]:
        print(f"Game {host_id} started with {len(game_hosts[host_id])} players")
        players = game_hosts[host_id]
        with hosts_lock:
            game_hosts.pop(host_id)
        cm.create_game(players, host_id)


# Function to start the server and listen for connections
def start_server(host='0.0.0.0', port=1234):
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((host, port))
    server.listen(5)
    print(f"Server listening on {host}:{port}")

    while True:
        client_socket, client_address = server.accept()
        client_handler = threading.Thread(target=handle_client, args=(client_socket, client_address))
        client_handler.start()


if __name__ == "__main__":
    start_server()
