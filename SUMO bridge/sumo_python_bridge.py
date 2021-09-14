import math
from typing import List
from typing import Tuple
import traci
from os.path import join

isGUI = False


class SumoEnv:
    def __init__(self, isGUI):
        self.sumo_dir = ("C:\\Program Files (x86)"
                         "\\Eclipse\\Sumo\\bin\\")
        if isGUI:
            self.sumo_binary = self.sumo_dir + \
                "sumo-gui.exe"
        else:
            self.sumo_binary = self.sumo_dir + \
                "sumo.exe"


class SumoClient(SumoEnv):
    def __init__(self, path: str, config_file: str):
        super(SumoClient, self).__init__(isGUI)
        self.path = path
        self.config = config_file
        self.__cmd = [self.sumo_binary,
                      '-c',
                      join(self.path,
                           self.config)]

    def start_sumo(self) -> None:
        traci.start(self.__cmd)
        print("TraCI started.")

    @staticmethod
    def next_step() -> None:
        traci.simulation.step()

    @staticmethod
    def time() -> float:
        time = traci.simulation.getCurrentTime()
        time *= 0.001
        return time

    @staticmethod
    def close() -> None:
        traci.close()

    @staticmethod
    def _get_all_vehicles() -> List[str]:
        return traci.vehicle.getIDList()

    @staticmethod
    def _get_all_traffic_lights() -> List[str]:
        return traci.trafficlight.getIDList()

    def get_all_list(self) -> Tuple[List[str],
                                    List[str]]:
        vehicle_list = self._get_all_vehicles()
        tls_list = self._get_all_traffic_lights()
        return vehicle_list, tls_list


class VehicleBridge:
    def __init__(self, veh_id):
        self.vehID = str(veh_id)
        try:
            self.type = traci.vehicle.getTypeID(self.vehID)
            self.route = traci.vehicle.getRouteID(self.vehID)
            self.edge = traci.vehicle.getRoadID(self.vehID)
            self.length = traci.vehicle.getLength(self.vehID)
            self.width = traci.vehicle.getWidth(self.vehID)
            self.size = self._category_veh()  # Vehicle size category
            if traci.vehicle.getSignals(self.vehID) & 8 == 8:  # Bitmask - 8 for brake light
                self.stBrakePedal = True
            else:
                self.stBrakePedal = False
#            tmp_pos = traci.vehicle.getPosition(self.vehID)  # position: x,y
#            self.pos_x_front_bumper = tmp_pos[0]  # X position (front bumper, meters)
#            self.pos_y_front_bumper = tmp_pos[1]  # Y position (front bumper, meters)
            #self._get_position()
            self.pos_x_front_bumper, self.pos_y_front_bumper, self.pos_z_front_bumper = self._get_position()
            self.velocity = traci.vehicle.getSpeed(self.vehID)
            self.heading = traci.vehicle.getAngle(self.vehID)
            self.pos_x_center, self.pos_y_center = self._get_center_pos()
        except:
            print(f'Error : Fail to find initial information of {self.vehID} vehicle')

    def _get_position(self) -> Tuple[float, float, float]:
        tmp_pos = traci.vehicle.getPosition3D(self.vehID)  # position: x,y,z
        pos_x_front_bumper = tmp_pos[0]  # X position (front bumper, meters)
        pos_y_front_bumper = tmp_pos[1]  # Y position (front bumper, meters)
        pos_z_front_bumper = tmp_pos[2]  # Z position (height, meters)
        return pos_x_front_bumper, pos_y_front_bumper, pos_z_front_bumper

    def _get_center_pos(self) -> Tuple[float, float]:
        self.pos_z_center = self.pos_z_front_bumper
        return self.pos_x_front_bumper - (math.sin(math.radians(self.heading)) * (self.length / 2)), \
               self.pos_y_front_bumper - (math.cos(math.radians(self.heading)) * (self.length / 2))

    #@staticmethod
    #def _get_lng_lat(pos_x, pos_y) -> Tuple[float, float]:
    #    lng, lat = traci.simulationStep().convertGeo(pos_x, pos_y)
    #    return lng, lat

    def _category_veh(self) -> int:
        if self.length < 1:
            return 1            # pedestrian (workaround)
        elif self.length < 4:
            return 11           # Small car
        elif self.length < 5:
            return 12           # Medium car
        else:
            return 13           # Large car

    #@staticmethod
    #def transform_coord(pos_x, pos_y) -> Tuple[float, float]:
    #    pos_lng, pos_lat = traci.simulation.convertGeo(pos_x, pos_y)
    #    return pos_lng, pos_lat

    def update_vehicle(self) -> None:
        try:
            if traci.vehicle.getSignals(self.vehID) & 8 == 8:
                self.stBrakePedal = True
            else:
                self.stBrakePedal = False

            #            tmp_pos = traci.vehicle.getPosition(self.vehID)  # position: x,y
            #            self.pos_x_front_bumper = tmp_pos[0]  # X position (front bumper, meters)
            #            self.pos_y_front_bumper = tmp_pos[1]  # Y position (front bumper, meters)
            self.pos_x_front_bumper, self.pos_y_front_bumper, self.pos_z_front_bumper = self._get_position()
            self.velocity = traci.vehicle.getSpeed(self.vehID)
            self.heading = traci.vehicle.getAngle(self.vehID)
            self.edge = traci.vehicle.getRoadID(self.vehID)
            #            self.__get_center_pos()  # self.PosX_Center, self.PosY_Center (center, meters)
            self.pos_x_center, self.pos_y_center = self._get_center_pos()
        except:
            print(f'Error : Fail to update information of {self.vehID} vehicle')


class TrafficLight:
    def __init__(self, id: str, lane: List[str], idx: int, pos_x: float, pos_y: float):
        self.ID = id
        self.lane = lane
        self.idx = idx
        self.pos_x = pos_x
        self.pos_y = pos_y
        self.lng, self.lat = self._get_lng_lat(self.pos_x, self.pos_y)
        self.current_state = 0
#        self.lane_shape = self.__2d_positions()

#    def __2d_positions(self):
#        lane_shape = {}
#        for lane in self.lane:
#            if lane not in lane_shape:
#                lane_shape[lane] = [traci.lane.getShape(lane)]
#            else:
#                lane_shape[lane].append(traci.lane.getShape(lane))
#        return lane_shape

    @staticmethod
    def _get_lng_lat(pos_x: float, pos_y: float) -> Tuple[float, float]:
        lng, lat = traci.simulation.convertGeo(pos_x, pos_y)
        return lng, lat

    #def dec_light_state(self, rgy_state: str) -> int:
    #    decoder = {
    #        "o": 0,     # off
    #        "O": 0,     # off
    #        "g": 1,     # green
    #        "G": 1,     # green
    #        "y": 2,     # yellow
    #        "Y": 2,     # yellow
    #        "r": 3,     # red
    #        "R": 3      # red
    #    }
    #    self.current_state = decoder[rgy_state]
    #    return self.current_state

    def update_light_state(self, rgy_state: str) -> int:
#        rgy_state = traci.trafficlight.getRedYellowGreenState(self.ID) #string
#        rgy_state_test = traci.trafficlight.getPhaseName(self.ID)
        decoder = {
            "o": 0,     # off
            "O": 0,     # off
            "g": 1,     # green
            "G": 1,     # green
            "y": 2,     # yellow
            "Y": 2,     # yellow
            "r": 3,     # red
            "R": 3      # red
        }
        self.current_state = decoder[rgy_state]
        return self.current_state


#class NetworkBridge:
#    def __init__(self):
#        self.link_shape = []
#
#    def parse_network(self):
#        self.edges = traci.lane.getIDList()
#        for edge in self.edges:
#            self.link_shape.append(traci.lane.getShape(edge))


if __name__ == '__main__':
    pass



