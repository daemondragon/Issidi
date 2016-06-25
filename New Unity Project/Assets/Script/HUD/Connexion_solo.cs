using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Connexion_solo : NetworkBehaviour
{
    // Use this for initialization
    void Start()
    {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().StartHost();
        gameObject.SetActive(true);
    }
}
