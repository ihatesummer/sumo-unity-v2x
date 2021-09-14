using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Globalization;

public class VehCommonInfo
{
    public float vehId { get; }
    public float speed { get; }
    public float heading { get; }
    public float brakeLight { get; }
    public int sizeCategory { get; }
    public string vehType { get; }

    public bool brakeState { get; }

    public VehCommonInfo(string tcpMsg)
    {
        if (HasValidMsgSeperatorInternal(tcpMsg))
        {
            string[] carMsg = tcpMsg.Split(';');
            if (IsValidLengthInternal(carMsg))
            {
                vehId = (float)Convert.ToDouble(carMsg[0], new CultureInfo("en-US"));
                // CultureInfo : 날짜 형식이나 시간 형식, 숫자 형식, 문자열의 정렬 등을 설정 국가에 맞게 정의
                speed = (float)Convert.ToDouble(carMsg[4], new CultureInfo("en-US"));
                heading = (float)Convert.ToDouble(carMsg[5], new CultureInfo("en-US"));
                brakeLight = (float)Convert.ToDouble(carMsg[6], new CultureInfo("en-US"));
                sizeCategory = (int)Convert.ToDouble(carMsg[7], new CultureInfo("en-US"));
                vehType = carMsg[8];

                if (brakeLight == 1)
                    brakeState = true;
                else
                    brakeState = false;
            }
            else
            {
                Debug.Log("CarInfo : incorrect message length" + tcpMsg);
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

    public string GetCommonInfo()
    {
        string[] strArr = new string[] { vehId.ToString(), speed.ToString(), heading.ToString(), brakeLight.ToString(), sizeCategory.ToString() };
        return String.Join(";", strArr);
    }
}
