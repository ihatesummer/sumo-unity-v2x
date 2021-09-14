using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumoVehFactory : IVehUpdtAndDel
{
    [Header("Vehicle ID")]
    private VehicleListCollection _vehCollection;

    public SumoVehFactory()
    {
        _vehCollection = VehicleListCollection.Instance();
    }
    public void UpdateVehInfo(string[] msg)
    {
        string[] receivedMsg = msg;
        GenerateVehObjInternal(receivedMsg);
    }
    public void UpdateVehInfo(string msg)
    {
        string receivedMsg = msg;
        GenerateVehObjInternal(receivedMsg);
    }
    private void GenerateVehObjInternal(string[] perVehicle)
    {
        for (var i=0; i<perVehicle.Length; i++)
        {
            VehCommonInfo commonInfo = new VehCommonInfo(perVehicle[i]);
            IVehInfo sumoCar = new SumoVehInfo(perVehicle[i]);
            sumoCar.commonInfo = commonInfo;
            UpdateCarDictInternal(sumoCar);
        }
    }
    private void GenerateVehObjInternal(string msg)
    {
        VehCommonInfo commonInfo = new VehCommonInfo(msg);
        IVehInfo sumoCar = new SumoVehInfo(msg);
        sumoCar.commonInfo = commonInfo;
        UpdateCarDictInternal(sumoCar);
    }

    private void UpdateCarDictInternal(IVehInfo sumoCar)
    {
        _vehCollection.idList.Add(sumoCar.commonInfo.vehId);
        if (IsNewVehIDInternal(sumoCar))
        {   
            // new car
            _vehCollection.sumoCarDict.Add(sumoCar.commonInfo.vehId, sumoCar);
        }
        else if(IsValidVehIDInternal(sumoCar))
        {   // update an existing car
            _vehCollection.sumoCarDict[sumoCar.commonInfo.vehId] = sumoCar;
        }
        else
        {

        }
    }
    private bool IsNewVehIDInternal(IVehInfo sumoCar)
    {
        // oldIdList에 car.vehId가 있는지 검사. 
        return !_vehCollection.sumoCarDict.ContainsKey(sumoCar.commonInfo.vehId);
    }
    private bool IsValidVehIDInternal(IVehInfo sumoCar)
    {
        return sumoCar.commonInfo.vehId != null;
    }
    public void DeleteVehicleInDict()
    {
        _vehCollection.sumoCarDict.Clear();
    }
}
