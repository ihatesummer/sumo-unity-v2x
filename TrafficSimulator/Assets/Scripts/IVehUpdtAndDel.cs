using System.Collections.Generic;
public interface IVehUpdtAndDel
{
    //Dictionary<string, SumoVehInfo> UpdateVehInfo(string[] msg);
    //Dictionary<string, SumoVehInfo> UpdateVehInfo(string msg);
    void UpdateVehInfo(string[] msg);
    void UpdateVehInfo(string msg);
    void DeleteVehicleInDict();
}
