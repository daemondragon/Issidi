using UnityEngine;

[RequireComponent(typeof(Deplacement))]
[RequireComponent(typeof(NetworkOwner))]

public class CameraDeplacement : MonoBehaviour
{
    public float max_distance;
    private Vector3 camera_direction;
    public float mouse_sensivity;
    private bool activate;

    private Deplacement deplacement;

    void Start()
    {
        deplacement = transform.root.GetComponent<Deplacement>();

        NetworkOwner net = transform.root.GetComponent<NetworkOwner>();
        GetComponent<Camera>().enabled = net.IsMine();
        enabled = net.IsMine();

        activate = true;
        mouse_sensivity = 1.5f;
        max_distance = 3.0f;
    }

    void MoveCameraX(float rotation)
    {//up down rotation
        Vector3 rot = this.transform.parent.transform.localEulerAngles;
        rot.x -= rotation;
        if (rot.x > 60 && rot.x < 178)//exact top view
            rot.x = 60;
        else if (rot.x >= 178 && rot.x < 296)
            rot.x = 296;


        this.transform.parent.transform.localEulerAngles = rot;
    }

    void MoveCameraY(float rotation)
    {//left right rotation
        Vector3 axis = transform.root.transform.up;
        transform.root.transform.RotateAround(transform.parent.transform.position, axis, rotation);
    }

    void UpdateCamera()
    {
        //Prevent the camera to go inside wall
        Vector3 parent_position = transform.parent.transform.position;

        camera_direction = transform.position - parent_position;
        camera_direction.Normalize();

        RaycastHit info;
        if (Physics.Raycast(parent_position, camera_direction, out info, max_distance))
            transform.position = parent_position + camera_direction * info.distance * 0.7f;
        else
            transform.position = parent_position + camera_direction * max_distance * 0.9f;

        if (deplacement && deplacement.canMoveCamera())
            MoveCamera();
    }

    void MoveCamera()
    {
        Vector2 cursor = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        MoveCameraX(cursor.y * mouse_sensivity);
        MoveCameraY(cursor.x * mouse_sensivity);
    }

    void Update()
    {
        if (activate)
        {
            if (Input.GetKeyDown(KeyCode.F5))
                SwitchSide();

            UpdateCamera();
        }
    }

    void SwitchSide()
    {
        transform.RotateAround(transform.position, transform.root.transform.up, 180);
        transform.localPosition = new Vector3(transform.localPosition.x,
                                                transform.localPosition.y,
                                               -transform.localPosition.z);
    }

    public Vector3 Forward()
    {
        return transform.forward;
    }

    public Vector3 Position()
    {
        return transform.position;
    }
}
