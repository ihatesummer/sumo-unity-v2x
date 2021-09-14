using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Design Pattern : Singleton 
public class VehicleListCollection
{
    private static VehicleListCollection _instance = null;
    public List<float> idList { get; set; }
    public List<float> oldIdList { get; set; }
    public Dictionary<float, IVehInfo> sumoCarDict { get; set; }
    public Dictionary<float, IVehInfo> msgCarDict { get; set; }

    private VehicleListCollection()
    {
        idList = new List<float>();
        oldIdList = new List<float>();
        sumoCarDict = new Dictionary<float, IVehInfo>();
        msgCarDict = new Dictionary<float, IVehInfo>();
    }
    public static VehicleListCollection Instance()
    {
        if (_instance == null)
        {
            _instance = new VehicleListCollection();
        }
        return _instance;
    }
    public void InitIdList()
    {
        this.idList = new List<float>();
    }

}
