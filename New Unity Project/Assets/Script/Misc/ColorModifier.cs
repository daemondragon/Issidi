using UnityEngine;
using System.Collections;

public class ColorModifier : MonoBehaviour
{
    public Material Blue;
    public Material Orange;

    public void SetTeam(Stats.Team team)
    {
        if (team != Stats.Team.None)
        {
            Material to_apply = Blue;
            if (team == Stats.Team.Orange)
                to_apply = Orange;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].materials;
                for (int j = 0; j < mats.Length; j++)
                    if (mats[j].HasProperty("_Color") && NeedChange(mats[j].color))
                        mats[j].color = to_apply.color;
            }
        }
        else
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].enabled = false;
        }
    }

    bool NeedChange(Color c)
    {
        bool is_black = c.r == 0 && c.g == 0 && c.b == 0;
        return (c == Orange.color ||
                c == Blue.color ||
                is_black);
    }
}
