using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
public class PerformanceMetricRelError
{
    private VehicleListCollection _vehListCollection;
    private IRequestHandler3D _sumoVehPositions;
    private IRequestHandler3D _sumoVehVariances;
    private IRequestHandler3D _msgVehPositions;
    private IRequestHandler3D _msgVehVariances;
    private IDistanceAngle _sumoVehDistanceAngle;
    private IDistanceAngle _msgVehDistanceAngle;

    private float incErrorAverage = 0;
    private int numSamples = 1;

    public PerformanceMetricRelError()
    {
        _vehListCollection = VehicleListCollection.Instance();
        _sumoVehPositions = new VehPosCollection();
        _sumoVehVariances = new VehVarianceCollection();
        _msgVehPositions = new VehPosCollection();
        _msgVehVariances = new VehVarianceCollection();
    }
    public Tuple<float, float, float> GetRelativeError()
    {
        List<float> distanceError;
        MeasureDistanceInternal();
        distanceError = GetDistanceErrorInternal();
        //incErrorAverage = IncrementalAverage(incErrorAverage, numSamples, distanceError);
        //Debug.LogError("Average Relative Error : " + incErrorAverage);

        //return new Tuple<float, float, float>(distanceError.Min(), distanceError.Max(), incErrorAverage);
        return new Tuple<float, float, float>(distanceError.Min(), distanceError.Max(), distanceError.Average());
    }
    private void MeasureDistanceInternal()
    {
        CollectVehiclePositionInternal();
        _sumoVehDistanceAngle = new VehGroupDistanceAngle(_sumoVehPositions);
        _msgVehDistanceAngle = new VehGroupDistanceAngle(_msgVehPositions);
        _sumoVehDistanceAngle.MeasureInterVehDistance();
        _msgVehDistanceAngle.MeasureInterVehDistance();
    }
    private void CollectVehiclePositionInternal()
    {
        _sumoVehPositions.Collect(_vehListCollection.sumoCarDict);
        _msgVehPositions.Collect(_vehListCollection.msgCarDict);
    }
    private List<float> GetDistanceErrorInternal()
    {
        int numVehicle = _vehListCollection.sumoCarDict.Count;
        List<float> distanceError = new List<float>();
        float[,] sumoVehDistance = _sumoVehDistanceAngle.distance;
        float[,] msgVehdistance = _msgVehDistanceAngle.distance;
        for (int vehIdx=0; vehIdx<numVehicle; vehIdx++)
        {
            for (int neighborIdx=0; neighborIdx<numVehicle; neighborIdx++)
            {
                distanceError.Add(Mathf.Abs(sumoVehDistance[vehIdx, neighborIdx] - msgVehdistance[vehIdx, neighborIdx]));
            }
        }

        return distanceError;
    }
    private float IncrementalAverage(float prevAverage, int numSamples, List<float> error)
    {
        float incAverage = prevAverage;
        foreach (float elem in error)
        {
            incAverage = incAverage + (elem - incAverage) / numSamples;
            numSamples++;
        }
        return incAverage;
    }
}
