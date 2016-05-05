using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class Interpolation : NetworkBehaviour
{
    Rigidbody body;

    [SyncVar]
    Vector3 network_position;
    [SyncVar]
    Quaternion network_rotation;
    [SyncVar]
    Vector3 network_velocity;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (hasAuthority)
                Cmd_SendData(transform.position, transform.rotation, body.velocity);
        else
            Interpolate();
    }

    [Command]
    void Cmd_SendData(Vector3 pp, Quaternion pr, Vector3 pv)
    {
        network_position = pp;
        network_rotation = pr;
        network_velocity = pv;
    }

    void Interpolate()
    {
        transform.position = Vector3.Lerp(transform.position, network_position, 0.75f);
        transform.rotation = Quaternion.Lerp(transform.rotation, network_rotation, 0.75f);
        body.velocity = network_velocity;
    }
}
