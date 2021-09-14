using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System.Linq;
using UnityEditor;
using System;

public class MainLogic : MonoBehaviour
{
    [Header("Simulation Time")]
    private Stopwatch stopWatch;
    [Header("Main Camera target control")]
    private CamTargetControl _followCam;

    [Header("TCP Interface")]
    private ITcpClient _tcpClient;
    private string _receivedMsg;
    private bool _quitApp = false;
    [Header("Vehicle List")]
    private VehicleListCollection _vehicleCollection;
    [Header("Vehicle Info Processor Interface")]
    private IVehUpdtAndDel _sumoVehFactory;
    private MsgVehFactoryChain _msgPassingVehFactory;
    [Header("Message Passing Algorithm")]
    private LocalizationAlgoritm _msgPassingAlgorithm;
    private const int N_ITERATION = 10;

    [Header("Prefabs of vehicle you want to instantiate")]
    public GameObject sumoCarPrefab;
    public GameObject msgCarPrefab;
    public GameObject sumoBusPrefab;
    public GameObject msgBusPrefab;
    public GameObject sumoTrainPrefab;
    public GameObject msgTrainPrefab;
    [Header("container for vehicle prefabs")]
    private VehiclePrefabs _vehiclePrefabs;
    [Header("Make vehicle appear in the game scene")]
    private VehicleInstance _vehInstantiator;
    [Header("Make vehicle disappear in the game scene")]
    private VehicleDestroy _vehDestroyer;
    [Header("Calculate Performance Metric")]
    private PerformanceMetrics pMetrics;

    public const float UPDATE_DELAY = 0.05f;
    bool delayCheck = true;

    private void Awake()
    {
        stopWatch = new Stopwatch();
    }

    // Start is called before the first frame update
    void Start()
    {
        stopWatch.Start();
        _followCam = new CamTargetControl();
        //_tcpClient = new TcpVehBridge(GetComponent<SumoTcpClient>());
        _tcpClient = new TcpVehBridge(GetComponent<SumoUnityTcpHandler>());
        _vehicleCollection = VehicleListCollection.Instance();
        _sumoVehFactory = new SumoVehFactory();
        _msgPassingVehFactory = new MsgVehFactoryChain();
        _msgPassingAlgorithm = new LocalizationAlgoritm();
        _vehiclePrefabs = new VehiclePrefabs(sumoCarPrefab, msgCarPrefab, sumoBusPrefab, msgBusPrefab, sumoTrainPrefab, msgTrainPrefab);
        _vehInstantiator = new VehicleInstance(_vehiclePrefabs);
        _vehDestroyer = new VehicleDestroy();
        pMetrics = new PerformanceMetrics();
    }

    // Update is called once per frame
    void Update()
    {
        if (delayCheck)
        {
            UnityRoutine();
            //delayCheck = false;
            //UnityRoutine();
            //StartCoroutine(SleepTimerInternal());
        }
    }
    void UnityRoutine()
    {
        _receivedMsg = _tcpClient.RxMsg();
        //UnityEngine.Debug.Log(_receivedMsg);
        _quitApp = _tcpClient.IsLastMsg();
        if (_receivedMsg != "null")
        {
            _vehicleCollection.oldIdList = GetLastVehListInternal();
            //_vehicleCollection.idList.Clear();
            //_vehicleCollection.idList = new List<float>();
            _vehicleCollection.InitIdList();
            RunVehicleFactoryInternal(_receivedMsg);
            _msgPassingAlgorithm.InitMsgPassingVehicle(_msgPassingVehFactory.msgPassingEquippedVehicleDict);
            _msgPassingAlgorithm.UpdateFactorMobility(_msgPassingVehFactory.msgPassingEquippedVehicleDict);
            _msgPassingAlgorithm.UpdatePosition(N_ITERATION,_msgPassingVehFactory.msgPassingEquippedVehicleDict);
            _vehInstantiator.ToScene(_vehicleCollection.sumoCarDict, _vehicleCollection.msgCarDict);
            _followCam.SetCameraTargetObj(_vehInstantiator);
            _vehDestroyer.ToScene(_vehicleCollection.sumoCarDict, _vehicleCollection.msgCarDict);
            pMetrics.UpdateError();
            _sumoVehFactory.DeleteVehicleInDict();
            _msgPassingVehFactory.DeleteVehicleInDict();
            _tcpClient.TxMsg(
                "Absolute Error (min) "+pMetrics.absoluteError.min +" "+
                "Absolute Error (max) "+pMetrics.absoluteError.max +" "+
                "Absolute Error (avg) "+pMetrics.absoluteError.average +" "+
                "Relative Error (min) "+pMetrics.relativeError.min +" "+
                "Relative Error (max) "+pMetrics.relativeError.max +" "+
                "Relative Error (avg) "+pMetrics.relativeError.average +" "
                );
        }
        else
        {
            _tcpClient.TxMsg(
                //"Absolute Error (min) "+pMetrics.absoluteError.min +" "+
                //"Absolute Error (max) "+pMetrics.absoluteError.max +" "+
                //"Absolute Error (avg) "+pMetrics.absoluteError.average +" "+
                //"Relative Error (min) "+pMetrics.relativeError.min +" "+
                //"Relative Error (max) "+pMetrics.relativeError.max +" "+
                //"Relative Error (avg) "+pMetrics.relativeError.average +" "
                "Absolute Error (min) "+0f+" "+
                "Absolute Error (max) "+0f+" "+
                "Absolute Error (avg) "+0f+" "+
                "Relative Error (min) "+0f+" "+
                "Relative Error (max) "+0f+" "+
                "Relative Error (avg) "+0f+" "
                );

        }
        if (_quitApp)
        {
            Quit();
        }
    }
    IEnumerator SleepTimerInternal()
    {
        yield return new WaitForSecondsRealtime(UPDATE_DELAY);
        delayCheck = true;
    }
    private List<float> GetLastVehListInternal()
    {
        return _vehicleCollection.sumoCarDict.Keys.ToList();
    }
    private void RunVehicleFactoryInternal(string msg)
    {
        if (msg.Contains("@"))
        {
            string[] perVehicle = msg.Split('@');
            _sumoVehFactory.UpdateVehInfo(perVehicle);
            _msgPassingVehFactory.UpdateVehInfo(perVehicle);
        }
        else if (_quitApp)
        {
            _sumoVehFactory.UpdateVehInfo(msg);
            _msgPassingVehFactory.UpdateVehInfo(msg);
        }
    }
    public void Quit()
    {
#if UNITY_EDITOR
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        EditorApplication.isPaused = true;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds);
        UnityEngine.Debug.Log("Elapsed time (HH:MM:SS) : "+elapsedTime);
#endif
    }
}
