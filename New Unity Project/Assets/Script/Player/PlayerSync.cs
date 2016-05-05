using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSync : NetworkBehaviour
{
    Rigidbody body;
    Transform weapon;

    [SyncVar]
    Vector3 player_position;
    [SyncVar]
    Quaternion player_rotation;
    [SyncVar]
    Vector3 player_velocity;

    [SyncVar]
    Vector3 weapon_position;
    [SyncVar]
    Quaternion weapon_rotation;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        WeaponOrientation temp = GetComponentInChildren<WeaponOrientation>();
        if (temp)
            weapon = temp.transform;
        else
            Debug.Log("No WeaponOrientation script found in children of " + this);

        Debug.Log(hasAuthority);
    }

    void FixedUpdate()
    {
        if (hasAuthority)
        {
            if (weapon)
                Cmd_SendData(transform.position, transform.rotation, body.velocity, weapon.localPosition, weapon.localRotation);
            else
                Cmd_SendData(transform.position, transform.rotation, body.velocity, Vector3.zero, Quaternion.identity);
        }
        else
            Interpolate();
    }

    [Command]
    void Cmd_SendData(Vector3 pp, Quaternion pr, Vector3 pv, Vector3 wp, Quaternion wr)
    {
        player_position = pp;
        player_rotation = pr;
        player_velocity = pv;

        if (weapon)
        {
            weapon_position = wp;
            weapon_rotation = wr;
        }
    }

    void Interpolate()
    {
        transform.position = Vector3.Lerp(transform.position, player_position, 0.75f);
        transform.rotation = Quaternion.Lerp(transform.rotation, player_rotation, 0.75f);
        body.velocity = player_velocity;

        //Weapon Interpolation
        if (weapon)
        {
            weapon.localPosition = weapon_position;
            weapon.localRotation = weapon_rotation;
        }
    }
}
