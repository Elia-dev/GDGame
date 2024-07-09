import socket
import threading


def receive_messages(client_socket):
    while True:
        try:
            message = client_socket.recv(1024).decode('utf-8')
            if not message:
                break
            if message == 'START':
                client_socket.send("START".encode('utf-8'))
            print(f"SERVER: {message}")
        except ConnectionResetError:
            break
    client_socket.close()


def start_client(host='127.0.0.1', port=1234):
    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client.connect((host, port))

    print("Type \'exit\' anytime to quit")

    # Start a thread to handle receiving messages from the server
    receiver_thread = threading.Thread(target=receive_messages, args=(client,))
    receiver_thread.start()

    while True:
        message = input("")
        if message.lower() == 'exit':
            break
        client.send(message.encode('utf-8'))

    client.close()


if __name__ == "__main__":
    start_client()
