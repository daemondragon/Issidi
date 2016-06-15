using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Automatically destroys the GameObject when there are no children left.
/// </summary>

public class CFX_AutodestructWhenNoChildren : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            if (GetComponent<NetworkIdentity>())
            {
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}
