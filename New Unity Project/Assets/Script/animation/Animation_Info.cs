using UnityEngine;
using System.Collections;

public class Animation_Info : MonoBehaviour
{
    public bool on_dash;
    public bool jumping;
    public bool double_jump;
    public bool shot;

    void Start()
    {
        on_dash = false;
        jumping = false;
        double_jump = false;
        shot = false;
    }
}
