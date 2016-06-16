using UnityEngine;
using System.Collections;

public class MessageZone : MonoBehaviour
{
    HUD_solo hud;
    public string message;
    // Use this for initialization
    void Start()
    {
        hud = null;
        //
    }

    void OnTriggerEnter(Collider col)
    {
        if (!hud)
        {
            GameObject temp = GameObject.FindGameObjectWithTag("HUD");
            if (temp)
                hud = temp.GetComponent<HUD_solo>();
        }
        if (hud)
            hud.setInfoMessage(message);
    }
}
