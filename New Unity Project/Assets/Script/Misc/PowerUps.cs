using UnityEngine;
using UnityEngine.Networking;

public class PowerUps : NetworkBehaviour
{
    public int life_bonus;
    public int energy_bonus;
    public int ammo_bonus;

    public float repop_time;
    public float rotation_time;

    bool activated = true;
    [SyncVar]
    float repop;

    // Update is called once per frame
    void Update()
    {
        if (!activated)
        {
            if (repop > repop_time)
            {
                SwitchRenderers(true);
                activated = true;
            }
            else if (hasAuthority)
                repop += Time.deltaTime;
        }

        UpdateRotation();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            GiveBonus(collider.gameObject);
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            GiveBonus(collider.gameObject);
    }

    void GiveBonus(GameObject player)
    {
        if (!activated)
            return;

        Stats stats = player.GetComponent<Stats>();
        if (stats)
        {
            stats.Life += life_bonus;
            stats.Energy += energy_bonus;
            stats.Ammo += ammo_bonus;
        }

        SwitchRenderers(false);
        activated = false;
        repop = 0.0f;
    }


    void SwitchRenderers(bool b)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = b;
        }
    }

    void UpdateRotation()
    {
        if (!hasAuthority)
            return;

        if (rotation_time == 0.0f)
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * 360.0f);
        else
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * 360.0f / rotation_time);
    }
}
