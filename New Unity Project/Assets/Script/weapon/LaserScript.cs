using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour
{
    LineRenderer line;

    public Material Nothing;
    public Material ReadyToShoot;

    Stats MyStats;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = true;
        MyStats = transform.root.GetComponent<Stats>();
    }
    void Update()
    {
        DrawLine();
    }

    void DrawLine()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        line.SetPosition(0, ray.origin);

        Material EndMat = Nothing;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Stats S = hit.transform.GetComponent<Stats>();
            if ((S && MyStats && S.team != MyStats.team) || hit.transform.GetComponent<CubeLife>())
            {
                EndMat = ReadyToShoot;
            }
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, ray.GetPoint(100));
        }
        line.material = EndMat;

    }

}