import json
import logging
import socket
from universal_sentence_encoder import load_universal_sentence_encoder, calculate_cosine_similarity

path = '/universal_sentence_encoder'
logging.info(f"Universal Sentence Encoder Path: {path}")

embed = load_universal_sentence_encoder(path)

host = '0.0.0.0'
port = 63778

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((host, port))
server_socket.listen(1)

logging.info(f"Listening on {host}:{port}")

while True:
    client_socket, client_address = server_socket.accept()
    logging.info(f"Connection from {client_address}")

    try:
        data = client_socket.recv(4096).decode('utf-8')
        received_data = json.loads(data)
        logging.info(f"Received data: {received_data}")

        first_sentence = received_data['first']
        second_sentence = received_data['second']

        similarity_value = calculate_cosine_similarity(embed, first_sentence, second_sentence)

        response_data = similarity_value if similarity_value is not None else 0.0
        client_socket.sendall(json.dumps(response_data).encode('utf-8'))

    except Exception as e:
        logging.error(f"Error processing client data: {e}")

    finally:
        client_socket.close()
