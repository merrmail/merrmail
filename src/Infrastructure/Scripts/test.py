import socket
import json
import logging

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')


def send_receive_data(ip, port, data):
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:
        client_socket.connect((ip, port))
        json_data = json.dumps(data)
        client_socket.sendall(json_data.encode('utf-8'))
        received_data = client_socket.recv(1024).decode('utf-8')
        return float(received_data)

    finally:
        client_socket.close()


server_ip = 'localhost'
server_port = 63778

json_payload = {"first": "Hello MerrMail", "second": "Hi MerrMail"}

result = send_receive_data(server_ip, server_port, json_payload)
print(f"Received float from server: {result}")
