using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTargetControl
{
    private float distance;
    private float height;
    private Transform camTransform;
    public CamTargetControl()
    {
        distance = 10.0f;
        height = 100.0f;
        //height = 200.0f;
        //height = 500.0f;
        camTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public void SetCameraTargetObj(ITarget vehInstantiator)
    {
        GameObject targetObject = vehInstantiator.GetTarget();
        camTransform.position = targetObject.transform.position - Vector3.forward * distance + Vector3.up * height;
    }
}
