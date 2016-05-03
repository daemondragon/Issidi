using UnityEngine;
using UnityEngine.Networking;

public class NetworkOwner : NetworkBehaviour
{
	public bool IsMine()
    {
        return (isLocalPlayer);
    }
}
