#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using V2XComponent;
using Simulation;

namespace V2XEnvironment {
    public class EnvironmentV2X : GaussianNoise
    {
        public int NumVehicles { get; set; } = 10;

        /*
        public float[,] InterVehDistance(params VehicleV2X[] vehicles)
        {
            int i = 0, j = 0;
            float[,] distance = new float[vehicles.Length, vehicles.Length];
            distance.Initialize();
            foreach (VehicleV2X vehA in vehicles)
            {
                foreach (VehicleV2X vehB in vehicles)
                {
                    if (i == j)
                    {
                        j++;
                        continue;
                    }
                    else if (!Mathf.Approximately(distance[j, i], 0.0f))
                    {
                        distance[i, j] = distance[j, i];
                    }
                    else
                    {
                        distance[i, j] = vehA.CalInterVehDistance(vehB);
                    }
                    j++;
                }
                j = 0;
                i++;
            }
            return distance;
        }
        */

        public float[,] InterVehDistance(float[] posX, float[] posY)
        {
            float[,] distBtwVeh = new float[NumVehicles, NumVehicles];
            distBtwVeh.Initialize();
            for (int vehIdx = 0; vehIdx<NumVehicles; vehIdx++)
            {
                for (int neighborIdx = 0; neighborIdx<NumVehicles; neighborIdx++)
                {
                    if (vehIdx > neighborIdx)
                    {
                        //float noise = (float)Math.Sqrt(dist_err) * (float)NormalDistribution(0, 1);
                        float noise = 0f;
                        distBtwVeh[vehIdx, neighborIdx] = distBtwVeh[neighborIdx, vehIdx] + noise;
                    }
                    else if (vehIdx == neighborIdx)
                    {
                        continue;
                    }
                    else
                    {
                        //float noise = (float)Math.Sqrt(dist_err) * (float)NormalDistribution(0, 1);
                        float noise = (float)Math.Sqrt(dist_err);
                        distBtwVeh[vehIdx, neighborIdx] = (float)Math.Sqrt(Math.Pow(posX[vehIdx] - posX[neighborIdx], 2) + Math.Pow(posY[vehIdx] - posY[neighborIdx], 2)) + noise;
                    }
                }
            }
            return distBtwVeh;
        }

        public float[,] InterVehAngle(float[] posX, float[] posY)
        {
            float[,] angleArrival = new float[NumVehicles, NumVehicles];
            float[,] y = new float[NumVehicles, NumVehicles];
            float[,] x = new float[NumVehicles, NumVehicles];
            y.Initialize();
            x.Initialize();
            angleArrival.Initialize();
            for (int vehIdx=0; vehIdx<NumVehicles; vehIdx++)
            {
                for (int neighborIdx=0; neighborIdx<NumVehicles; neighborIdx++)
                {
                    if (vehIdx == neighborIdx)
                    {
                        continue;
                    }
                    else
                    {
                        //float noise = (float)Math.Sqrt(angle_err) * (float)NormalDistribution(0, 1);
                        float noise = (float)Math.Sqrt(angle_err);
                        y[vehIdx, neighborIdx] = posY[vehIdx] - posY[neighborIdx];
                        x[vehIdx, neighborIdx] = posX[vehIdx] - posX[neighborIdx];
                        if (x[vehIdx, neighborIdx] < 0)
                        {
                            angleArrival[vehIdx, neighborIdx] = (float)Math.Atan(y[vehIdx, neighborIdx] / x[vehIdx, neighborIdx]) + (float)Math.PI + noise;
                        }
                        else
                        {
                            angleArrival[vehIdx, neighborIdx] = (float)Math.Atan(y[vehIdx, neighborIdx] / x[vehIdx, neighborIdx]) + noise;

                        }
                    }

                }
            }
            return angleArrival;
        }

        public void IndividualMeasure(VehicleV2X vehicle, float[] distance, float[] angle, float[] posX, float[] posY, float[] varX, float[] varY, float[] refPosX, float[] refPosY)
        {
                vehicle.UpdateFactorMeasurement(distance, angle,posX, posY, varX, varY, refPosX, refPosY);
        }

        public Tuple<float, float> GetBroadcastPos(VehicleV2X vehicle)
        {
            return new Tuple<float, float>(vehicle.BroadcastPosition().Item1, vehicle.BroadcastPosition().Item2);
        }
        public Tuple<float, float> GetBroadcastVar(VehicleV2X vehicle)
        {
            return new Tuple<float, float>(vehicle.BroadcastVariance().Item1, vehicle.BroadcastVariance().Item2);
        }

    } 
}
#endif
