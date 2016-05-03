using UnityEngine;
using System.Collections;

public class ColorModifier : MonoBehaviour
{
    public Material White;
    public Material Blue;
    public Material Orange;

    public void SetTeam(Stats.Team team)
    {
        Material to_apply = White;
        if (team == Stats.Team.Blue)
            to_apply = Blue;
        else if (team == Stats.Team.Orange)
            to_apply = Orange;

        foreach (Transform child in transform)
        {
            Material mat = child.gameObject.GetComponent<Renderer>().material;
            if (NeedChange(mat.color))
                mat.color = to_apply.color;
        }
    }

    bool NeedChange(Color c)
    {
        return (c == Orange.color ||
                c == Blue.color ||
                c == White.color);
    }
}
