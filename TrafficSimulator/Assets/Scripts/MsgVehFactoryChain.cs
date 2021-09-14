using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class MsgVehFactoryChain : IVehUpdtAndDel
{
    private IVehUpdtAndDel _msgBasicVehFactory;
    private MsgPassingVehFactory _msgPassingEquippedVehFactory;
    private VehicleListCollection _vehCollection;
    public Dictionary<float, MsgVehMain> msgPassingEquippedVehicleDict { get; set; }

    public MsgVehFactoryChain()
    {
        _msgBasicVehFactory = new MsgBasicVehFactory();
        _msgPassingEquippedVehFactory = new MsgPassingVehFactory();
        _vehCollection = VehicleListCollection.Instance();
    }
    public void UpdateVehInfo(string[] msg)
    {
        _msgBasicVehFactory.UpdateVehInfo(msg);
        List<float> vehIDList = _vehCollection.idList;
        float minimumValue = vehIDList.Min();
        _msgPassingEquippedVehFactory.SetAnchor(_vehCollection.sumoCarDict, vehIDList[vehIDList.IndexOf(minimumValue)]);
        _msgPassingEquippedVehFactory.SensingNeighborVehicle();
        this.msgPassingEquippedVehicleDict = _msgPassingEquippedVehFactory.GenerateMessagePassingEquippedVehicle();
    }
    public void UpdateVehInfo(string msg)
    {
        _msgBasicVehFactory.UpdateVehInfo(msg);
        List<float> vehIDList = _vehCollection.idList;
        float minimumValue = vehIDList.Min();
        _msgPassingEquippedVehFactory.SetAnchor(_vehCollection.sumoCarDict, vehIDList[vehIDList.IndexOf(minimumValue)]);
        _msgPassingEquippedVehFactory.SensingNeighborVehicle();
        this.msgPassingEquippedVehicleDict = _msgPassingEquippedVehFactory.GenerateMessagePassingEquippedVehicle();
    }
    public void DeleteVehicleInDict()
    {
        _msgBasicVehFactory.DeleteVehicleInDict();
    }
}
