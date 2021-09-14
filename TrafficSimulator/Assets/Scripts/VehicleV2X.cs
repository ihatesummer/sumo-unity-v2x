#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Simulation;


namespace V2XComponent
{
    public class VehicleV2X : GaussianNoise
    {
        private int id;
        private float posX, posY;                           // vpos ( vehicle position )
        private float varX = 1, varY = 1;                   // belvar1 ( variance )
        private float updatedPosX, updatedPosY;
        private float updatedVarX, updatedVarY;
        private const double varux = 0.1f;                  // varux
        private const int varuy = 1;                        // varuy
        private float velX, velY;                           // velocity ( vehicle velocity)
        private float[] estimatePosX = new float[10];
        private float[] estimatePosY = new float[10];
        private float[] accuracyVarX = new float[10];
        private float[] accuracyVarY = new float[10];

        public int WhoIsAnchor { get; set; } = 4;
        public int NumVehicles { get; set; } = 10;
        private const double tDelta = 0.1;
        public VehicleV2X(int id, float posX, float posY, float velX, float velY)
        {
            this.id = id;
            this.posX = posX;
            this.posY = posY;
            this.velX = velX;
            this.velY = velY;
            estimatePosX.Initialize();
            estimatePosY.Initialize();
            accuracyVarX.Initialize();
            accuracyVarY.Initialize();
        }

        public Tuple<float, float> getPosition()
        {
            return new Tuple<float, float>(this.posX, this.posY);
        }
        public Tuple<float, float> getVariance()
        {
            return new Tuple<float, float>(this.varX, this.varY);
        }

        public void InitBelief()
        {
            //float noiseX = (float)Math.Sqrt(dist_err) / 2 * (float)NormalDistribution(0, 1);
            //float noiseY = (float)Math.Sqrt(dist_err) / 2 * (float)NormalDistribution(0, 1);
            float noiseX = (float)Math.Sqrt(dist_err) / 2;
            float noiseY = (float)Math.Sqrt(dist_err) / 2;

            this.posX = this.posX + noiseX;
            this.posY = this.posY + noiseY;
        }

        public void InitBelief(float noiseX, float noiseY)
        {
            this.posX = this.posX + (float)Math.Sqrt(dist_err) / 2 * noiseX;
            this.posY = this.posY + (float)Math.Sqrt(dist_err) / 2 * noiseY;
        }

        public void UpdateFactorMobility(float velX, float velY)
        {
            this.velX = velX;
            this.velY = velY;
            //float noiseX = (float)Math.Sqrt(0.25) * (float)NormalDistribution(0, 1);
            //float noiseY = (float)Math.Sqrt(0.25) * (float)NormalDistribution(0, 1);
            float noiseX = 0f;
            float noiseY = 0f;

            this.posX = this.posX + this.velX * (float)tDelta + noiseX;
            this.posY = this.posY + this.velY * (float)tDelta + noiseY;
            this.varX += (float)varux;
            this.varY += varuy;
        }

        public void UpdateFactorMobility(float velX, float velY, float noiseX, float noiseY)
        {
            this.velX = velX;
            this.velY = velY;

            this.posX = this.posX + this.velX * (float)tDelta + (float)Math.Sqrt(0.25) * noiseX;
            this.posY = this.posY + this.velY * (float)tDelta + (float)Math.Sqrt(0.25) * noiseY;

        }

        public void UpdateFactorMeasurement(float[] distance, float[] angle, float[] posX, float[] posY, float[] varX, float[] varY, float[] refPosX, float[] refPosY)
        {
                UpdateMeasurement(distance, angle,posX, posY, varX, varY, refPosX, refPosY);
        }

        public void UpdateMeasurement(float[] distance, float[] angle, float[] posX, float[] posY, float[] varX, float[] varY, float[] refPosX, float[] refPosY)
        {
            // agent vehicle
            float distVarX, distVarY;
            float sumPosX = 0, sumPosY = 0;
            float sumVarX = 0, sumVarY = 0;
            //float updatedVarX, updatedVarY;
            for (int i=0; i<distance.Length; i++)
            {
                if (i != this.id)
                {
                    distVarX = (float)(dist_err * Math.Pow(Math.Cos(angle[i]), 2) + Math.Pow(distance[i], 2) * angle_err * Math.Pow(Math.Sin(angle[i]), 2));
                    distVarY = (float)(Math.Pow(distance[i], 2) * angle_err * Math.Pow(Math.Cos(angle[i]), 2) + dist_err * Math.Pow(Math.Sin(angle[i]), 2));
                    if (i == WhoIsAnchor)
                    {
                        estimatePosX[i] = (float)(distance[i] * Math.Cos(angle[i]) + refPosX[i]);
                        estimatePosY[i] = (float)(distance[i] * Math.Sin(angle[i]) + refPosY[i]);
                    }
                    else
                    {
                        estimatePosX[i] = (float)(distance[i] * Math.Cos(angle[i]) + posX[i]);
                        estimatePosY[i] = (float)(distance[i] * Math.Sin(angle[i]) + posY[i]);
                    }
                    accuracyVarX[i] = varX[i] + distVarX;
                    accuracyVarY[i] = varY[i] + distVarY;
                }

            }
            for (int i=0; i<NumVehicles; i++)
            {
                if (i != this.id)
                {
                    sumVarX += 1 / accuracyVarX[i];
                    sumVarY += 1 / accuracyVarY[i];
                    sumPosX += estimatePosX[i] / accuracyVarX[i];
                    sumPosY += estimatePosY[i] / accuracyVarY[i];
                }
            }
            this.updatedVarX = 1 / ( 1/this.varX + sumVarX);
            this.updatedVarY = 1 / ( 1/this.varY + sumVarY);
            this.updatedPosX = this.updatedVarX * (this.posX / this.varX + sumPosX);
            this.updatedPosY = this.updatedVarY * (this.posY / this.varY + sumPosY);
            //this.posX = this.updatedVarX * (posX[this.id] / this.varX + sumPosX);
            //this.posY = this.updatedVarY * (posY[this.id] / this.varY + sumPosY);
            //this.varX = updatedVarX;
            //this.varY = updatedVarY;
        }
        public Tuple<float, float> BroadcastPosition()
        {
            //return new Tuple<float, float>(this.posX, this.posY);
            return new Tuple<float, float>(this.updatedPosX, this.updatedPosY);
        }
        public Tuple<float, float> BroadcastVariance()
        {
            //return new Tuple<float, float>(this.varX, this.varY);
            return new Tuple<float, float>(this.updatedVarX, this.updatedVarY);
        }

        //public float CalInterVehDistance(VehicleV2X foreignVeh)
        //{
        //    Tuple<float, float> posForeignVeh = foreignVeh.getPosition();
        //    float distance = (float)Math.Sqrt(Math.Pow(this.posX - posForeignVeh.Item1, 2) + Math.Pow(this.posY - posForeignVeh.Item2, 2));
        //    return distance;
        //}

    }
}
#endif
