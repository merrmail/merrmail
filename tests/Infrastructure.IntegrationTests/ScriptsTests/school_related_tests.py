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

first_array = [
    "Inquiry Regarding Printing Services",
    "Where can I print my stuff?",
    "Me hungry what time I can eat",
    "B236 Room Location",
    "Uniform Purchase Location",
    "Inquiry Regarding Summer Classes",
    "Enrollment Process Inquiry",
    "Inquiry on Enrollment Process",
    "where can i pay school tuition?",
    "Guidance Needed: Selecting Courses",
    "Academic Advising Appointment",
    "Application Submission Assistance",
    "Information on Financial Aid and Scholarships",
    "I have a problem",
    "Question about Upcoming Exam",
    "Question about Tuition Fees",
    "Tuition Fee Adjustment Request",
]

second_array = [
    "school balance payment location",
    "commencement of summer classes",
    "uniform store location",
    "cafeteria open hours",
    "printing location",
    "room b236 location",
    "application for scholarships",
    "online advising schedule",
    "choosing curriculum",
    "commencement of summer classes",
    "submission of requirements",
    "enrollment process",
]

for first in first_array:
    lowest_result = float('inf')  # Initialize with positive infinity
    lowest_second = ""
    for second in second_array:
        json_payload = {"first": first, "second": second}
        result = send_receive_data(server_ip, server_port, json_payload)
        if result < lowest_result:
            lowest_result = result
            lowest_second = second
        print(f"Received float from server ({first}) ({second}): {result}")
    print(f"Lowest result for '{first}': {lowest_result} (associated with '{lowest_second}')")
    print()
