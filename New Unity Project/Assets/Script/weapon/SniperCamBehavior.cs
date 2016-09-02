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
    Stats stats;
    public Camera cam;

    float BTime = 0.250f;
    float CurTime = 0;


    int MinFOV = 20; //Zoomed
    int MaxFOV = 60; //DeZoomed FRANGLAIS

    int Dif;

    float ZoomingTime = 0.15f; //s
    float Elasped = 0f;

    bool IsZoomed = false;

    public Transform MeshToHide;

    void Start()
    {
        net = transform.root.GetComponent<NetworkOwner>();
        stats = GetComponentInParent<Stats>();
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
            if (MeshToHide != null && (!stats || stats.team != Stats.Team.None))
            {
                MeshToHide.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            Zoom();
        }
        else
        {
            if (MeshToHide != null && (!stats || stats.team != Stats.Team.None))
            {
                MeshToHide.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
            DeZoom();
        }
        if (Input.GetMouseButtonDown(1) && CurTime > BTime)
        {
            IsZoomed = !IsZoomed;
            Elasped = 0;
            CurTime = 0;
        }
        else
        {
            CurTime += Time.deltaTime;
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
