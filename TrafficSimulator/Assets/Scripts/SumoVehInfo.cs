using UnityEngine;

using System;
using System.Globalization;

public class SumoVehInfo : IVehInfo
{
    public string vehObjectType { get; set; } = "SUMO";
    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }
    public float varX { get; set; } = 0f;
    public float varY { get; set; } = 0f;
    public float varZ { get; set; } = 0f;
    public float velX { get; set; } = 0f;
    public float velY { get; set; } = 0f;
    public float velZ { get; set; } = 0f;
    public VehCommonInfo commonInfo { get; set; }

    public SumoVehInfo(string tcpMsg)
    {
        if (HasValidMsgSeperatorInternal(tcpMsg))
        {
            string[] carMsg = tcpMsg.Split(';');
            if (IsValidLength(carMsg))
            {
                // CultureInfo : 날짜 형식이나 시간 형식, 숫자 형식, 문자열의 정렬 등을 설정 국가에 맞게 정의

                // 2020.07.08
                // SUMO coordinate : X & Y
                // Unity coordinate (right hand rule) : X & Y & Z
                posX = (float)Convert.ToDouble(carMsg[1], new CultureInfo("en-US"));
                posY = (float)Convert.ToDouble(carMsg[3], new CultureInfo("en-US"));
                posZ = (float)Convert.ToDouble(carMsg[2], new CultureInfo("en-US"));
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
    private bool IsValidLength(string[] seperatedMsg)
    {
        return seperatedMsg.Length >= 7;
    }
    public Tuple<float, float, float> GetPosition()
    {
        return new Tuple<float, float, float>(this.posX, this.posY, this.posZ);
    }
    public Tuple<float, float, float> GetVariance()
    {
        return new Tuple<float, float, float>(this.posX, this.posY, this.posZ);
    }

}
