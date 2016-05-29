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

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!activated)
        {
            if (repop > repop_time)
            {
                GetComponent<Renderer>().enabled = true;
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

        GetComponent<Renderer>().enabled = false;
        activated = false;
        repop = 0.0f;
    }

    void UpdateRotation()
    {
        if (!hasAuthority)
            return;

        transform.RotateAround(transform.position, transform.up, Time.deltaTime * 360.0f / rotation_time);
    }
}
