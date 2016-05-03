using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Orientation : MonoBehaviour
{
    private Transform CameraTarget;
    NetworkOwner net;
    void Start()
    {
        CameraTarget = transform.root.FindChild("CameraTarget");
      
        net = transform.root.GetComponent<NetworkOwner>();
    }
    void Update()
    {
        if (net.IsMine())
        {
            transform.localEulerAngles = CameraTarget.transform.localEulerAngles; //la veneritude
        }



    }
}
