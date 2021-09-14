using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePrefabs
{
    [Header("Prefab of vehicle you want to instantiate")]
    public GameObject sumoCar;
    public GameObject msgCar;
    public GameObject sumoBus;
    public GameObject msgBus;
    public GameObject sumoTrain;
    public GameObject msgTrain;
    public VehiclePrefabs(GameObject sumoCarPrefab, GameObject msgCarPrefab, GameObject sumoBusPrefab, GameObject msgBusPrefab, GameObject sumoTrainPrefab, GameObject msgTrainPrefab)
    {
        this.sumoCar = sumoCarPrefab;
        this.msgCar = msgCarPrefab;
        this.sumoBus = sumoBusPrefab;
        this.msgBus = msgBusPrefab;
        this.sumoTrain = sumoTrainPrefab? sumoTrainPrefab : sumoCarPrefab;
        this.msgTrain = msgTrainPrefab? msgTrainPrefab : msgCarPrefab;
    }
}
