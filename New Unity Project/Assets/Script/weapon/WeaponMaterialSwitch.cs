using UnityEngine;
using System.Collections;

public class WeaponMaterialSwitch : MonoBehaviour
{

    // Use this for initialization
    Stats Player;
    public Material Orange;
    public Material LightOrange;

   
    bool RendererDone = false;
    void Start()
    {
        Player = transform.root.GetComponent<Stats>();

    }

    private void RunSwitchColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                RendererDone = true;
                Material mat = mats[j];
                if (mat.name.Equals("LightBlue (Instance)")) { mat.color = LightOrange.color; }
                if (mat.name.Equals("blue (Instance)")) { mat.color = Orange.color; }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null && Player.team == Stats.Team.Orange && !RendererDone) { RunSwitchColor(); }
    }

}
