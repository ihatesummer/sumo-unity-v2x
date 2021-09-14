import os, sys
import traci
import traci.constants as tc


class SumoENV:
    def __init__(self, is_gui=False):
        self.isGUI = is_gui
        self.sumo_dir = "C:\\Program Files (x86)\\Eclipse\\Sumo\\bin\\"
        if self.isGUI:
            self.sumo_binary = "sumo-gui.exe"
        else:
            self.sumo_binary = "sumo.exe"

    @staticmethod
    def register_sumo_path():
        if "SUMO_HOME" in os.environ:
            tools = os.path.join(os.environ['SUMO_HOME'], 'tools')
            sys.path.append(tools)
        else:
            sys.exit("please declare environment variable 'SUMO_HOME'")


class SumoController(SumoENV):
    def __init__(self, is_gui=False):
        SumoENV.__init__(self, is_gui)
        self.working_dir = "C:\\TrafficSimulator_v2_sumo-master"
        self.config_dir = "2020-03-09-18-06-28"
        self.config_file = "osm.sumocfg"

    def get_launch_cmd(self):
#        sumoCmd = [sumoBinary, '-c', "C:\\Program Files (x86)\\Eclipse\\Sumo\\doc\\examples\\sumo\\simple_nets\\box\\box1l\\test.sumocfg" ]
#        sumoCmd = [sumoBinary, '-c', "C:\\Users\\LICS\\PycharmProjects\\sumo\\sublane_model\\test.sumocfg"]
#        sumoCmd = [sumoBinary, '-c', "C:\\Users\\LICS\\PycharmProjects\\sumo\\2020-03-09-18-06-28\\osm.sumocfg"]
        sumo_cmd = [self.sumo_binary, '-c', f'{self.working_dir}\\{self.config_dir}\\{self.config_file}']
        return sumo_cmd


if __name__ == '__main__':
    sumo_controller = SumoController(is_gui=False)
    launch_cmd = sumo_controller.get_launch_cmd()
    traci.start(launch_cmd)
    print(traci.vehicle.getIDList())
#    traci.junction.subscribeContext("gneJ0", tc.CMD_GET_VEHICLE_VARIABLE, 10, [tc.VAR_SPEED, tc.VAR_WAITING_TIME])
#    print(traci.junction.getContextSubscriptionResults('gneJ0'))
    step = 0
    while step < 1000:
        print(step)
        print(traci.vehicle.getIDList())
        #print(traci.junction.getContextSubscriptionResults('gneJ0'))
        traci.simulationStep()
        step += 1

    traci.close(False)
