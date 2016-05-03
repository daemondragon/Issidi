using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    private NetworkClient client;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CreateRoom(int port = 15312)
    {
        NetworkServer.Listen(port);
        client = ClientScene.ConnectLocalServer();
        client.RegisterHandler(MsgType.Connect, delegate { Debug.Log("Local client connected"); });
    }

    public void JoinRoom(string ip = "127.0.0.1", int port = 15312)
    {
        client = new NetworkClient();
        client.RegisterHandler(MsgType.Connect, delegate { Debug.Log("Client connected"); });
        client.Connect(ip, port);
    }
}
