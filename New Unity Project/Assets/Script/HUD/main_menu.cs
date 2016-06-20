using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;


public class main_menu : MonoBehaviour
{

    GameObject mainpanel;
    GameObject coPanel;

    public bool isAtStartup = true;
    NetworkClient myClient;

    InputField[] inputs;
    InputField name;
    InputField address;
    InputField port;

    string ip_str;
    string port_str;
    string name_str;

    // Use this for initialization
    void Start()
    {
        mainpanel = GameObject.Find("main");
        coPanel = GameObject.Find("menu_connexion");
        coPanel.SetActive(false);
        mainpanel.SetActive(true);

        inputs = GameObject.Find("menu_connexion").GetComponentsInChildren<InputField>();
        name = inputs[0];
        address = inputs[1];
        port = inputs[2];

       name_str = "Unamed";
        ip_str = "127.0.0.1";
        port_str = "7777";
    }

    // Update is called once per frame
    void Update()
    {
        if (name_valid(name.text))
            name_str = name.text;
        if (address_valid(address.text))
            ip_str = address.text;
        if (port.text != "")
            port_str = port.text;
        
    }
    private bool name_valid(string s)
    {
        return s != "";
    }
    private bool address_valid(string s)
    {
        IPAddress osef;
        return s != "" && IPAddress.TryParse(s, out osef);
    }
    public void openPanelco()
    {
        mainpanel.SetActive(false);
        coPanel.SetActive(true);

        // SceneManager.LoadScene("jeremy");
    }
    public void launchsolo()
    {/*
        mainpanel.SetActive(false);
        coPanel.SetActive(true);
    */
        SceneManager.LoadScene(2);
    }
    public void quit()
    {
        Application.Quit();
    }
    public void option()
    {

    }
    // Create a server and listen on a port
    public void SetupServer()
    {
        NetworkServer.Listen(Convert.ToInt32(port_str));
        isAtStartup = false;
    }

    // Create a client and connect to the server port
    public void SetupClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.Connect(ip_str, Convert.ToInt32(port_str));
        isAtStartup = false;
    }

    // Create a local client and connect to the local server
    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        isAtStartup = false;
    }
    // client function
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
    }
    public void Create_join()
    {
     
            SetupServer();
            SetupLocalClient();

    }
    public void return2main()
    {
        coPanel.SetActive(false);
        mainpanel.SetActive(true);
    }
  
}
