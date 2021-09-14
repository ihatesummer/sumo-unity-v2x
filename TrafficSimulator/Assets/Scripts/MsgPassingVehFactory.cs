using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class MsgPassingVehFactory
{
    [Header("Collections")]
    private IRequestHandler3D _positionCollection;
    private IRequestHandler3D _varianceCollection;
    private IDistanceAngle _vehGroupDistanceAngle;
    [Header("Message Passing Algorithm")]
    private IVehInfo anchorVeh;
    private MsgVehMain _msgPassingVehicle;
    private Dictionary<float, MsgVehMain> _msgPassingVehicleDict;

    public MsgPassingVehFactory()
    {
        _msgPassingVehicleDict = new Dictionary<float, MsgVehMain>();
    }
    public void SetAnchor(Dictionary<float, IVehInfo> sumoCarDict, float vehID)
    {
        anchorVeh = sumoCarDict[vehID];
    }
    public void SensingNeighborVehicle()
    {
        CollectNeighborPositionInternal();
        CollectNeighborVarianceInternal();
        MeasureDistanceAngleInternal();
    }
    private void CollectNeighborPositionInternal()
    {
        _positionCollection = new VehPosCollection();
        _positionCollection.Collect(VehicleListCollection.Instance().msgCarDict);
    }
    private void CollectNeighborVarianceInternal()
    {
        _varianceCollection = new VehVarianceCollection();
        _varianceCollection.Collect(VehicleListCollection.Instance().msgCarDict);
    }
    private void MeasureDistanceAngleInternal()
    {
        _vehGroupDistanceAngle = new VehGroupDistanceAngle(_positionCollection);
        _vehGroupDistanceAngle.MeasureInterVehDistance();
        _vehGroupDistanceAngle.MeasureInterVehAngle();
    }
    public Dictionary<float, MsgVehMain> GenerateMessagePassingEquippedVehicle()
    {
        _msgPassingVehicleDict.Clear();
        VehicleListCollection vehCollection = VehicleListCollection.Instance();
        Dictionary<float, IVehInfo> msgCarDict = vehCollection.msgCarDict;
        //foreach (int vehID in vehCollection.idList)
        foreach (float vehID in msgCarDict.Keys)
        {
            _msgPassingVehicle = new MsgVehMain(anchorVeh, vehCollection.msgCarDict[vehID], new MsgPassingComponent());
            _msgPassingVehicle.neighborVehPositions = _positionCollection;
            _msgPassingVehicle.neighborVehVariances = _varianceCollection;
            //_msgPassingVehicle.distance = _vehGroupDistanceAngle.GetDistance(vehCollection.idList.IndexOf(vehID));
            //_msgPassingVehicle.angle = _vehGroupDistanceAngle.GetAngle(vehCollection.idList.IndexOf(vehID));
            _msgPassingVehicle.distance = _vehGroupDistanceAngle.GetDistance(new List<float>(vehCollection.msgCarDict.Keys).IndexOf(vehID));
            _msgPassingVehicle.angle = _vehGroupDistanceAngle.GetAngle(new List<float>(vehCollection.msgCarDict.Keys).IndexOf(vehID));
            UpdateMsgPassingEquippedVehicleInternal(vehCollection.msgCarDict[vehID]);
        }
        return _msgPassingVehicleDict;
    }
    private void UpdateMsgPassingEquippedVehicleInternal(IVehInfo MsgPassingEquippedVehicle)
    {
        if (IsNewVehicleInternal(MsgPassingEquippedVehicle))
        {
            _msgPassingVehicleDict.Add(MsgPassingEquippedVehicle.commonInfo.vehId, _msgPassingVehicle);
        }
        else
        {
            _msgPassingVehicleDict[MsgPassingEquippedVehicle.commonInfo.vehId] = _msgPassingVehicle;
        }
    }
    private bool IsNewVehicleInternal(IVehInfo MsgPassingEquippedVehicle)
    {
        return !_msgPassingVehicleDict.ContainsKey(MsgPassingEquippedVehicle.commonInfo.vehId);
    }
}
