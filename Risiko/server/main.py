import socket
import threading
import client_manager as cm

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
        client_socket.send("Choose if you want to host a game or join in an existing game\nHost -> \"H\" \nJoin -> \"J\"".encode("utf-8"))
        response = client_socket.recv(1024).decode('utf-8')
    if (response.lower() == "j"):
        joined_game = cm.choose_game(client_socket, game_hosts)
        client_sockets = game_hosts[joined_game]
        client_sockets.append(client_socket)
        with hosts_lock:
            game_hosts[joined_game] = client_sockets
        client_socket.send(f"Correctly joined on match: {joined_game}".encode('utf-8'))
    else:
        host_id = cm.host_creation(client_socket)
        with hosts_lock:
            game_hosts[host_id] = [client_socket]

    client_socket.send("Hey! If you have come this far, it means that the code I programmed works well\n".encode('utf-8'))
    while True:
        try:
            client_socket.send("If you are proud of me type 'miao' to get a gift\nElse type 'exit' to close connection".encode('utf-8'))
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

    print(f"Connection from {client_address} has been closed.")
    client_socket.close()

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