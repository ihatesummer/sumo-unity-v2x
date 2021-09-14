using System.Collections.Generic;
public interface IRequestHandler3D
{
    void Collect(Dictionary<float, IVehInfo> carDict);
    Dictionary<float, float> GetCollectionX();
    Dictionary<float, float> GetCollectionY();
    Dictionary<float, float> GetCollectionZ();
    void SetCollectionX(float vehID, float value);
    void SetCollectionY(float vehID, float value);
    void SetCollectionZ(float vehID, float value);
}
