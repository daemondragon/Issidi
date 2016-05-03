using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CubeLife : NetworkBehaviour
{
    public float MaximumLife = 100;
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
        NetworkServer.Spawn(explo);
        NetworkServer.Destroy(gameObject);
    }

    public void Damage(float Damage)
    {
        Debug.Log("damaged:" + Damage);
        Life -= Damage;
        Debug.Log("life:" + Life);
    }


    void Update()
    {
        if (Life <= 0) { Cmd_Destroy(); }
    }
}
