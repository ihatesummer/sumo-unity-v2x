using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDistanceAngle
{
    float[,] distance { get; set; }
    float[,] angleArrival { get; set; }
    void MeasureInterVehDistance();
    float[] GetDistance(int row);
    void MeasureInterVehAngle();
    float[] GetAngle(int row);
}
