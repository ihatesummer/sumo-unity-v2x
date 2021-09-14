#if true
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class NetParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ImportAndGenerate.parseXMLfiles("./Assets/Scripts/environment");
        //SumoNetLoader.parseJunctions("./Assets/Scripts/environment");
        SumoNetLoader.parseXMLfiles("./Assets/Scripts/environment");
        UnityEngine.Debug.Log("SUMO XML Load test is over");
        SumoNetLoader.drawStreetNetwork();
    }

    // Update is called once per frame
    void Update()
    {
        //UnityEngine.Debug.Log("SUMO XML Load test is over");
    }
}
#endif
