using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Design Pattern : Singleton 
public class VehicleScene
{
    public Dictionary<float, GameObject> aliveSumoVeh { get; set; }
    public Dictionary<float, GameObject> aliveMsgVeh { get; set; }

    private static VehicleScene _instance = null;
    public VehicleScene()
    {
        aliveSumoVeh = new Dictionary<float, GameObject>();
        aliveMsgVeh = new Dictionary<float, GameObject>();
    }
    public static VehicleScene Instance()
    {
        if (_instance == null)
        {
            _instance = new VehicleScene();
        }
        return _instance;
    }
}
