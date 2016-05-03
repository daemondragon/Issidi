using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flamethrower : NetworkBehaviour
{

    // Use this for initialization
    Stats stats;
    ParticleSystem part;
    ParticleSystem.EmissionModule em_mod;

    NetworkOwner me;

    [SyncVar]
    bool distant_player;

    void Start()
    {
        me = transform.root.GetComponent<NetworkOwner>();
        part = GetComponent<ParticleSystem>();
        em_mod = part.emission;
        stats = transform.root.GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {

       
        if (stats.IsDead() || stats.paused)
            return;



        em_mod.enabled = me.IsMine() && stats.CanShoot() && Input.GetButton("Fire1");


    }
    

}
