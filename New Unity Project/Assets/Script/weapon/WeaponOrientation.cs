using UnityEngine;
using System.Collections.Generic;

public class WeaponOrientation : MonoBehaviour
{
    private CameraDeplacement Camera;//to know the direction of the camera
    NetworkOwner net;//for future interpolation

    public Transform CameraTarget;

    // Use this for initialization
    void Start()
    {
        net = transform.root.GetComponent<NetworkOwner>();
        Camera = transform.root.GetComponentInChildren<CameraDeplacement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (net.IsMine())
            RotateWeapon2();
        //RotateWeapon();
    }

    public Vector3 getPosition()
    {
        return (transform.position);
    }

    public Quaternion getRotation()
    {
        return (transform.rotation);
    }

    public Vector3 getForward()
    {
        return (transform.forward);
    }
    void RotateWeapon2()
    {
        if (CameraTarget != null)
        {
            Vector3 rot = CameraTarget.localEulerAngles;
            rot.x += 16;
            transform.localEulerAngles = rot;
        }

    }
    void RotateWeapon()
    {
        if (!Camera)
            return;

        RaycastHit info;
        if (Physics.Raycast(Camera.Position(), Camera.Forward(), out info))
            transform.LookAt(info.point, transform.root.transform.up);//Look at the exact point where the camera ray hit something
        else
            transform.LookAt(transform.position + transform.root.transform.forward, transform.root.transform.up);
    }
}
