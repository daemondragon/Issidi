using System;
using System.Net;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUD_connexion : NetworkBehaviour
{
    Text player_name;
    GameObject wrong_port;
    Text port;
    GameObject wrong_adress;
    Text adress;

    Button join_button;
    Button create_button;

    public GameObject HUD_Player;

    // Use this for initialization
    void Start()
    {
        wrong_port = GameObject.Find("wrong_port");
        port = GameObject.Find("port_input").GetComponentsInChildren<Text>()[1];
        wrong_adress = GameObject.Find("wrong_adress");
        adress = GameObject.Find("ip_input").GetComponentsInChildren<Text>()[1];
        player_name = GameObject.Find("name_input").GetComponentsInChildren<Text>()[1];

        join_button = GameObject.Find("join").GetComponent<Button>();
        create_button = GameObject.Find("create").GetComponent<Button>();

        if (!HUD_Player)
            Debug.Log("no HUD attached in HUD_connexion");
    }

    // Update is called once per frame
    void Update()
    {
        bool valid_adress = adress.text == "" || address_valid(adress.text);
        bool valid_port = port.text == "" || Convert.ToUInt16(port.text) > 1024;

        wrong_adress.SetActive(!(adress.text == "" || valid_adress));
        wrong_port.SetActive(!valid_port);

        join_button.interactable = valid_adress && valid_port;
        create_button.interactable = valid_port;
    }

    public void CreateParty()
    {
        NetworkManager net_manager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        net_manager.networkPort = port.text == "" ? (ushort)7777 : Convert.ToUInt16(port.text);
        net_manager.StartHost();

        if (HUD_Player)
            HUD_Player.GetComponent<HUD_player>().Name = (player_name.text == "" ? "A robot has no name" : player_name.text);

        gameObject.SetActive(false);
    }

    public void JoinParty()
    {
        NetworkManager net_manager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        net_manager.networkAddress = (adress.text == "" ? "127.0.0.1" : adress.text);
        net_manager.networkPort = port.text == "" ? (ushort)7777 : Convert.ToUInt16(port.text);
        net_manager.StartClient();

        if (HUD_Player)
            HUD_Player.GetComponent<HUD_player>().Name = (player_name.text == "" ? "A robot has no name" : player_name.text);

        gameObject.SetActive(false);
    }


    private bool address_valid(string s)
    {
        IPAddress osef;
        return s != "" && IPAddress.TryParse(s, out osef) && s.Contains(".");
    }
}
