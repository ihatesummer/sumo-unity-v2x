using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehPosCollection : IRequestHandler3D
{
    private VehicleListCollection _vehCollection;
    private Dictionary<float, float> _posX;
    private Dictionary<float, float> _posY;
    private Dictionary<float, float> _posZ;
    public VehPosCollection()
    {
        _vehCollection = VehicleListCollection.Instance();
        _posX = new Dictionary<float, float>();
        _posY = new Dictionary<float, float>();
        _posZ = new Dictionary<float, float>();
    }
    public void Collect(Dictionary<float, IVehInfo> carDict)
    {
        //_posX.Clear();
        //_posY.Clear();
        //_posZ.Clear();
        _posX = new Dictionary<float, float>();
        _posY = new Dictionary<float, float>();
        _posZ = new Dictionary<float, float>();
        foreach(float vehID in _vehCollection.idList)
        {
            if (IsNewVehIDInternal(vehID))
            {
                _posX.Add(vehID, carDict[vehID].posX);
                _posY.Add(vehID, carDict[vehID].posY);
                _posZ.Add(vehID, carDict[vehID].posZ);
            }
            else
            {
                _posX[vehID] = carDict[vehID].posX;
                _posY[vehID] = carDict[vehID].posY;
                _posZ[vehID] = carDict[vehID].posZ;
            }
        }
    }
    private bool IsNewVehIDInternal(float vehID)
    {
        return !_posX.ContainsKey(vehID) && !_posY.ContainsKey(vehID) && !_posZ.ContainsKey(vehID);
    }
    public Dictionary<float, float> GetCollectionX() { return _posX; }
    public Dictionary<float, float> GetCollectionY() { return _posY; }
    public Dictionary<float, float> GetCollectionZ() { return _posZ; }
    public void SetCollectionX(float vehID, float posX) { _posX[vehID] = posX; }
    public void SetCollectionY(float vehID, float posY) { _posY[vehID] = posY; }
    public void SetCollectionZ(float vehID, float posZ) { _posZ[vehID] = posZ; }
}
