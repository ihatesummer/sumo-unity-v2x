import os, sys
import traci
import traci.constants as tc


def register_sumo_path():
    if "SUMO_HOME" in os.environ:
        tools = os.path.join(os.environ['SUMO_HOME'], 'tools')
        sys.path.append(tools)
    else:
        sys.exit("please declare environment variable 'SUMO_HOME'")


#sumoGUI = "C:\\Program Files (x86)\\Eclipse\\Sumo\\bin\\sumo-gui.exe"
sumoBinary = "C:\\Program Files (x86)\\Eclipse\\Sumo\\bin\\sumo.exe"
#sumoCmd = [sumoBinary, '-c', "C:\\Program Files (x86)\\Eclipse\\Sumo\\doc\\examples\\sumo\\simple_nets\\box\\box1l\\test.sumocfg" ]
#sumoCmd = [sumoBinary, '-c', "C:\\Users\\LICS\\PycharmProjects\\sumo\\sublane_model\\test.sumocfg"]
sumoCmd = [sumoBinary, '-c', "C:\\Users\\LICS\\PycharmProjects\\sumo\\2020-03-09-18-06-28\\osm.sumocfg"]

traci.start(sumoCmd)
print(traci.vehicle.getIDList())
#traci.junction.subscribeContext("gneJ0", tc.CMD_GET_VEHICLE_VARIABLE, 10, [tc.VAR_SPEED, tc.VAR_WAITING_TIME])
#print(traci.junction.getContextSubscriptionResults('gneJ0'))
step = 0
while step < 1000:
    print(step)
    print(traci.vehicle.getIDList())
    #print(traci.junction.getContextSubscriptionResults('gneJ0'))
    traci.simulationStep()
    step += 1

traci.close(False)
