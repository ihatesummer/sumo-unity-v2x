import socket
import threading


class TcpClient:
    def __init__(self, ip, port):
        self.ip = ip
        self.port = port
        self.client_socket = None
        self.client_addr = None
        self.server_socket = None

    def boot(self):
        self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    def connect(self):
        self.client_socket.connect((self.ip, self.port))

    def send_msg(self, msg='start'):
        self.client_socket.send(msg.encode())

    def wait_server(self):
        print(f'waiting for Server')
        data = self.client_socket.recv(1024)
        print(f'Received {data.decode()}')

    def receive_msg(self):
        global close
        while not close:
            print(f'waiting for Server')
            data = self.client_socket.recv(1024)
            print(f'Received {data.decode()}')
            if data.decode() == 'finish':
                close = True

    def shut_down(self):
        self.client_socket.close()


if __name__ == '__main__':
    IP = '127.0.0.1'
    PORT = 4042
    close = False
    client = TcpClient(IP, PORT)
    client.boot()
    client.connect()
    client.send_msg()
#    while True:
#        client.wait_server()
#    client.shut_down()
    client_thread = threading.Thread(client.receive_msg(), args=(()))
    client_thread.start()



