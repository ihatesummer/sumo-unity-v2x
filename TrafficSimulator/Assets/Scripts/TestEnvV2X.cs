#if false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using V2XComponent;
using V2XEnvironment;

public class TestEnvV2X : MonoBehaviour
{
    public  const int NumVehicles = 10;
    private const int NumIteration = 10;

    private const double ROADWIDTH = 3.5;
    private const float posXVeh1 = 1 * (float)ROADWIDTH, posXVeh2 = 1 * (float)ROADWIDTH;
    private const float posXVeh3 = 2 * (float)ROADWIDTH, posXVeh4 = 2 * (float)ROADWIDTH;
    private const float posXVeh5 = 3 * (float)ROADWIDTH, posXVeh6 = 3 * (float)ROADWIDTH;
    private const float posXVeh7 = 4 * (float)ROADWIDTH, posXVeh8 = 4 * (float)ROADWIDTH;
    private const float posXVeh9 = 5 * (float)ROADWIDTH, posXVeh10 = 5 * (float)ROADWIDTH;
    private float[] refPosX = new float[NumVehicles] { posXVeh1, posXVeh2, posXVeh3, posXVeh4, posXVeh5,
                                              posXVeh6, posXVeh7, posXVeh8, posXVeh9, posXVeh10};
    private float[] refPosY = new float[NumVehicles] {0.8884f, -1.1471f, -1.0689f, -0.8095f, -2.9443f,
                                             1.4384f, 0.3252f, -0.7549f, 1.3703f, -1.7115f};
    float velX = 0f, velY = 20f;

    VehicleV2X vehicle1, vehicle2, vehicle3, vehicle4, vehicle5,
        vehicle6, vehicle7, vehicle8, vehicle9, vehicle10;
    int[] vehicleIDs = new int[NumVehicles] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    float[,] distBtwVeh = new float[NumVehicles, NumVehicles];                // zijn, trdist + noise
    float[,] angleBtwVeh = new float[NumVehicles, NumVehicles];                // thetaijn, traoa + noise

    public struct VehiclesPosition
    {
        public float[] posX;
        public float[] posY;
        public VehiclesPosition(VehicleV2X veh1, VehicleV2X veh2, VehicleV2X veh3, VehicleV2X veh4, VehicleV2X veh5,
            VehicleV2X veh6, VehicleV2X veh7, VehicleV2X veh8, VehicleV2X veh9, VehicleV2X veh10)
        {
            this.posX = new float[NumVehicles]
            {
                veh1.getPosition().Item1,
                veh2.getPosition().Item1,
                veh3.getPosition().Item1,
                veh4.getPosition().Item1,
                veh5.getPosition().Item1,
                veh6.getPosition().Item1,
                veh7.getPosition().Item1,
                veh8.getPosition().Item1,
                veh9.getPosition().Item1,
                veh10.getPosition().Item1
            };
            this.posY = new float[NumVehicles]
            {
                veh1.getPosition().Item2,
                veh2.getPosition().Item2,
                veh3.getPosition().Item2,
                veh4.getPosition().Item2,
                veh5.getPosition().Item2,
                veh6.getPosition().Item2,
                veh7.getPosition().Item2,
                veh8.getPosition().Item2,
                veh9.getPosition().Item2,
                veh10.getPosition().Item2
            };
        }
    }

    public struct VehiclesVariance
    {
        public float[] varX;
        public float[] varY;
        public VehiclesVariance(VehicleV2X veh1, VehicleV2X veh2, VehicleV2X veh3, VehicleV2X veh4, VehicleV2X veh5,
            VehicleV2X veh6, VehicleV2X veh7, VehicleV2X veh8, VehicleV2X veh9, VehicleV2X veh10)
        {
            this.varX = new float[NumVehicles]
            {
                veh1.getVariance().Item1,
                veh2.getVariance().Item1,
                veh3.getVariance().Item1,
                veh4.getVariance().Item1,
                veh5.getVariance().Item1,
                veh6.getVariance().Item1,
                veh7.getVariance().Item1,
                veh8.getVariance().Item1,
                veh9.getVariance().Item1,
                veh10.getVariance().Item1
            };
            this.varY = new float[NumVehicles]
            {
                veh1.getVariance().Item2,
                veh2.getVariance().Item2,
                veh3.getVariance().Item2,
                veh4.getVariance().Item2,
                veh5.getVariance().Item2,
                veh6.getVariance().Item2,
                veh7.getVariance().Item2,
                veh8.getVariance().Item2,
                veh9.getVariance().Item2,
                veh10.getVariance().Item2
            };
        }
    }

    public struct IndvDistAngle
    {
        public float[] measureDistVeh1;
        public float[] measureDistVeh2;
        public float[] measureDistVeh3;
        public float[] measureDistVeh4;
        public float[] measureDistVeh5;
        public float[] measureDistVeh6;
        public float[] measureDistVeh7;
        public float[] measureDistVeh8;
        public float[] measureDistVeh9;
        public float[] measureDistVeh10;
        public float[] measureAngleVeh1;
        public float[] measureAngleVeh2;
        public float[] measureAngleVeh3;
        public float[] measureAngleVeh4;
        public float[] measureAngleVeh5;
        public float[] measureAngleVeh6;
        public float[] measureAngleVeh7;
        public float[] measureAngleVeh8;
        public float[] measureAngleVeh9;
        public float[] measureAngleVeh10;
        public IndvDistAngle(float[] dist1, float[] dist2, float[] dist3, float[] dist4, float[] dist5, float[] dist6, float[] dist7, float[] dist8, float[] dist9, float[] dist10,
            float[] angle1, float[] angle2, float[] angle3, float[] angle4, float[] angle5, float[] angle6, float[] angle7, float[] angle8, float[] angle9, float[] angle10)
        {
            this.measureDistVeh1 = dist1;
            this.measureDistVeh2 = dist2;
            this.measureDistVeh3 = dist3;
            this.measureDistVeh4 = dist4;
            this.measureDistVeh5 = dist5;
            this.measureDistVeh6 = dist6;
            this.measureDistVeh7 = dist7;
            this.measureDistVeh8 = dist8;
            this.measureDistVeh9 = dist9;
            this.measureDistVeh10 = dist10;
            this.measureAngleVeh1 = angle1;
            this.measureAngleVeh2 = angle2;
            this.measureAngleVeh3 = angle3;
            this.measureAngleVeh4 = angle4;
            this.measureAngleVeh5 = angle5;
            this.measureAngleVeh6 = angle6;
            this.measureAngleVeh7 = angle7;
            this.measureAngleVeh8 = angle8;
            this.measureAngleVeh9 = angle9;
            this.measureAngleVeh10 = angle10;
        }

    }

    void CreateVeh()
    {
        vehicle1  = new V2XComponent.VehicleV2X(vehicleIDs[0], refPosX[0], refPosY[0], velX, velY);
        vehicle2  = new V2XComponent.VehicleV2X(vehicleIDs[1], refPosX[1], refPosY[1], velX, velY);
        vehicle3  = new V2XComponent.VehicleV2X(vehicleIDs[2], refPosX[2], refPosY[2], velX, velY);
        vehicle4  = new V2XComponent.VehicleV2X(vehicleIDs[3], refPosX[3], refPosY[3], velX, velY);
        vehicle5  = new V2XComponent.VehicleV2X(vehicleIDs[4], refPosX[4], refPosY[4], velX, velY);
        vehicle6  = new V2XComponent.VehicleV2X(vehicleIDs[5], refPosX[5], refPosY[5], velX, velY);
        vehicle7  = new V2XComponent.VehicleV2X(vehicleIDs[6], refPosX[6], refPosY[6], velX, velY);
        vehicle8  = new V2XComponent.VehicleV2X(vehicleIDs[7], refPosX[7], refPosY[7], velX, velY);
        vehicle9  = new V2XComponent.VehicleV2X(vehicleIDs[8], refPosX[8], refPosY[8], velX, velY);
        vehicle10 = new V2XComponent.VehicleV2X(vehicleIDs[9], refPosX[9], refPosY[9], velX, velY);
        vehicle1.WhoIsAnchor = 4; 
        vehicle2.WhoIsAnchor = 4;   
        vehicle3.WhoIsAnchor = 4;   
        vehicle4.WhoIsAnchor = 4;   
        vehicle5.WhoIsAnchor = 4;   
        vehicle6.WhoIsAnchor = 4;   
        vehicle7.WhoIsAnchor = 4;   
        vehicle8.WhoIsAnchor = 4;   
        vehicle9.WhoIsAnchor = 4;   
        vehicle10.WhoIsAnchor = 4;  
    }

    void InitBelief()
    {
        float[,] belief_rand = new float[NumVehicles, 2] { {-1.991195789422071f, -0.815854961856177f}, {0.028408933918270f,-1.997759124366525f}, {1.930515408752332f, -0.748191408314749f}, {0.941136788326655f,-0.199304020638472f }, {-0.006924451301872f ,-0.372760077970442f},
        {-0.338241647272311f, -0.847371160164792f}, {0.528265783681366f, 1.337238043128168f}, {1.056899594340163f, -1.616317257947854f}, {0.014740616602442f, -0.491817594931569f}, {1.614761266912288f, -0.969301567007431f} };
            
        //vehicle1.InitBelief(belief_rand[0,0], belief_rand[0,1]);
        //vehicle2.InitBelief(belief_rand[1,0], belief_rand[1,1]);
        //vehicle3.InitBelief(belief_rand[2,0], belief_rand[2,1]);
        //vehicle4.InitBelief(belief_rand[3,0], belief_rand[3,1]);
        //vehicle5.InitBelief(belief_rand[4,0], belief_rand[4,1]);
        //vehicle6.InitBelief(belief_rand[5,0], belief_rand[5,1]);
        //vehicle7.InitBelief(belief_rand[6,0], belief_rand[6,1]);
        //vehicle8.InitBelief(belief_rand[7,0], belief_rand[7,1]);
        //vehicle9.InitBelief(belief_rand[8,0], belief_rand[8,1]);
        //vehicle10.InitBelief(belief_rand[9,0], belief_rand[9,1]);
        vehicle1.InitBelief();
        vehicle2.InitBelief();
        vehicle3.InitBelief();
        vehicle4.InitBelief();
        vehicle5.InitBelief();
        vehicle6.InitBelief();
        vehicle7.InitBelief();
        vehicle8.InitBelief();
        vehicle9.InitBelief();
        vehicle10.InitBelief();
    }
    void InitMobilityInference()
    {
        float[,] veh_rand = new float[NumVehicles, 2] { {0.3065f, 0.1056f}, {0.5938f, 0.2827f}, {0.1552f, 6.5867e-04f}, {0.2836f, 0.5508f}, {0.8709f, 0.0423f},
        {0.9047f, 0.1310f}, {0.8337f, 0.8005f}, {0.9179f, 0.1373f}, {0.5047f, 0.4050f}, {0.1736f, 0.5752f} };
        //vehicle1.UpdateFactorMobility(velX, velY, veh_rand[0, 0], veh_rand[0, 1]);
        //vehicle2.UpdateFactorMobility(velX, velY, veh_rand[1, 0], veh_rand[1, 1]);
        //vehicle3.UpdateFactorMobility(velX, velY, veh_rand[2, 0], veh_rand[2, 1]);
        //vehicle4.UpdateFactorMobility(velX, velY, veh_rand[3, 0], veh_rand[3, 1]);
        //vehicle5.UpdateFactorMobility(velX, velY, veh_rand[4, 0], veh_rand[4, 1]);
        //vehicle6.UpdateFactorMobility(velX, velY, veh_rand[5, 0], veh_rand[5, 1]);
        //vehicle7.UpdateFactorMobility(velX, velY, veh_rand[6, 0], veh_rand[6, 1]);
        //vehicle8.UpdateFactorMobility(velX, velY, veh_rand[7, 0], veh_rand[7, 1]);
        //vehicle9.UpdateFactorMobility(velX, velY, veh_rand[8, 0], veh_rand[8, 1]);
        //vehicle10.UpdateFactorMobility(velX, velY, veh_rand[9, 0], veh_rand[9, 1]);
        vehicle1.UpdateFactorMobility(velX, velY);
        vehicle2.UpdateFactorMobility(velX, velY);
        vehicle3.UpdateFactorMobility(velX, velY);
        vehicle4.UpdateFactorMobility(velX, velY);
        vehicle5.UpdateFactorMobility(velX, velY);
        vehicle6.UpdateFactorMobility(velX, velY);
        vehicle7.UpdateFactorMobility(velX, velY);
        vehicle8.UpdateFactorMobility(velX, velY);
        vehicle9.UpdateFactorMobility(velX, velY);
        vehicle10.UpdateFactorMobility(velX, velY);
    }

    /*
    public void MeasureDist(params VehicleV2X[] vehicles)
    {
        EnvironmentV2X vehicleMoniter = new V2XEnvironment.EnvironmentV2X();
        distBtwVeh = vehicleMoniter.InterVehDistance(vehicles);
        Console.WriteLine(distBtwVeh.ToString());
    }
    */

    public void MeasureInterVehicleDist(float[] posX, float[] posY)
    {
        EnvironmentV2X vehicleMoniter = new V2XEnvironment.EnvironmentV2X();
        distBtwVeh = vehicleMoniter.InterVehDistance(posX, posY);
        //Console.WriteLine(distBtwVeh.ToString());
    }

    public void MeasureInterVehicleAngle(float[] posX, float[] posY)
    {
        EnvironmentV2X vehicleMoniter = new V2XEnvironment.EnvironmentV2X();
        angleBtwVeh = vehicleMoniter.InterVehAngle(posX, posY);
        //Console.WriteLine(angleBtwVeh.ToString());
    }

    public VehiclesPosition GetVehiclesPosition()
    {
        VehiclesPosition indvPosition = new VehiclesPosition(
            vehicle1, vehicle2, vehicle3, vehicle4, vehicle5,
            vehicle6, vehicle7, vehicle8, vehicle9, vehicle10
            );
        return indvPosition;
    }
    public VehiclesVariance GetVehiclesVariance()
    {
        VehiclesVariance indvVariance = new VehiclesVariance(
            vehicle1, vehicle2, vehicle3, vehicle4, vehicle5,
            vehicle6, vehicle7, vehicle8, vehicle9, vehicle10
            );
        return indvVariance;
    }

    public IndvDistAngle GetIndvMeasurements(float[,] distances, float[,] angles)
    {
        IndvDistAngle measurements = new IndvDistAngle(
            Enumerable.Range(0, NumVehicles).Select(x => distances[0, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[1, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[2, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[3, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[4, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[5, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[6, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[7, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[8, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => distances[9, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[0, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[1, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[2, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[3, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[4, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[5, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[6, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[7, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[8, x]).ToArray(),
            Enumerable.Range(0, NumVehicles).Select(x => angles[9, x]).ToArray()
            );
        return measurements;
    }
    
    public void UpdateMeasurementFactor(ref VehiclesPosition indvPositions, ref VehiclesVariance indvVariances, ref IndvDistAngle indvMeasurements)
    {
        EnvironmentV2X vehicleMoniter = new V2XEnvironment.EnvironmentV2X();
        vehicleMoniter.IndividualMeasure(vehicle1, indvMeasurements.measureDistVeh1, indvMeasurements.measureAngleVeh1, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle2, indvMeasurements.measureDistVeh2, indvMeasurements.measureAngleVeh2, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle3, indvMeasurements.measureDistVeh3, indvMeasurements.measureAngleVeh3, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle4, indvMeasurements.measureDistVeh4, indvMeasurements.measureAngleVeh4, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle5, indvMeasurements.measureDistVeh5, indvMeasurements.measureAngleVeh5, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle6, indvMeasurements.measureDistVeh6, indvMeasurements.measureAngleVeh6, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle7, indvMeasurements.measureDistVeh7, indvMeasurements.measureAngleVeh7, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle8, indvMeasurements.measureDistVeh8, indvMeasurements.measureAngleVeh8, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle9, indvMeasurements.measureDistVeh9, indvMeasurements.measureAngleVeh9, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);
        vehicleMoniter.IndividualMeasure(vehicle10, indvMeasurements.measureDistVeh10, indvMeasurements.measureAngleVeh10, indvPositions.posX, indvPositions.posY, indvVariances.varX, indvVariances.varY, refPosX, refPosY);

    }
    public void BroadcastPositions(ref VehiclesPosition positions, params VehicleV2X[] vehicles)
    {
        float posX, posY;
        EnvironmentV2X vehicleMoniter = new V2XEnvironment.EnvironmentV2X();
        int i = 0;
        foreach (VehicleV2X veh in vehicles)
        {
             posX = vehicleMoniter.GetBroadcastPos(veh).Item1;
             posY = vehicleMoniter.GetBroadcastPos(veh).Item2;
             positions.posX[i] = posX;
             positions.posY[i] = posY;
            i++;
        }
    }
    public void BroadcastVariances(ref VehiclesVariance variances, params VehicleV2X[] vehicles)
    {
        float varX, varY;
        EnvironmentV2X vehicleMoniter = new V2XEnvironment.EnvironmentV2X();
        int i = 0;
        foreach (VehicleV2X veh in vehicles)
        {
             varX = vehicleMoniter.GetBroadcastVar(veh).Item1;
             varY = vehicleMoniter.GetBroadcastVar(veh).Item2;
             variances.varX[i] = varX;
             variances.varY[i] = varY;
            i++;
        }
    }


    void Start()
    {
        CreateVeh();
        VehiclesPosition individualPositions;
        VehiclesVariance individualVariances;
        IndvDistAngle individualMeasurements;
        InitBelief();
        InitMobilityInference();
        MeasureInterVehicleDist(refPosX, refPosY);
        MeasureInterVehicleAngle(refPosX, refPosY);
        individualPositions = GetVehiclesPosition();
        individualVariances = GetVehiclesVariance();
        individualMeasurements = GetIndvMeasurements(distBtwVeh, angleBtwVeh);
        for (int i = 0; i<NumIteration; i++)
        {
            UpdateMeasurementFactor(ref individualPositions, ref individualVariances, ref individualMeasurements);
            BroadcastPositions(ref individualPositions, vehicle1, vehicle2, vehicle3, vehicle4, vehicle5, vehicle6, vehicle7, vehicle8, vehicle9, vehicle10);
            BroadcastVariances(ref individualVariances, vehicle1, vehicle2, vehicle3, vehicle4, vehicle5, vehicle6, vehicle7, vehicle8, vehicle9, vehicle10);
        }
        //NotiMeasureResults(vehicle1, vehicle2, vehicle3, vehicle4, vehicle5, vehicle6, vehicle7, vehicle8, vehicle9, vehicle10);
        Console.WriteLine("Test V2X Environment");
    }

    private void Update()
    {
        //StartCoroutine(new WaitForSecondsRealtime(60));
    }

}
#endif
