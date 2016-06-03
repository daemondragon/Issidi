using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Flamethrower : NetworkBehaviour
{


    Stats stats;
    ParticleSystem part;
    ParticleSystem.EmissionModule em_mod;

    public Transform TargetTransform;

    bool OldBool = false;

    NetworkOwner me;

    [SyncVar(hook = "EnablePartsChanged")]
    bool EnableParts = true;

    void Start()
    {
        me = GetComponent<NetworkOwner>();
        part = TargetTransform.GetComponent<ParticleSystem>();
        em_mod = part.emission;
        EnableParts = false;
        stats = GetComponent<Stats>();
    }

    private void EnablePartsChanged(bool value)
    {

        if (part == null)
            return;

        if (value)
        {
            part.Play(true);
        }
        else
        {
            part.Stop(true);
        }
        em_mod.enabled = value;

    }

    // Update is called once per frame
    void Update()
    {
        if (stats.IsDead() || !stats.CanMovePlayer)
            return;

        if (!me.isLocalPlayer)
            return;

        bool testeur = me.IsMine() && stats.CanShoot() && Input.GetButton("Fire1");

        if (testeur != OldBool)
        {
            CmdThrusters(testeur);
        }

        OldBool = testeur;
    }


    [Command]
    void CmdThrusters(bool ena)
    {
        Debug.Log("sending :" + ena);
        EnableParts = ena;
    }




}
