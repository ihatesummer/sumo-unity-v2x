using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScene 
{
    void ToScene(Dictionary<float, IVehInfo> sumoCarDict, Dictionary<float, IVehInfo> msgCarDict);
    void GetSceneVehicle();
}
