#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Globalization;


public class CarManager : MonoBehaviour
{
    const int numCars = 5;
    [Header("Scripts gameObject")]
    public GameObject myScripts;

    [Header("Map offset")]
    public int posOffsetX = 250;
    public int posOffsetY = 250;

    [Header("Vehicle ID Lists")]
    private string[] idList = new string[numCars];
    private string[] oldIdList = new string[numCars];
    GameObject[] car = new GameObject[numCars];
    private Dictionary<string, CarInfo> carDict = new Dictionary<string, CarInfo>();

    [Header("timer for motion visualization")]
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        InitCarEnv();
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.deltaTime;

        string recievedMsg = myScripts.GetComponent<SumoTcpClient>().RxMsg();
        SplitData(recievedMsg, timer);
    }

    public void InitCarEnv()
    {
        string[] moveableVehList = new string[] { "Tocus1", "Tocus2", "Tocus3", "Tocus4", "Tocus5" };
        for (int i=0; i < moveableVehList.Length; i++)
        {
            car[i] = GameObject.Find(moveableVehList[i]);
        }
    }

    public void SplitData(string msg, float timer)
    {
        if (msg.Contains("@"))
        {
            Array.Clear(idList, 0, idList.Length);
            string[] perVehicle = msg.Split('@');
            for (int i=0; i<perVehicle.Length; i++)
            {
                CarInfo car = new CarInfo(perVehicle[i]);
                idList[i] = car.vehId;
                // oldIdList에 car.vehId가 있는지 검사. 
                if (Array.IndexOf(oldIdList, car.vehId) == -1 && car.vehId != null)
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
            Transform(carDict, idList);
            oldIdList = idList;
        }
        else
        {

        }
    }

    public void Transform(Dictionary<string, CarInfo> carDict, string[] idList)
    {
        const int wrongCar = 6;
        int cntCars = carDict.Count;
        int vehType = wrongCar;

        for (int i=0; i<cntCars; i++)
        {
            CarInfo tempInfo = carDict[idList[i]];
            switch (tempInfo.vehId)
            {
                case "carA":
                    vehType = 0;
                    break;
                case "carB":
                    vehType = 0;
                    break;
                case "carC":
                    vehType = 0;
                    break;
                case "carD":
                    vehType = 0;
                    break;
                case "carE":
                    vehType = 0;
                    break;
                default:
                    print("something is wrong");
                    break;
            }
            Vector3 tempPos = car[vehType].transform.position;
            tempPos.x = (float)(tempInfo.posX + posOffsetX);
            tempPos.z = (float)(tempInfo.posY + posOffsetY);
            Quaternion tempRot = car[vehType].transform.rotation;
            Quaternion rot;
            Vector3 yDir = new Vector3(0, 1, 0);
            rot = Quaternion.AngleAxis((tempInfo.heading), yDir);
            car[vehType].transform.SetPositionAndRotation(tempPos, rot);
            car[vehType].GetComponent<srcVehicleHandler>().CalculateSteering(tempInfo.heading, tempInfo.speed, timer);
            car[vehType].GetComponent<srcVehicleHandler>().BrakeLightSwitch(tempInfo.brakeState);
        }
    }
}

public class CarInfo
{
    public string vehId;
    public float posX;
    public float posY;
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
                posX = (float)Convert.ToDouble(carMsg[1], new CultureInfo("ko-KR"));
                posY = (float)Convert.ToDouble(carMsg[2], new CultureInfo("ko-KR"));
                speed = (float)Convert.ToDouble(carMsg[3], new CultureInfo("ko-KR"));
                heading = (float)Convert.ToDouble(carMsg[4], new CultureInfo("ko-KR"));
                brakeLight = (float)Convert.ToDouble(carMsg[5], new CultureInfo("ko-KR"));
                sizeCategory = (int)Convert.ToDouble(carMsg[6], new CultureInfo("ko-KR"));

                if (brakeLight == 1)
                    brakeState = true;
                else
                    brakeState = false;
            }
            else
            {
                Debug.Log("CarInfo : incorrect message lenght");
            }
        }
    }
}
#endif
