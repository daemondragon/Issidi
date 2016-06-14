using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Teleporter : NetworkBehaviour
{
    public GameObject spawner;
    Spawner spawn;

	// Use this for initialization
	void Start () {
        UpdateSpawn();
	}
	
    void UpdateSpawn()
    {
        if (spawner)
            spawn = spawner.GetComponent<Spawner>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && spawner)
        {
            if (!spawn)
                UpdateSpawn();

            if (!spawn)
                Debug.Log("no spawner in GameObject: " + spawner);
            else
                col.gameObject.GetComponent<Stats>().PlaceToSpawn(spawn);
        }
    }
}
