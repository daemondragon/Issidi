using UnityEngine;
using System.Collections;

public class SniperCamBehavior : MonoBehaviour
{

    enum CamBeh
    {
        Zommed,
        Zooming,
        UnZooming,
        UnZoomed

    }
    // Use this for initialization

    NetworkOwner net;
    public Camera cam;


    int MinFOV = 20; //Zoomed
    int MaxFOV = 60; //DeZoomed FRANGLAIS

    int Dif;

    float ZoomingTime = 0.15f; //s
    float Elasped = 0f;

    bool IsZoomed = false;

    void Start()
    {
        net = transform.root.GetComponent<NetworkOwner>();
        Dif = MaxFOV - MinFOV;
    }

    // Update is called once per frame
    void Update()
    {
        if (!net.IsMine())
            return;
        Elasped += Time.deltaTime;
        if (IsZoomed)
        {

            Zoom();
        }
        else
        {
            DeZoom();
        }
        if (Input.GetMouseButtonDown(1))
        {
            IsZoomed = !IsZoomed;
            Elasped = 0;
        }
    }

    void DeZoom()
    {
        if (cam.fieldOfView < MaxFOV)
        {
            cam.fieldOfView = MinFOV + Dif * (Elasped / ZoomingTime);
        }
    }

    void Zoom()
    {
        if (cam.fieldOfView > MinFOV)
        {
            cam.fieldOfView = MaxFOV - Dif * (Elasped / ZoomingTime);
        }

    }
}
