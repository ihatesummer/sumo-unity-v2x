import socket
import time


class TcpServer:
    def __init__(self, ip: str, port: int):
        self.ip = ip
        self.port = port
        self.server_socket = None
        self.client_socket = None
        self.client_addr = None

    def boot(self) -> socket.socket:
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.bind((self.ip, self.port))
        self.server_socket.listen()
        print(f'waiting for Unity Client')
        self.client_socket, self.client_addr = self.server_socket.accept()
        return self.client_socket

    def wait_unity(self) -> bool:
        print(f'Connected by {self.client_addr}')
        while True:
            data = self.client_socket.recv(1024)
            if data.decode() == 'start':
                self.client_socket.sendall(data)
                return True
            else:
                print(f'waiting for Unity Client')
                time.sleep(0.01)

    def send_msg(self, msg: str = 'hello_unity') -> None:
        time.sleep(0.01)
        self.client_socket.sendall(msg.encode('utf-8'))

    def shut_down(self) -> None:
        self.server_socket.close()
        self.client_socket.close()


if __name__ == '__main__':
    IP = '127.0.0.1'
    PORT = 4042
    server = TcpServer(IP, PORT)
    server.boot()
    server.wait_unity()
    server.send_msg()
    while True:
        server.send_msg(msg='psh')
    server.shut_down()
