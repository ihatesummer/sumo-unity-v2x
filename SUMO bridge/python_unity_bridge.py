from queue import Queue
import threading
import time
import socket
from typing import List
from sumo_python_bridge import VehicleBridge
from sumo_python_bridge import TrafficLight


class UnityBridge:
    def __init__(self):
        self._is_running = False
        self._unity_event = None
        self._queue = Queue(maxsize=1)
        self._unity_client_socket = None
        self.unity_thread = None
        self.error_val_msg = ''
        self.recv_queue = Queue()

    def start_unity(self, unity_client: socket.socket) -> None:
        self._is_running = True
        self._unity_event = threading.Event()
        # Thread parameter 'args' is the argument tuple.
        # make sure to pass tuple type.
        thread_arg = (unity_client, )
        self.unity_thread = threading.Thread(target=self._send_msg, args=thread_arg)
        self.unity_thread.start()
        self._unity_client_socket = unity_client

    def _send_msg(self, client_socket: socket.socket) -> None:
        while self._is_running:
            if self._queue.empty():
                time.sleep(0.2)
            else:
                msg = self._queue.get()
                print(msg)
                try:
                    client_socket.send(msg.encode())
                except socket.error as e:
                    print(e)
                    self._unity_event.set()
                    break

    def construct_msg(self, vehicles: List[VehicleBridge], traffic_lights: List[TrafficLight]) -> None:
        msg = "O1G"
        if len(vehicles) == 1:
            vehicles *= 2
        for veh in vehicles:
            msg += f'{veh.vehID};{veh.pos_x_center:.3f};{veh.pos_y_center:.3f};' \
                   f'{veh.pos_z_center:.3f};'\
                   f'{veh.velocity:.2f};{veh.heading:.2f};' \
                   f'{int(veh.stBrakePedal)};{veh.size};{veh.type}@'
#            msg += f'{veh.vehID};{veh.pos_x_center:.3f};{veh.pos_y_center:.3f};' \
#                   f'{veh.velocity:.2f};{veh.heading:.2f};' \
#                   f'{int(veh.stBrakePedal)};{veh.size}@'

        for tls in traffic_lights:
            break

        msg = msg + "&\n"
        with self._queue.mutex:
            self._queue.queue.clear()
        self._queue.put(msg)

    def recv_msg(self) -> None:
        data = self._unity_client_socket.recv(1024)
        self.error_val_msg = data.decode()
        if not self.error_val_msg:
            return False
        elif self.error_val_msg == "start":
            self.recv_queue.put("absolute error (min) 0 absolute error (max) 0 absolute error (average) 0 "
                                "relative error (min) 0 relative error (max) 0 relative error (average) 0")
        else:
            #print(f'{self.error_val_msg}')
            self.recv_queue.put(self.error_val_msg)

    @property
    def is_running(self) -> bool:
        return self._is_running

    @is_running.setter
    def is_running(self, bool_val: bool):
        self._is_running = bool_val




