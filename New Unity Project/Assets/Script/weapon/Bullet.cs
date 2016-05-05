using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]

public class Bullet : NetworkBehaviour
{
    //If null, no explosion
    public GameObject explosion;

    public float speed;
    public float gravity;
    public float damage;

    private Vector3 gravity_vector;
    private Rigidbody body;
    private Stats.Team team;

    //if negative, infinite life_time (don't die if no collision)
    public float life_time;

    float TimeDestoy = -1;

    public float ZoneEffect;

    public void DelayedInitiate(Stats.Team team, Vector3 up)
    {
        body = GetComponent<Rigidbody>();
        body.velocity = transform.forward * speed;
        gravity_vector = gravity * up;

        enabled = hasAuthority;
        this.team = team;
    }

    // Update is called once per frame
    void Update()
    {
        if (body)
            body.velocity += gravity_vector * Time.deltaTime;

        if (life_time > 0.0f)
        {
            float delta_time = Time.deltaTime;
            if (delta_time > life_time)
                delta_time = life_time;
            life_time -= delta_time;
        }
        else if (life_time == 0.0f && hasAuthority)
        {
            Cmd_DestroyBullet();
        }
    }

    void ApplyDamages(Collider[] Colliders)
    {
        foreach (var collision in Colliders)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Stats stats = collision.gameObject.GetComponent<Stats>();
                if (stats)
                {
                    //We can't take damage if one of the two player is in Spectator team, or if we are in the same team
                    if (stats.team != Stats.Team.None && team != Stats.Team.None && team != stats.team)
                    {
                        if (ZoneEffect > 0)
                        {
                            stats.Life -= damage * (1 - (Vector3.Distance(transform.position, collision.transform.position) / ZoneEffect));
                        }
                        else
                        {
                            stats.Life -= damage;
                        }
                    }

                }
            }

            if (collision.gameObject.CompareTag("MurDestructible"))
            {
                CubeLife c = collision.gameObject.GetComponent<CubeLife>();
                if (c)
                {
                    if (ZoneEffect > 0)
                    {
                        c.Damage(damage * (1-(Vector3.Distance(transform.position, collision.transform.position) / ZoneEffect)));
                    }
                    else
                    {
                        c.Damage(damage);
                    }
                }
            }
        }
    }

    void PlayerReceiveBullet(Collision collision)
    {
        if (ZoneEffect > 0)
        {
            var nearby = Physics.OverlapSphere(transform.position, ZoneEffect);
            ApplyDamages(nearby);
        }
        else
        {
            Collider[] C = new Collider[1];
            C[0] = collision.collider;
            ApplyDamages(C);
        }





    }

    [Command]
    void Cmd_Boom()
    {
        if (explosion != null)
        {
            GameObject Explo = Instantiate(explosion);
            Explo.transform.position = this.transform.position;
            NetworkServer.Spawn(Explo);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasAuthority)
            return;

        DetachPartiles();
        Cmd_Boom();
        PlayerReceiveBullet(collision);
        Cmd_DestroyBullet();
    }

    //if the bullet spawn in the player, it will be this case and not OnCollisionEnter
    void OnCollisionStay(Collision collision)
    {
        if (!hasAuthority)
            return;

        Cmd_Boom();
        PlayerReceiveBullet(collision);
        Cmd_DestroyBullet();
    }

    [Command]
    void Cmd_DestroyBullet()
    {
        /*
         * Don't know what it is used for
        GameObject explo = Instantiate(template);
        explo.transform.position = transform.position;
        explo.transform.LookAt(r.contacts[0].normal);//r == collision in OnCollisionEnter
         */
       // Debug.Log("Destroyed bullet");
        Destroy(this.gameObject);
    }

    void DetachPartiles()
    {
        var t = transform.FindChild("parts");
        if (t != null)
        {
            var b = t.GetComponent<ParticleSystem>().emission;
            var en = b.enabled = false;
            t.parent = null;
            Destroy(t.gameObject, 4);
        }

    }
}
