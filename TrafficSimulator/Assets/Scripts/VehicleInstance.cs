using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using UnityEngine;

public class VehicleInstance : ITarget, IScene
{
    private VehicleListCollection _vehCollection;
    private VehicleScene _vehInScene;

    [Header("Prefab of vehicle you want to instantiate")]
    public GameObject sumoCarPrefab;
    public GameObject msgCarPrefab;
    public GameObject sumoBusPrefab;
    public GameObject msgBusPrefab;
    public GameObject sumoTrainPrefab;
    public GameObject msgTrainPrefab;
    public int totNumCar;
    //public GameObject sumoInstancePrefab;
    //public GameObject msgInstancePrefab;

    public VehicleInstance(VehiclePrefabs prefab)
    {
        this.sumoCarPrefab = prefab.sumoCar;
        this.msgCarPrefab = prefab.msgCar;
        this.sumoBusPrefab = prefab.sumoBus;
        this.msgBusPrefab = prefab.msgBus;
        this.sumoTrainPrefab = prefab.sumoTrain;
        this.msgTrainPrefab = prefab.msgTrain;
        this.totNumCar = 0;
    }
    public GameObject GetTarget()
    {
        return _vehInScene.aliveSumoVeh[_vehCollection.idList[0]];
    }
    public void ToScene(Dictionary<float, IVehInfo> sumoCarDict, Dictionary<float, IVehInfo> msgCarDict)
    {
        GetVehListCollection();
        GetSceneVehicle();
        //UpdateSumoVehObj(sumoCarDict);
        //UpdateMsgVehObj(msgCarDict);
        UpdateVehicleObjectInternal(sumoCarDict, _vehInScene.aliveSumoVeh);
        UpdateVehicleObjectInternal(msgCarDict, _vehInScene.aliveMsgVeh);
    }
    private void GetVehListCollection()
    {
        _vehCollection = VehicleListCollection.Instance();
    }
    public void GetSceneVehicle()
    {
        _vehInScene = VehicleScene.Instance();
    }
    /*private void UpdateSumoVehObj(Dictionary<int, IVehInfo> sumoCarDict)
    {
        int numCars = _vehCollection.idList.Count;
        UnityEngine.Vector3 objPosition;
        UnityEngine.Quaternion objRotation;
        GameObject vehObject;
        for (var i = 0; i < numCars; i++) {
            if (_vehInScene.aliveSumoVeh.ContainsKey(_vehCollection.idList[i]))
            {
                //_vehInScene.aliveVeh[_vehCollection.idList[i]].transform.position = new UnityEngine.Vector3(_vehCollection.sumoCarDict[_vehCollection.idList[i]].posX, _vehCollection.sumoCarDict[_vehCollection.idList[i]].posZ, _vehCollection.sumoCarDict[_vehCollection.idList[i]].posY);
                //_vehInScene.aliveVeh[_vehCollection.idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0, _vehCollection.sumoCarDict[_vehCollection.idList[i]].commonInfo.heading, 0);
                _vehInScene.aliveSumoVeh[_vehCollection.idList[i]].transform.position = new UnityEngine.Vector3(sumoCarDict[_vehCollection.idList[i]].posX, sumoCarDict[_vehCollection.idList[i]].posY, sumoCarDict[_vehCollection.idList[i]].posZ);
                _vehInScene.aliveSumoVeh[_vehCollection.idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0, sumoCarDict[_vehCollection.idList[i]].commonInfo.heading, 0);
            }
            else
            {
                //objPosition = new UnityEngine.Vector3(_vehCollection.sumoCarDict[_vehCollection.idList[i]].posX, _vehCollection.sumoCarDict[_vehCollection.idList[i]].posZ, _vehCollection.sumoCarDict[_vehCollection.idList[i]].posY);
                //objRotation = UnityEngine.Quaternion.Euler(0, _vehCollection.sumoCarDict[_vehCollection.idList[i]].commonInfo.heading, 0);
                ////objRotation = UnityEngine.Quaternion.Euler(0, sumoCarDict[idList[i]].heading - 90.0f, 0);
                objPosition = new UnityEngine.Vector3(sumoCarDict[_vehCollection.idList[i]].posX, sumoCarDict[_vehCollection.idList[i]].posY, sumoCarDict[_vehCollection.idList[i]].posZ);
                objRotation = UnityEngine.Quaternion.Euler(0, sumoCarDict[_vehCollection.idList[i]].commonInfo.heading, 0);
                //objRotation = UnityEngine.Quaternion.Euler(0, sumoCarDict[idList[i]].heading - 90.0f, 0);

                vehObject = Object.Instantiate(sumoVehiclePrefab, objPosition, objRotation);
                vehObject.name = "SUMO"+_vehCollection.idList[i].ToString();
                _vehInScene.aliveSumoVeh.Add(_vehCollection.idList[i], vehObject);
            }
        }
    }
    */
    /*private void UpdateMsgVehObj(Dictionary<int, IVehInfo> msgCarDict)
    {
        int numCars = _vehCollection.idList.Count;
        UnityEngine.Vector3 objPosition;
        UnityEngine.Quaternion objRotation;
        GameObject vehObject;
        for (var i = 0; i < numCars; i++) {
            if (_vehInScene.aliveMsgVeh.ContainsKey(_vehCollection.idList[i]))
            {
                _vehInScene.aliveMsgVeh[_vehCollection.idList[i]].transform.position = new UnityEngine.Vector3(msgCarDict[_vehCollection.idList[i]].posX, msgCarDict[_vehCollection.idList[i]].posY, msgCarDict[_vehCollection.idList[i]].posZ);
                _vehInScene.aliveMsgVeh[_vehCollection.idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0, msgCarDict[_vehCollection.idList[i]].commonInfo.heading, 0);
            }
            else
            {
                objPosition = new UnityEngine.Vector3(msgCarDict[_vehCollection.idList[i]].posX, msgCarDict[_vehCollection.idList[i]].posY, msgCarDict[_vehCollection.idList[i]].posZ);
                objRotation = UnityEngine.Quaternion.Euler(0, msgCarDict[_vehCollection.idList[i]].commonInfo.heading, 0);
                //objRotation = UnityEngine.Quaternion.Euler(0, sumoCarDict[idList[i]].heading - 90.0f, 0);

                vehObject = Object.Instantiate(msgVehiclePrefab, objPosition, objRotation);
                vehObject.name = "MsgPassing"+_vehCollection.idList[i].ToString();
                _vehInScene.aliveMsgVeh.Add(_vehCollection.idList[i], vehObject);
            }
        }
    }
    */
    private void UpdateVehicleObjectInternal(Dictionary<float, IVehInfo> carDict, Dictionary<float, GameObject> aliveVehObj)
    {
        int numCars = _vehCollection.idList.Count;
        UnityEngine.Vector3 objPosition;
        UnityEngine.Quaternion objRotation;
        GameObject vehInstancePrefab;
        GameObject vehObject;
        for (var i = 0; i < numCars; i++) {
            if (aliveVehObj.ContainsKey(_vehCollection.idList[i]))
            {
                aliveVehObj[_vehCollection.idList[i]].transform.position = new UnityEngine.Vector3( carDict[_vehCollection.idList[i]].posX,
                                                                                                    carDict[_vehCollection.idList[i]].posY,
                                                                                                    carDict[_vehCollection.idList[i]].posZ  );
                //aliveVehObj[_vehCollection.idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0,
                //                                                                             carDict[_vehCollection.idList[i]].commonInfo.heading,
                //                                                                             0);
                float additionalRotation = carDict[_vehCollection.idList[i]].commonInfo.vehType == "pt_bus" ? 90.0f :
                                            carDict[_vehCollection.idList[i]].commonInfo.vehType == "bus_bus" ? 90.0f : 0f;
                aliveVehObj[_vehCollection.idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0,
                                                                                             carDict[_vehCollection.idList[i]].commonInfo.heading + additionalRotation,
                                                                                             0);
            }
            else
            {
                objPosition = new UnityEngine.Vector3(  carDict[_vehCollection.idList[i]].posX,
                                                        carDict[_vehCollection.idList[i]].posY,
                                                        carDict[_vehCollection.idList[i]].posZ  );
                //objRotation = UnityEngine.Quaternion.Euler(0, carDict[_vehCollection.idList[i]].commonInfo.heading, 0);
                float additionalRotation = carDict[_vehCollection.idList[i]].commonInfo.vehType == "pt_bus" ? 90.0f :
                                            carDict[_vehCollection.idList[i]].commonInfo.vehType == "bus_bus" ? 90.0f : 0f;
                objRotation = UnityEngine.Quaternion.Euler(0, carDict[_vehCollection.idList[i]].commonInfo.heading + additionalRotation, 0);
                vehInstancePrefab = GetVehiclePrefabInternal(carDict, i);
                vehObject = Object.Instantiate(vehInstancePrefab, objPosition, objRotation);
                vehObject.name = carDict[_vehCollection.idList[i]].vehObjectType + _vehCollection.idList[i].ToString();
                aliveVehObj.Add(_vehCollection.idList[i], vehObject);
                totNumCar += 1;
                Debug.Log("number of car : " + totNumCar);
            }
        }
    }
    private GameObject GetVehiclePrefabInternal(Dictionary<float, IVehInfo> carDict, int i)
    {
        GameObject prefab;
        switch (carDict[_vehCollection.idList[i]].commonInfo.vehType)
        {
            case "pt_bus":
            case "bus_bus":
                prefab = carDict[_vehCollection.idList[i]].vehObjectType == "SUMO"? this.sumoBusPrefab : this.msgBusPrefab;
                break;
            case "urban_rail_urban":
                prefab = carDict[_vehCollection.idList[i]].vehObjectType == "SUMO"? this.sumoTrainPrefab : this.msgTrainPrefab;
                break;
            default:
                prefab = carDict[_vehCollection.idList[i]].vehObjectType == "SUMO"? this.sumoCarPrefab : this.msgCarPrefab;
                break;
        }

        return prefab;
    }

}
