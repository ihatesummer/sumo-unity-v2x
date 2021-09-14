using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehVarianceCollection : IRequestHandler3D
{
    private VehicleListCollection _vehCollection;
    private Dictionary<float, float> _varX;
    private Dictionary<float, float> _varY;
    private Dictionary<float, float> _varZ;

    public VehVarianceCollection()
    {
        _vehCollection = VehicleListCollection.Instance();
        _varX = new Dictionary<float, float>();
        _varY = new Dictionary<float, float>();
        _varZ = new Dictionary<float, float>();
    }
    public void Collect(Dictionary<float, IVehInfo> carDict)
    {
        //_varX.Clear();
        //_varY.Clear();
        //_varZ.Clear();
        _varX = new Dictionary<float, float>();
        _varY = new Dictionary<float, float>();
        _varZ = new Dictionary<float, float>();
        foreach(float vehID in _vehCollection.idList)
        {
            if (IsNewVehIDInternal(vehID))
            {
                _varX.Add(vehID, carDict[vehID].varX);
                _varY.Add(vehID, carDict[vehID].varY);
                _varZ.Add(vehID, carDict[vehID].varZ);
            }
            else
            {
                _varX[vehID] = carDict[vehID].varX;
                _varY[vehID] = carDict[vehID].varY;
                _varZ[vehID] = carDict[vehID].varZ;
            }
        }
    }
    private bool IsNewVehIDInternal(float vehID)
    {
        return !_varX.ContainsKey(vehID) && !_varY.ContainsKey(vehID) && !_varZ.ContainsKey(vehID);
    }
    public Dictionary<float, float> GetCollectionX() { return _varX; }
    public Dictionary<float, float> GetCollectionY() { return _varY; }
    public Dictionary<float, float> GetCollectionZ() { return _varZ; }
    public void SetCollectionX(float vehID, float varX) { _varX[vehID] = varX; }
    public void SetCollectionY(float vehID, float varY) { _varY[vehID] = varY; }
    public void SetCollectionZ(float vehID, float varZ) { _varZ[vehID] = varZ; }

}
