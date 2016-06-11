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


            UpdateRot();
            return;

            transform.localEulerAngles = CameraTarget.transform.localEulerAngles; //la veneritude
        }



    }

    void UpdateRot()
    {
        RaycastHit R;
        if (Physics.Raycast(CameraTarget.position, CameraTarget.forward, out R, 500))
        {
            if (R.distance < 10)
            {
                transform.localEulerAngles = CameraTarget.transform.localEulerAngles;
            }
            else
            {
                transform.LookAt(R.transform.position, transform.up);
            }
        }
        else
        {
            transform.LookAt(transform.position + transform.forward * 500, transform.up);
        }

    }
}
