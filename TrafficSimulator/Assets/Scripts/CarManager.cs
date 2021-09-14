#if false
#define isDestroy
//#undef isDestroy


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Globalization;
using System.Linq;
using UnityEditor;
using System.Numerics;

public class CarManager : MonoBehaviour
{
    [Header("Camera")]
    GameObject followCam;

    [Header("polling whether tcp comm is over")]
    private bool quitApp = false;

    [Header("Scripts gameObject")]
    public GameObject vehiclePrefab;
    private double steeringAngleApproximationFactor = 6.0f;

    [Header("Map offset")]
    public int posOffsetX = 250;
    public int posOffsetY = 250;

    [Header("Vehicle ID")]
    private List<string> idList;
    private List<string> oldIdList;
    private static Dictionary<string, GameObject> vehicles3D;
    private List<GameObject> vehiclesInScene;
    private Dictionary<string, CarInfo> prevStepCarDict = new Dictionary<string, CarInfo>();
    private Dictionary<string, CarInfo> carDict = new Dictionary<string, CarInfo>();

    [Header("Simulation Time")]
    private float startUpTime;

    //[Header("timer for motion visualization")]
    //private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        startUpTime = Time.time;
        InitPhysicSetting();
        //followCam = GameObject.Find("Main Camera");
        followCam = GameObject.FindGameObjectWithTag("MainCamera");
        InitCarEnv();
        //InitColors();
    }

    // Update is called once per frame
    void Update()
    {

        string receivedMsg = GetComponent<SumoTcpClient>().RxMsg();
        quitApp = GetComponent<SumoTcpClient>().IsLastMsg();
//        Debug.Log(receivedMsg);
        if (receivedMsg != null)
        {
            prevStepCarDict = new Dictionary<string, CarInfo>(carDict);
            SplitData(receivedMsg);
            Update3DVehiclesInScene(carDict, idList);
            
        }
        if (quitApp)
        {
            Quit();
        }
    }

    void InitPhysicSetting()
    {
        Physics.gravity = new UnityEngine.Vector3(0, 0, 0);
        vehiclePrefab.GetComponent<Rigidbody>().useGravity = false;
        vehiclePrefab.GetComponent<BoxCollider>().enabled = false;
    }

    public void InitCarEnv()
    {
        idList = new List<string>();
        oldIdList = new List<string>();
        vehiclesInScene = new List<GameObject>();
        vehicles3D = new Dictionary<string, GameObject>();
    }

    public void SplitData(string msg)
    {
        oldIdList = idList.ToList();
        idList.Clear();
        if (msg.Contains("@"))
        {
            string[] perVehicle = msg.Split('@');
            UpdateVehID(perVehicle);
            //oldIdList = idList.ToList();
        }
        else if (quitApp)
        {
            UpdateVehID(msg);
        }
    }

    void UpdateVehID(string[] perVehicle)
    {
        for (int i=0; i<perVehicle.Length; i++)
        {
            CarInfo car = new CarInfo(perVehicle[i]);
            CheckExistance(car);
        }

    }
    void UpdateVehID(string msg)
    {
        CarInfo car = new CarInfo(msg);
        CheckExistance(car);
    }
    void CheckExistance(CarInfo car)
    {
        idList.Add(car.vehId);
        // oldIdList에 car.vehId가 있는지 검사. 
        if (!oldIdList.Contains(car.vehId) && car.vehId != null)
        {   
            // new car
            carDict.Add(car.vehId, car);
        }
        else if(car.vehId != null)
        {   // update an existing car
            carDict[car.vehId] = car;
        }
        else
        {

        }

    }
    public void Update3DVehiclesInScene(Dictionary<string, CarInfo> carDict, List<string> idList)
    {
        int cntCars = idList.Count;
        UnityEngine.Vector3 objPosition;
        UnityEngine.Quaternion objRotation;
        GameObject vehObject;
        for (int i = 0; i < cntCars; i++)
        {
            if (vehicles3D.ContainsKey(idList[i]))
            {
                vehicles3D[idList[i]].transform.position = new UnityEngine.Vector3(carDict[idList[i]].posX, carDict[idList[i]].posZ, carDict[idList[i]].posY);
                vehicles3D[idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0, carDict[idList[i]].heading, 0);
                //vehicles3D[idList[i]].transform.rotation = UnityEngine.Quaternion.Euler(0, carDict[idList[i]].heading - 90.0f, 0);

                // Set wheel rotational speed from vehicle speed
                //double speed = carDict[idList[i]].speed;
                //double wheelRadius = 0.4;
                //float rotationAngleDelta = (float)(Time.deltaTime * speed / wheelRadius * Mathf.Rad2Deg);
                ////vehicles3D[idList[i]].transform.Find("vehiclePrefab").gameObject.transform.Find("Wheels/WheelsColliders/WheelFL").gameObject.transform.Rotate(Vector3.right, rotationAngleDelta);
                //vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FL").gameObject.transform.Rotate(UnityEngine.Vector3.up, rotationAngleDelta);
                //vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FR").gameObject.transform.Rotate(UnityEngine.Vector3.up, rotationAngleDelta);
                //vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel RL").gameObject.transform.Rotate(UnityEngine.Vector3.up, rotationAngleDelta);
                //vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel RR").gameObject.transform.Rotate(UnityEngine.Vector3.up, rotationAngleDelta);

                //// Set Wheel Angles (Steering Angle) as approximated by the last yaw angle and the current one divided by time (=speed)
                //if (prevStepCarDict.ContainsKey(idList[i]))
                //{
                //    float angularSpeed = (float)((carDict[idList[i]].heading - prevStepCarDict[idList[i]].heading) * steeringAngleApproximationFactor);
                //    float actualXVL = vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FL").gameObject.transform.localRotation.eulerAngles.x;
                //    float actualZVL = vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FL").gameObject.transform.localRotation.eulerAngles.z;
                //    float actualXVR = vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FR").gameObject.transform.localRotation.eulerAngles.x;
                //    float actualZVR = vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FR").gameObject.transform.localRotation.eulerAngles.z;
                //    vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FL").gameObject.transform.localRotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(actualXVL, angularSpeed, actualZVL));
                //    vehicles3D[idList[i]].gameObject.transform.Find("Wheels/Wheels colliders/Wheel FR").gameObject.transform.localRotation = UnityEngine.Quaternion.Euler(new UnityEngine.Vector3(actualXVR, angularSpeed, actualZVR));
                //}

            }
            else
            {
                objPosition = new UnityEngine.Vector3(carDict[idList[i]].posX, carDict[idList[i]].posZ, carDict[idList[i]].posY);
                objRotation = UnityEngine.Quaternion.Euler(0, carDict[idList[i]].heading, 0);
                //objRotation = UnityEngine.Quaternion.Euler(0, carDict[idList[i]].heading - 90.0f, 0);
                vehObject = Instantiate(vehiclePrefab, objPosition, objRotation);
                vehObject.name = idList[i];


                // Position
                //vehObject.transform.position = new Vector3(carDict[idList[i]].posX, carDict[idList[i]].posZ, carDict[idList[i]].posY);
                //vehObject.transform.rotation = Quaternion.Euler(0, carDict[idList[i]].heading, 0);
                //vehObject.transform.Rotate(new Vector3(0, 1, 0), carDict[idList[i]].heading - 90.0f);

                vehicles3D.Add(idList[i], vehObject);
                vehiclesInScene.Add(vehObject);
            }
        }

#if isDestroy
        if (cntCars < vehicles3D.Count)
        {
            List<string> destroyList = new List<string>();
            foreach (string id in vehicles3D.Keys)
            {
                if (!idList.Contains(id))
                {
                    destroyList.Add(id);
                }
            }
            foreach (string id in destroyList)
            {
                Destroy(vehicles3D[id]);
                vehicles3D.Remove(id);
            }
        }
#endif

        CamTarget(vehicles3D[idList[0]]);

    }
    public void Quit()
    {
#if UNITY_EDITOR

        EditorApplication.isPaused = true;
        Debug.Log("Elapsed time (min) : "+((Time.realtimeSinceStartup - startUpTime)/ 60.0f).ToString());
        //Debug.Log("Elapsed time (sec) : "+(EditorApplication.timeSinceStartup).ToString());
#endif

    }

    public void CamTarget(Dictionary<string, CarInfo> vehicles)
    {
        float alpha = 0.5f;
        float avgX = vehicles[idList[0]].posX;
        float avgZ = vehicles[idList[0]].posZ;
        Camera worldCamera = followCam.GetComponent<FollowCam>().GetComponent<Camera>();

        int cntCar = vehicles.Count;

        for (int i=0; i < cntCar; i++ ) {
            avgX = (alpha * vehicles[idList[i]].posX) + (1 - alpha)*avgX;
            avgZ = (alpha * vehicles[idList[i]].posX) + (1 - alpha)*avgZ;
        }
        followCam.GetComponent<FollowCam>().posX = avgX;
        followCam.GetComponent<FollowCam>().posZ = avgZ;
    }
    public void CamTarget(GameObject target)
    {
        followCam.GetComponent<FollowCam>().targetObjTransform = target.transform;
    }
}

public class CarInfo
{
    public string vehId;
    public float posX;
    public float posY;
    public float posZ;
    public float speed;
    public float heading;
    public float brakeLight;
    public int sizeCategory;

    public bool brakeState;

    public CarInfo(string tcpMsg)
    {
        if (tcpMsg.Contains(";"))
        {
            string[] carMsg = tcpMsg.Split(';');
            if (carMsg.Length >= 7)
            {
                vehId = carMsg[0];
                // CultureInfo : 날짜 형식이나 시간 형식, 숫자 형식, 문자열의 정렬 등을 설정 국가에 맞게 정의
                posX = (float)Convert.ToDouble(carMsg[1], new CultureInfo("en-US"));
                posY = (float)Convert.ToDouble(carMsg[2], new CultureInfo("en-US"));
                posZ = (float)Convert.ToDouble(carMsg[3], new CultureInfo("en-US"));
                speed = (float)Convert.ToDouble(carMsg[4], new CultureInfo("en-US"));
                heading = (float)Convert.ToDouble(carMsg[5], new CultureInfo("en-US"));
                brakeLight = (float)Convert.ToDouble(carMsg[6], new CultureInfo("en-US"));
                sizeCategory = (int)Convert.ToDouble(carMsg[7], new CultureInfo("en-US"));

                if (brakeLight == 1)
                    brakeState = true;
                else
                    brakeState = false;
            }
            else
            {
                Debug.Log("CarInfo : incorrect message length");
            }
        }
    }
}
#endif
