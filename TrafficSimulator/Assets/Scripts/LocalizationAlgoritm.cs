using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationAlgoritm 
{
    public LocalizationAlgoritm()
    {
    }
    public void InitMsgPassingVehicle(Dictionary<float, MsgVehMain> msgPassingVehicleDict)
    {
        try
        {
            foreach (MsgVehMain msgPassingVehicle in msgPassingVehicleDict.Values)
            {
                if (msgPassingVehicle.isFirstTime)
                {
                    msgPassingVehicle.InitBelief();
                    msgPassingVehicle.isFirstTime = false;
                }
            }

        }
        catch(NullReferenceException e)
        {
            Debug.Log(e);
        }
        //foreach (MsgVehMain msgPassingVehicle in msgPassingVehicleDict.Values)
        //{
        //    if (msgPassingVehicle.isFirstTime)
        //    {
        //        msgPassingVehicle.InitBelief();
        //        msgPassingVehicle.isFirstTime = false;
        //    }
        //}
    }
    public void UpdateFactorMobility(Dictionary<float, MsgVehMain> msgPassingVehicleDict)
    {
        foreach (MsgVehMain msgPassingVehicle in msgPassingVehicleDict.Values)
        {
            msgPassingVehicle.UpdateFactorMobility();
        }
    }

    public void UpdatePosition(int iteration, Dictionary<float, MsgVehMain> msgPassingVehicleDict)
    {
        for (var i = 0; i < iteration; i++)
        {
            foreach (MsgVehMain msgPassingVehicle in msgPassingVehicleDict.Values)
            {
                msgPassingVehicle.UpdateFactorMeasurement();
            }
            foreach (MsgVehMain msgPassingVehicle in msgPassingVehicleDict.Values)
            {
                msgPassingVehicle.BroadcastPosition();
                msgPassingVehicle.BroadcastVariance();
                //msgPassingVehicle.DeterminePosition();
                //msgPassingVehicle.DetermineVariance();
            }
        }
        //UpdateIndividualPositionVarianceInternal(msgPassingVehicleDict);
    }
    //private void UpdateIndividualPositionVarianceInternal(Dictionary<int, MsgVehMain> msgPassingVehicleDict)
    //{
    //    foreach(MsgVehMain msgPassingVehicle in msgPassingVehicleDict.Values)
    //    {
    //        msgPassingVehicle.DeterminePosition();
    //        msgPassingVehicle.DetermineVariance();
    //    }

    //}
}
