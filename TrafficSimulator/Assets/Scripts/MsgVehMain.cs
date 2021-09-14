using System;

public class MsgVehMain
{
    private IVehInfo _AnchorVehicle;
    private MsgPassingComponent _MsgPassing;
    public IRequestHandler3D neighborVehPositions { get; set; }
    public IRequestHandler3D neighborVehVariances { get; set; }
    public IVehInfo msgVehicle { get; }
    public float[] distance { get; set; }
    public float[] angle { get; set; }
    public bool isFirstTime { get; set; } = true;
    public MsgVehMain(IVehInfo anchor, IVehInfo veh, MsgPassingComponent algorithm)
    {
        _AnchorVehicle = anchor;
        msgVehicle = veh;
        _MsgPassing = algorithm;
        //_MsgPassing.WhoIsAnchor = _AnchorVehicle.commonInfo.vehId;
        VehicleListCollection vehicleListCollection = VehicleListCollection.Instance();
        _MsgPassing.WhoIsAnchor = vehicleListCollection.idList.IndexOf(_AnchorVehicle.commonInfo.vehId);
    }
    public void InitBelief()
    {
        _MsgPassing.InitBelief(msgVehicle);
    }
    public void UpdateFactorMobility()
    {
        float velX = msgVehicle.commonInfo.speed * (float)Math.Cos(msgVehicle.commonInfo.heading);
        float velZ = msgVehicle.commonInfo.speed * (float)Math.Sin(msgVehicle.commonInfo.heading);
        _MsgPassing.UpdateFactorMobility(msgVehicle, 0f, 0f, 0f);
    }
    public void UpdateFactorMeasurement()
    {
        _MsgPassing.distance = distance;
        _MsgPassing.angle = angle;
        _MsgPassing.UpdateFactorMeasurement(msgVehicle, _AnchorVehicle, neighborVehPositions, neighborVehVariances);
    }
    public void BroadcastPosition()
    {
        BroadcastPositionInternal();
        DeterminePositionInternal();
    }
    private void BroadcastPositionInternal()
    {
        Tuple<float, float, float> position = _MsgPassing.BroadcastPosition();
        neighborVehPositions.SetCollectionX(msgVehicle.commonInfo.vehId, position.Item1);
        neighborVehPositions.SetCollectionY(msgVehicle.commonInfo.vehId, position.Item2);
        neighborVehPositions.SetCollectionZ(msgVehicle.commonInfo.vehId, position.Item3);
    }
    private void DeterminePositionInternal()
    {
        msgVehicle.posX = _MsgPassing.updatedPosX;
        msgVehicle.posY = _MsgPassing.updatedPosY;
        msgVehicle.posZ = _MsgPassing.updatedPosZ;
    }
    public void BroadcastVariance()
    {
        BroadcastVarianceInternal();
        DetermineVarianceInternal();
    }
    private void BroadcastVarianceInternal()
    {
        Tuple<float, float, float> variance = _MsgPassing.BroadcastVariance();
        neighborVehVariances.SetCollectionX(msgVehicle.commonInfo.vehId, variance.Item1);
        neighborVehVariances.SetCollectionY(msgVehicle.commonInfo.vehId, variance.Item2);
        neighborVehVariances.SetCollectionZ(msgVehicle.commonInfo.vehId, variance.Item3);
    }
    private void DetermineVarianceInternal()
    {
        msgVehicle.varX = _MsgPassing.updatedVarX;
        msgVehicle.varY = _MsgPassing.updatedVarY;
        msgVehicle.varZ = _MsgPassing.updatedVarZ;
    }

}
