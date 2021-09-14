using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using Simulation;

public class MsgPassingComponent : GaussianNoise
{
    private List<float> _estimatePosX { get; set; }
    private List<float> _estimatePosY { get; set; }
    private List<float> _estimatePosZ { get; set; }
    private List<float> _accuracyVarX { get; set; }
    private List<float> _accuracyVarY { get; set; }
    private List<float> _accuracyVarZ { get; set; }
    public int WhoIsAnchor { get; set; }
    public float updatedPosX {get; set;}
    public float updatedPosY {get; set;}
    public float updatedPosZ {get; set;}
    public float updatedVarX {get; set;}
    public float updatedVarY {get; set;}
    public float updatedVarZ {get; set;}

    public float[] distance { get; set; }
    public float[] angle { get; set; }
    public MsgPassingComponent()
    {
        _estimatePosX = new List<float>();
        _estimatePosY = new List<float>();
        _estimatePosZ = new List<float>();
        _accuracyVarX = new List<float>();
        _accuracyVarY = new List<float>();
        _accuracyVarZ = new List<float>();
        updatedPosX = 0f;
        updatedPosY = 0f;
        updatedPosZ = 0f;
        updatedVarX = 0f;
        updatedVarY = 0f;
        updatedVarZ = 0f;
    }

    public void InitBelief(IVehInfo MsgVeh)
    {
        //float noiseX = (float)Math.Sqrt(dist_err) / 2 * (float)NormalDistribution(0, 1);
        //float noiseY = (float)Math.Sqrt(dist_err) / 2 * (float)NormalDistribution(0, 1);
        //float noiseZ = (float)Math.Sqrt(dist_err) / 2 * (float)NormalDistribution(0, 1);
        float noiseX = (float)Math.Sqrt(dist_err) / 2;
        float noiseY = (float)Math.Sqrt(dist_err) / 2;
        float noiseZ = (float)Math.Sqrt(dist_err) / 2;

        MsgVeh.posX = MsgVeh.posX + noiseX;
        MsgVeh.posZ = MsgVeh.posZ + noiseZ;
    }
    public void UpdateFactorMobility(IVehInfo MsgVeh, float velX, float velY, float velZ)
    {
        MsgVeh.velX = velX;
        MsgVeh.velY = velY;
        MsgVeh.velZ = velZ;
        //float noiseX = (float)Math.Sqrt(0.25) * (float)NormalDistribution(0, 1);
        //float noiseY = (float)Math.Sqrt(0.25) * (float)NormalDistribution(0, 1);
        //float noiseZ = (float)Math.Sqrt(0.25) * (float)NormalDistribution(0, 1);
        float noiseX = 0f;
        float noiseY = 0f;
        float noiseZ = 0f;

        MsgVeh.posX = MsgVeh.posX + MsgVeh.velX * MsgVehConstantInfo.tDelta + noiseX;
        MsgVeh.posZ = MsgVeh.posZ + MsgVeh.velZ * MsgVehConstantInfo.tDelta + noiseZ;
        MsgVeh.varX += MsgVehConstantInfo.varux;
        MsgVeh.varY += MsgVehConstantInfo.varuy;
        MsgVeh.varZ += MsgVehConstantInfo.varuz;
    }
    public void UpdateFactorMeasurement(IVehInfo msgVeh, IVehInfo anchorVeh, IRequestHandler3D positions, IRequestHandler3D variances)
    {
        float[] posX = positions.GetCollectionX().Values.ToArray();
        float[] posY = positions.GetCollectionY().Values.ToArray();
        float[] posZ = positions.GetCollectionZ().Values.ToArray();
        float[] varX = variances.GetCollectionX().Values.ToArray();
        float[] varY = variances.GetCollectionY().Values.ToArray();
        float[] varZ = variances.GetCollectionZ().Values.ToArray();
        float refPosX = anchorVeh.posX;
        float refPosY = anchorVeh.posY;
        float refPosZ = anchorVeh.posZ;
        //Debug.Log("Anchor Vehicle ID = "+anchorVeh.commonInfo.vehId.ToString()+" Msg Vehicle ID = "+msgVeh.commonInfo.vehId.ToString());
        UpdateEstimatePositionInternal(msgVeh, posX, posY, posZ, refPosX, refPosY, refPosZ);
        UpdateAccuracyVarianceInternal(msgVeh, varX, varY, varZ);
        UpdatePositionVarianceInternal(msgVeh);
    }
    private void UpdateEstimatePositionInternal(IVehInfo MsgVeh, float[] posX, float[] posY, float[] posZ, float refPosX, float refPosY, float refPosZ)
    {
        //int numVeh = distance.Length;
        int numVeh = posX.Length;
        for (int i=0; i<numVeh; i++)
        {
            //if (i != MsgVeh.commonInfo.vehId)
            //{
                if (i == WhoIsAnchor)
                {
                    _estimatePosX.Add((float)(this.distance[i] * Math.Cos(this.angle[i]) + refPosX));
                    _estimatePosZ.Add((float)(this.distance[i] * Math.Sin(this.angle[i]) + refPosZ));
                }
                else
                {
                    _estimatePosX.Add((float)(this.distance[i] * Math.Cos(this.angle[i]) + posX[i]));
                    _estimatePosZ.Add((float)(this.distance[i] * Math.Sin(this.angle[i]) + posZ[i]));
                }
            //}
        }
    }
    private void UpdateAccuracyVarianceInternal(IVehInfo MsgVeh, float[] varX, float[] varY, float[] varZ)
    {
        float distVarX, distVarZ;
        //int numVeh = this.distance.Length;
        int numVeh = varX.Length;
        for (int i=0; i<numVeh; i++)
        {
            //if (i != MsgVeh.commonInfo.vehId)
            //{
                distVarX = (float)(Math.Pow(this.distance[i], 2) * angle_err * Math.Pow(Math.Sin(this.angle[i]), 2) + dist_err * Math.Pow(Math.Cos(this.angle[i]), 2));
                distVarZ = (float)(Math.Pow(this.distance[i], 2) * angle_err * Math.Pow(Math.Cos(this.angle[i]), 2) + dist_err * Math.Pow(Math.Sin(this.angle[i]), 2));
                _accuracyVarX.Add(varX[i] + distVarX);
                _accuracyVarZ.Add(varZ[i] + distVarZ);
            //}
        }
    }
    private void UpdatePositionVarianceInternal(IVehInfo MsgVeh)
    {
        float sumPosX = 0, sumPosY = 0, sumPosZ = 0;
        float sumVarX = 0, sumVarY = 0, sumVarZ = 0;
        //int numVeh = this._estimatePosX.Count;
        int numVeh = this._accuracyVarX.Count;
        for (int i=0; i<numVeh; i++)
        {
            //if( i != MsgVeh.commonInfo.vehId)
            //{
                sumVarX += 1 / this._accuracyVarX[i];
                sumVarZ += 1 / this._accuracyVarZ[i];
                sumPosX += this._estimatePosX[i] / this._accuracyVarX[i];
                sumPosZ += this._estimatePosZ[i] / this._accuracyVarZ[i];
            //}
        }
        this.updatedVarX = 1 / ( 1/MsgVeh.varX + sumVarX);
        this.updatedVarZ = 1 / ( 1/MsgVeh.varZ + sumVarZ);
        this.updatedPosX = this.updatedVarX * (MsgVeh.posX / MsgVeh.varX + sumPosX);
        this.updatedPosZ = this.updatedVarZ * (MsgVeh.posZ / MsgVeh.varZ + sumPosZ);
    }
    public Tuple<float, float, float> BroadcastPosition()
    {
        InitEstimatePositionInternal();
        return new Tuple<float, float, float>(this.updatedPosX, this.updatedPosY, this.updatedPosZ);
    }
    public Tuple<float, float, float> BroadcastVariance()
    {
        InitAccuracyVarInternal();
        return new Tuple<float, float, float>(this.updatedVarX, this.updatedVarY, this.updatedVarZ);
    }
    private void InitEstimatePositionInternal()
    {
        this._estimatePosX.Clear();
        this._estimatePosY.Clear();
        this._estimatePosZ.Clear();
    }
    private void InitAccuracyVarInternal()
    {
        this._accuracyVarX.Clear();
        this._accuracyVarY.Clear();
        this._accuracyVarZ.Clear();
    }
}
