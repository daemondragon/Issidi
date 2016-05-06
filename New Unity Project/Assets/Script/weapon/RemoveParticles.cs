using UnityEngine;
using System.Collections;

public class RemoveParticles : MonoBehaviour
{

    public float MaxDistance;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DontVanish();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLISION ENTER : " + collision.transform.name);
        Detach();
    }

    void DontVanish()
    {
        RaycastHit Hit;

        if (Physics.Raycast(transform.position, transform.forward, out Hit, MaxDistance))
        {
            Debug.Log("RAYCAST : " + Hit.transform.name);
            Detach();
        }
    }

    
}
