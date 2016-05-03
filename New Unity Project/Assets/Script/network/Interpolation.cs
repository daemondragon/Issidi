using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(Rigidbody))]

public class Interpolation :  NetworkBehaviour
{
    Vector3 last_position;
    [SyncVar]
    Vector3 network_velocity;
    [SyncVar]
    Quaternion network_rotation;

    Rigidbody rigid_body;

    // Use this for initialization
    void Start ()
    {
        rigid_body = GetComponent<Rigidbody>();
        last_position = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (hasAuthority)
        {
            network_velocity = rigid_body.velocity;
            network_rotation = transform.rotation;
        }
        else
            Interpolate(Time.deltaTime);
	}

    void Interpolate(float delta_time)
    {
        float distance = Vector3.Distance(transform.position, last_position);
        float speed = network_velocity.magnitude;

        if (distance < 3.0f)//Random value
        {//In this case, we interpolate the position, else we teleport the player (too far from target)
            if (speed * delta_time < distance)
            {//Need interpolation
                rigid_body.velocity = network_velocity;
            }//else, at this frame it will be at the good position
        }
        last_position = transform.position;

        //Interpolate rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, network_rotation, 0.75f);
    }
}
