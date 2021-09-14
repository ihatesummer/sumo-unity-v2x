using System;
using System.Linq;
using Simulation;

public class VehGroupDistanceAngle : GaussianNoise, IDistanceAngle
{
    private IRequestHandler3D _positions;
    private float[] _posX;
    private float[] _posY;
    private float[] _posZ;
    private int _numVehicle;
    
    public float[,] distance { get; set; }
    public float[,] angleArrival { get; set; }

    public VehGroupDistanceAngle(IRequestHandler3D pos)
    {
        this._positions = pos;
        this._posX = _positions.GetCollectionX().Values.ToArray();
        this._posY = _positions.GetCollectionY().Values.ToArray();
        this._posZ = _positions.GetCollectionZ().Values.ToArray();
        _numVehicle = _positions.GetCollectionX().Count;
        distance = new float[_numVehicle, _numVehicle];
        angleArrival = new float[_numVehicle, _numVehicle];
        distance.Initialize();
        angleArrival.Initialize();
    }
    public void MeasureInterVehDistance()
    {
        for (var vehIdx = 0; vehIdx<_numVehicle; vehIdx++)
        {
            for (var neighborIdx = 0; neighborIdx<_numVehicle; neighborIdx++)
            {
                if (vehIdx > neighborIdx)
                {
                    //float noise = (float)Math.Sqrt(dist_err) * (float)NormalDistribution(0, 1);
                    float noise = 0f;
                    distance[vehIdx, neighborIdx] = distance[neighborIdx, vehIdx] + noise;
                }
                else if (vehIdx == neighborIdx)
                {
                    continue;
                }
                else
                {
                    //float noise = (float)Math.Sqrt(dist_err) * (float)NormalDistribution(0, 1);
                    float noise = (float)Math.Sqrt(dist_err);
                    distance[vehIdx, neighborIdx] = (float)Math.Sqrt(Math.Pow(_posX[vehIdx] - _posX[neighborIdx], 2) + Math.Pow(_posZ[vehIdx] - _posZ[neighborIdx], 2)) + noise;
                }
            }
        }
    }
    public float[] GetDistance(int row)
    {   
        return Enumerable.Range(0, _numVehicle).Select(x => distance[row, x]).ToArray();
    }
    public void MeasureInterVehAngle()
    {
        float[,] xAxis = new float[_numVehicle, _numVehicle];
        float[,] zAxis = new float[_numVehicle, _numVehicle];
        xAxis.Initialize();
        zAxis.Initialize();
        angleArrival.Initialize();
        for (var vehIdx=0; vehIdx<_numVehicle; vehIdx++)
        {
            for (var neighborIdx=0; neighborIdx<_numVehicle; neighborIdx++)
            {
                if (vehIdx == neighborIdx)
                {
                    continue;
                }
                else
                {
                    //float noise = (float)Math.Sqrt(angle_err) * (float)NormalDistribution(0, 1);
                    float noise = (float)Math.Sqrt(angle_err);
                    xAxis[vehIdx, neighborIdx] = _posX[vehIdx] - _posX[neighborIdx];
                    zAxis[vehIdx, neighborIdx] = _posZ[vehIdx] - _posZ[neighborIdx];
                    //if (xAxis[vehIdx, neighborIdx] < 0)
                    //{
                    //    _angleArrival[vehIdx, neighborIdx] = (float)Math.Atan2(zAxis[vehIdx, neighborIdx] , xAxis[vehIdx, neighborIdx]) + (float)Math.PI + noise;
                    //}
                    //else
                    //{
                    //    _angleArrival[vehIdx, neighborIdx] = (float)Math.Atan2(zAxis[vehIdx, neighborIdx] , xAxis[vehIdx, neighborIdx]) + noise;
                    //}
                    angleArrival[vehIdx, neighborIdx] = (float)Math.Atan2(zAxis[vehIdx, neighborIdx] , xAxis[vehIdx, neighborIdx]) + noise;
                }

            }
        }
    }
    public float[] GetAngle(int row)
    {
        return Enumerable.Range(0, _numVehicle).Select(x => angleArrival[row, x]).ToArray();
    }

}
