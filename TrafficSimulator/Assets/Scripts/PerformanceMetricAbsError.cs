using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

public class PerformanceMetricAbsError
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

    public PerformanceMetricAbsError()
    {
        _vehListCollection = VehicleListCollection.Instance();
        _sumoVehPositions = new VehPosCollection();
        _sumoVehVariances = new VehVarianceCollection();
        _msgVehPositions = new VehPosCollection();
        _msgVehVariances = new VehVarianceCollection();
    }
    public Tuple<float, float, float> GetAbsoluteError()    // Mean Squared Displacement
    {
        List<float> positionError;
        MeasureDistanceInternal();
        positionError = GetPositionErrorInternal();
        //incErrorAverage = IncrementalAverage(incErrorAverage, numSamples, positionError);

        //Debug.LogError("Average Absolute Error : " + incErrorAverage);

        //return new Tuple<float, float, float>(positionError.Min(), positionError.Max(), incErrorAverage);
        return new Tuple<float, float, float>(positionError.Min(), positionError.Max(), positionError.Average());
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
    private List<float> GetPositionErrorInternal()
    {
        int numVehicle = _vehListCollection.idList.Count;
        Dictionary<float, float> sumoVehPosX = _sumoVehPositions.GetCollectionX();
        Dictionary<float, float> sumoVehPosZ = _sumoVehPositions.GetCollectionZ();
        Dictionary<float, float> msgVehPosX = _msgVehPositions.GetCollectionX();
        Dictionary<float, float> msgVehPosZ = _msgVehPositions.GetCollectionZ();
        List<float> positionError = new List<float>();
        float distance;
        foreach (float vehID in _vehListCollection.idList)
        {
            distance = Mathf.Sqrt(Mathf.Pow(sumoVehPosX[vehID] - msgVehPosX[vehID], 2) + Mathf.Pow(sumoVehPosZ[vehID] - msgVehPosZ[vehID], 2));
            positionError.Add(Mathf.Sqrt(distance));
        }
        return positionError; 
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
