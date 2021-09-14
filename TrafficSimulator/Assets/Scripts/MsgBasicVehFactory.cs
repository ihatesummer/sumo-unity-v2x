using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MsgBasicVehFactory : IVehUpdtAndDel
{
    [Header("Vehicle ID")]
    private VehicleListCollection _vehCollection;

    public MsgBasicVehFactory()
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
            MsgVehInfo msgCar = new MsgVehInfo(perVehicle[i]);
            msgCar.commonInfo = commonInfo;
            UpdateCarDictInternal(msgCar);
        }
    }
    private void GenerateVehObjInternal(string msg)
    {
        VehCommonInfo commonInfo = new VehCommonInfo(msg);
        MsgVehInfo msgCar = new MsgVehInfo(msg);
        msgCar.commonInfo = commonInfo;
        UpdateCarDictInternal(msgCar);
    }
    private void UpdateCarDictInternal(MsgVehInfo msgCar)
    {
        _vehCollection.idList.Add(msgCar.commonInfo.vehId);
        if (IsNewVehIDInternal(msgCar))
        {   
            // new car
            _vehCollection.msgCarDict.Add(msgCar.commonInfo.vehId, msgCar);
        }
        else if(IsValidVehIDInternal(msgCar))
        {   // update an existing car
            _vehCollection.msgCarDict[msgCar.commonInfo.vehId] = msgCar;
        }
        else
        {

        }
    }
    private bool IsNewVehIDInternal(MsgVehInfo msgCar)
    {
        // oldIdList에 car.vehId가 있는지 검사. 
        return !_vehCollection.msgCarDict.ContainsKey(msgCar.commonInfo.vehId);
    }
    private bool IsValidVehIDInternal(MsgVehInfo msgCar)
    {
        return msgCar.commonInfo.vehId != null;
    }
    public void DeleteVehicleInDict()
    {
        _vehCollection.msgCarDict.Clear();
    }
}
