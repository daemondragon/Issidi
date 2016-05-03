using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkIdentity))]

public class BulletIA : NetworkBehaviour
{

    //If null, no explosion
    public GameObject explosion;

    public float speed;
    public float gravity;
    public float damage;

    private Vector3 gravity_vector;
    private Rigidbody body;

    //if negative, infinite life_time (don't die if no collision)
    public float life_time;

    public void DelayedInitiate(Stats.Team team, Vector3 up)
    {
        body = GetComponent<Rigidbody>();
        body.velocity = transform.forward * speed;
        gravity_vector = gravity * up;

        enabled = hasAuthority;
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
        else if (life_time == 0.0f && !hasAuthority)
        {
            Cmd_DestroyBullet();
        }
    }

    void PlayerReceiveBullet(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Stats stats = collision.gameObject.GetComponent<Stats>();
            if (stats)
            {
                stats.Life -= damage;
            }

        }
    }

    //[Command]
    void Cmd_Boom()
    {
        if (explosion != null)
        {
            GameObject Explo = Instantiate(explosion);
            Explo.transform.position = transform.position;
            NetworkServer.Spawn(Explo);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasAuthority)
            return;

        Cmd_Boom();
        PlayerReceiveBullet(collision);
        Cmd_DestroyBullet();
    }

    //[Command]
    void Cmd_DestroyBullet()
    {
        /*
         * Don't know what it is used for
        GameObject explo = Instantiate(template);
        explo.transform.position = transform.position;
        explo.transform.LookAt(r.contacts[0].normal);//r == collision in OnCollisionEnter
         */
        Debug.Log("Destroyed bullet");
        Destroy(this.gameObject);
    }

}
