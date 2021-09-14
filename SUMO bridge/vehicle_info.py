import os, sys
import traci

isGUI = False


class SumoEnv:
    def __init__(self, gui=isGUI):
        self.sumo_dir = "C:\\Program Files (x86)\\Eclipse\\Sumo\\bin\\"
        if gui:
            self.sumo_binary = self.sumo_dir + "sumo-gui.exe"
        else:
            self.sumo_binary = self.sumo_dir + "sumo.exe"


    @staticmethod
    def register_sumo_path():
        if "SUMO_HOME" in os.environ:
            tools = os.path.join(os.environ['SUMO_HOME'], 'tools')
            sys.path.append(tools)
        else:
            sys.exit("please declare environment variable 'SUMO_HOME'")


class SumoLaunch(SumoEnv):
    def __init__(self, dir, config):
        super(SumoLaunch, self).__init__()  # 부모 클래스의 생성자 호출
        self.dir = dir
        self.config = config
        self.cmd = [self.sumo_binary, '-c', f'C:\\TrafficSimulator_v2_sumo-master\\{self.dir}\\{self.config}']

    def get_launch_cmd(self):
        return self.cmd


sumo = SumoLaunch("2020-03-09-18-06-28", "osm.sumocfg")
sumo_cmd = sumo.get_launch_cmd()
traci.start(sumo_cmd)

step = 0
while step < 1000:
    print(step)
    print(traci.vehicle.getIDList())
    traci.simulationStep()
    step += 1

traci.close(False)
