from typing import List, Tuple
from os.path import join
import re
import traci
from sumo_python_bridge import VehicleBridge
from sumo_python_bridge import TrafficLight


class DataProcessor:
    def __init__(self):
        self._new_cars = []

    @staticmethod
    def preproc_psg_trips(sumo_dir: str) \
            -> None:
        """
        Preprocesses osm.passenger.trips.xml file.
        (Changes trip id into quoted integer format.) 
        """
        file = join(sumo_dir, "osm.passenger.trips.xml")
        with open (file, 'r+' ) as f:
            content = f.read()
            content_new = re.sub(
                'trip id="veh',
                'trip id="',
                content)
            f.seek(0)
            f.write(content_new)

    def update_obj(self,
                   old_veh: List[VehicleBridge],
                   new_veh_ids: List[str],
                   old_tls: List[TrafficLight]) \
            -> Tuple[List[VehicleBridge],
                     List[TrafficLight]]:
        new_veh = self._update_vehicle_obj(old_veh, new_veh_ids)
        new_tls = self._update_tls_obj(old_tls)
        return new_veh, new_tls

    def _update_vehicle_obj(self,
                            old_obj: List[VehicleBridge],
                            new_id_list: List[str]) \
            -> List[VehicleBridge]:
        new_id_list = self._make_unique(new_id_list)
        self._remove_old_car(old_obj, new_id_list)
        self._update_new_car(old_obj, new_id_list)
        new_obj = self._parse_veh(self._new_cars)
        self._new_cars.clear()
        new_obj = new_obj + old_obj
        for vehicle in new_obj:
            vehicle.update_vehicle()
        return new_obj

    @staticmethod
    def _update_tls_obj(old_obj: List[TrafficLight]) \
            -> List[TrafficLight]:
        for tls in old_obj:
            #string
            rgy_state = \
                traci.trafficlight.getRedYellowGreenState(tls.ID)
            for state in rgy_state:
                tls.update_light_state(state)
        new_obj = old_obj
        return new_obj

    @staticmethod
    def _make_unique(id_list: List[str]) -> List[str]:
        return list(set(id_list))

    @staticmethod
    def _remove_old_car(obj_veh: List[VehicleBridge],
                        new_id_list: List[str]) \
            -> None:
        for veh in obj_veh:
            if veh.vehID not in new_id_list:
                obj_veh.remove(veh)

    def _update_new_car(self,
                        old_obj: List[VehicleBridge],
                        new_id_list: List[str]) \
            -> None:
        old_veh_id = [veh.vehID for veh in old_obj]
        for new_id in new_id_list:
            if new_id not in old_veh_id:
                self._new_cars.append(new_id)

    def parse_obj(self,
                  veh_ids: List[str],
                  tls_ids: List[str]) \
        -> Tuple[List[VehicleBridge],
                 List[TrafficLight]]:
        Vehicles = self._parse_veh(veh_ids)
        TrafficLights = self._parse_tls(tls_ids)
        return Vehicles, TrafficLights

    def _parse_veh(self,
                   id_list: List[str]) \
            -> List[VehicleBridge]:
        Vehicles = []
        vehicles_id = self._make_unique(id_list)
        for veh in vehicles_id:
            Vehicles.append(VehicleBridge(veh))
        return Vehicles

    @staticmethod
    def _parse_tls(id_list: List[str]) \
            -> List[TrafficLight]:
        TrafficLights = []
        traffic_lights = id_list
        for tls_id in traffic_lights:
            controlled_lane = \
                traci.trafficlight.getControlledLanes(tls_id)
            for idx, lane in enumerate(controlled_lane):
                lane_shape = traci.lane.getShape(lane)
                #
                #   lane_shape = ((x0, y0), (x1, y1), (x2, y2), (x3, y3))
                #   How to draw traffic light by only using 1 point of 'lane_shape' in Unity?
                #
                pos_x = lane_shape[-1][0]
                pos_y = lane_shape[-1][1]
                obj_TrafficLight = TrafficLight(
                    tls_id, lane, idx, pos_x, pos_y)
                TrafficLights.append(obj_TrafficLight)
        return TrafficLights
