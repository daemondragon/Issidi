using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CubeLife : NetworkBehaviour
{
    public float MaximumLife = 100;

    [SyncVar]
    float Life;
    public GameObject ExplosionEffect;

    void Start()
    {
        Life = MaximumLife;
        if (ExplosionEffect == null)
        {
            throw new System.Exception("No explosion effect");
        }
    }

    [Command]
    void Cmd_Destroy()
    {
        GameObject explo = Instantiate(ExplosionEffect);
        explo.transform.position = transform.position;
        explo.transform.rotation = transform.rotation;



        


        Detonator Det = explo.GetComponent<Detonator>();
        if (Det)
        {
            Det.direction = transform.rotation.eulerAngles;
        }

        NetworkServer.Spawn(explo);
        NetworkServer.Destroy(gameObject);
    }

    public void Damage(float Damage)
    {
        Life -= Damage;
    }



    void Update()
    {
        if (Life <= 0) { Cmd_Destroy(); }
    }
}
