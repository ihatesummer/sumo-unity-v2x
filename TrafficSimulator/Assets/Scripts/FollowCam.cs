#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public float posX = 250;
    public float posZ = 250;
    private float dist = 10.0f;
    private float height = 50.0f;
    ////private float smoothRotate = 5.0f;

    private Transform camTransform;
    public Transform targetObjTransform;
    // Start is called before the first frame update
    void Start()
    {
        //camTransform = GetComponent<Transform>();
        //targetObjTransform = GameObject.Find("Vehicle Object").transform;
        
    }

    // Update is called once per frame
    // LateUpdate() is called after all Update() functions are executed
    void LateUpdate()
    {
        //camTransform.position = targetObjTransform.position - Vector3.forward * dist + Vector3.up * height;
        ////pos_cam.LookAt(target.transform);
    }
}
#endif
