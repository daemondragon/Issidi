using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_Sync_Pos : NetworkBehaviour
{

    [Header("Options")]
    public float smoothSpeed = 10f;
    public Transform TargetChild;

    [SyncVar]
    private Vector3 mostRecentPos;
    private Vector3 prevPos;

    Transform TargetTransform;

    void Start()
    {
        if(TargetChild != null)
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
            if (prevPos != transform.position)
            {
                // Send position to server (function runs server-side)
                CmdSendDataToServer(transform.position);

                prevPos = transform.position;
            }
        }
        else
        {
            // Apply position to other players (mostRecentPos read from Server vis SyncVar)
            TargetTransform.position = Vector3.Lerp(transform.position, mostRecentPos, smoothSpeed * Time.deltaTime);
        }
    }

    [Command]
    void CmdSendDataToServer(Vector3 pos)
    {
        mostRecentPos = pos;
    }

}