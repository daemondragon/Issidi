using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_Sync_Rot : NetworkBehaviour
{

    [Header("Options")]
    public float smoothSpeed = 10f;
    public Transform TargetChild;

    [SyncVar]
    private Quaternion mostRecentRot;
    private Quaternion prevRot;

    Transform TargetTransform;

    void Start()
    {
        if (TargetChild != null)
        {
            TargetTransform = TargetChild;
        }
        else
        {
            TargetTransform = transform;
        }
    }

   
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            // If moved, send my data to server
            if (prevRot != TargetTransform.rotation)
            {
                // Send position to server (function runs server-side)
                CmdSendDataToServer(TargetTransform.rotation);

                prevRot = TargetTransform.rotation;
            }
        }
        else
        {
            // Apply position to other players (mostRecentPos read from Server vis SyncVar)
            TargetTransform.rotation = Quaternion.Lerp(TargetTransform.rotation, mostRecentRot, smoothSpeed * Time.deltaTime);


        }
    }

    [Command]
    void CmdSendDataToServer(Quaternion rot)
    {
        mostRecentRot = rot;
    }

}