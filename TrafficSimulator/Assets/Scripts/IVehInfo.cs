using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public interface IVehInfo 
{
    string vehObjectType { get; set; }
    float posX { get; set; }
    float posY { get; set; }
    float posZ { get; set; }
    float varX { get; set; }
    float varY { get; set; }
    float varZ { get; set; }
    float velX { get; set; }
    float velY { get; set; }
    float velZ { get; set; }
    VehCommonInfo commonInfo { get; set; }
    Tuple<float, float, float> GetPosition();
    Tuple<float, float, float> GetVariance();

}
