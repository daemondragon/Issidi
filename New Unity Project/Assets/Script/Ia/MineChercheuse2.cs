using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MineChercheuse2 : NetworkBehaviour
{
    float timeLeft = 10.0f;
    GameObject game_object;
    public float speed;
    public float turn;
    public float damages;

    void Start()
    {
        game_object = gameObject;
    }



    void Update()
    {
        if (timeLeft > 10)
        {
            timeLeft -= Time.deltaTime;
        }
        else if (timeLeft < 0)
        {
            damage();
            Destroy(game_object);
        }
        else
        {
            timeLeft -= Time.deltaTime;
            GameObject Player = GameObject.FindGameObjectWithTag("Player");
            if (Player != null)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
                GameObject closest = targets[0];
                float closestDist = Mathf.Infinity;
                foreach (GameObject Target in targets)
                {
                    float dist = (transform.position - Target.transform.position).sqrMagnitude;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = Target;
                    }
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closest.transform.position - transform.position), turn * Time.deltaTime);
                transform.position += transform.forward * speed * Time.deltaTime;
                if (false)
                {

                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        damage();
        Destroy(game_object);
    }


    void damage()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject Target in targets)
            {
                float dist = (transform.position - Target.transform.position).magnitude;
                if (dist < 50)
                {
                    Stats stats = Target.GetComponent<Stats>();
                    stats.Life -= damages;
                }
            }
        }
    }
}
