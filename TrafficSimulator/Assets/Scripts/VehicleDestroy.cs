using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class VehicleDestroy : IScene
{
    private VehicleScene _vehInScene;
    private VehicleListCollection _vehCollection;
    public VehicleDestroy()
    {
    }
    public void ToScene(Dictionary<float, IVehInfo> sumoCarDict, Dictionary<float, IVehInfo> msgCarDict)
    {
        GetVehListCollectionInternal();
        GetSceneVehicle();
        DestroyVehicleInSceneInternal(_vehInScene.aliveSumoVeh);
        DestroyVehicleInSceneInternal(_vehInScene.aliveMsgVeh);
    }
    private void GetVehListCollectionInternal()
    {
        _vehCollection = VehicleListCollection.Instance();
    }
    public void GetSceneVehicle()
    {
        _vehInScene = VehicleScene.Instance();
    }
    /*private void DestroySumoVehInSceneInternal()
    {
        int numCars = _vehCollection.idList.Count;
        if (numCars < _vehInScene.aliveSumoVeh.Count)
        {
            foreach (int id in _vehInScene.aliveSumoVeh.Keys)
            {
                if (!_vehCollection.idList.Contains(id))
                {
                    Object.Destroy(_vehInScene.aliveSumoVeh[id]);
                }
            }
        }
    }*/
    /*private void DestroyMsgVehInSceneInternal()
    {
        int numCars = _vehCollection.idList.Count;
        if (numCars < _vehInScene.aliveMsgVeh.Count)
        {
            foreach (int id in _vehInScene.aliveMsgVeh.Keys)
            {
                if (!_vehCollection.idList.Contains(id))
                {
                    Object.Destroy(_vehInScene.aliveMsgVeh[id]);
                }
            }
        }
    }
    */
    private void DestroyVehicleInSceneInternal(Dictionary<float, GameObject> aliveVehObj)
    {
        List<float> willBeDeleted = new List<float>();
        foreach (float id in aliveVehObj.Keys)
        {
            if (!_vehCollection.idList.Contains(id))
            {
                Object.Destroy(aliveVehObj[id]);
                willBeDeleted.Add(id);
            }
        }
        foreach (float id in willBeDeleted)
        {
            aliveVehObj.Remove(id);
        }
    }
}
