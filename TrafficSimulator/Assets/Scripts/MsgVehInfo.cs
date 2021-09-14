using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Globalization;
using Simulation;
using System.Linq;

public class MsgVehInfo : IVehInfo
{
    public string vehObjectType { get; set; } = "MsgPassing";
    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }
    public float varX { get; set; } = 1;
    public float varY { get; set; } = 1;
    public float varZ { get; set; } = 1;
    public float velX { get; set; } = 0;
    public float velY { get; set; } = 0;
    public float velZ { get; set; } = 0;
    public VehCommonInfo commonInfo { get; set; }

    public MsgVehInfo(string tcpMsg)
    {
        // 2020.06.29
        // velocity of x and z axis should be confirmed 
        // whether they are correctly configured.
        //this.velX = commonInfo.speed * (float)Math.Cos(commonInfo.heading);
        //this.velY = 0;
        //this.velZ = commonInfo.speed * (float)Math.Sin(commonInfo.heading);
        if (HasValidMsgSeperatorInternal(tcpMsg))
        {
            string[] carMsg = tcpMsg.Split(';');
            if (IsValidLengthInternal(carMsg))
            {
                // 2020.07.08
                // SUMO coordinate : X & Y
                // Unity coordinate (right hand rule) : X & Y & Z
                this.posX = (float)Convert.ToDouble(carMsg[1], new CultureInfo("en-US"));
                this.posY = (float)Convert.ToDouble(carMsg[3], new CultureInfo("en-US"));
                this.posZ = (float)Convert.ToDouble(carMsg[2], new CultureInfo("en-US"));
            }
            else
            {
                Debug.Log("CarInfo : incorrect message length");
            }
        }
    }
    private bool HasValidMsgSeperatorInternal(string tcpMsg)
    {
        return tcpMsg.Contains(";");
    }
    private bool IsValidLengthInternal(string[] seperatedMsg)
    {
        return seperatedMsg.Length >= 7;
    }
    public Tuple<float, float, float> GetPosition()
    {
        return new Tuple<float, float, float>(this.posX, this.posY, this.posZ);
    }
    public Tuple<float, float, float> GetVariance()
    {
        return new Tuple<float, float, float>(this.varX, this.varY, this.varZ);
    }

}
