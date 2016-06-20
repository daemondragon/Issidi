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
    GameObject croix_rouge_A;
    GameObject croix_rouge_P;

    public bool isAtStartup = true;
    NetworkClient myClient;

    InputField[] inputs;
    InputField Name;
    InputField address;
    InputField port;

    Button[] buttons;

    string ip_str;
    string port_str;
    string name_str;

    // Use this for initialization
    void Start()
    {
        mainpanel = GameObject.Find("main");
        coPanel = GameObject.Find("menu_connexion");


        buttons = GameObject.Find("menu_connexion").GetComponentsInChildren<Button>();
        croix_rouge_A = GameObject.Find("wrong_adress");
        croix_rouge_P = GameObject.Find("wrong_port");



        Name = GameObject.Find("name_lbl").GetComponentInChildren<InputField>();
              address = GameObject.Find("ip_lbl").GetComponentInChildren<InputField>();
        port = GameObject.Find("port_lbl").GetComponentInChildren<InputField>();

        name_str = "Unamed";
        ip_str = "127.0.0.1";
        port_str = "7777";

        coPanel.SetActive(false);
        mainpanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        bool c1 = Name.text != "";
        bool c2 = address_valid(address.text);
        int port_int = 0;



        if (c1)
            name_str = Name.text;
        if (c2)
        {
            ip_str = address.text;
            croix_rouge_A.SetActive(false);
        }
        else if (address.text != "")
            croix_rouge_A.SetActive(true);
        else
            croix_rouge_A.SetActive(false);


        bool c3 = true;
        if (port.text != "")
        {
            port_int = Convert.ToUInt16(port.text);

            c3 = port_int < 65536 && port_int > 1024;
            port_str = port.text;
        }

        if (c3)
        {
            
            croix_rouge_P.SetActive(false);
        }
        else if (port.text != "")
            croix_rouge_P.SetActive(true);
        else
            croix_rouge_P.SetActive(false);



        button_set((c2 || address.text == "")&& c3 && address.text!="");
    }
    private void button_set(bool b)
    {

        buttons[1].interactable = b;

    }

    private bool address_valid(string s)
    {
        IPAddress osef;
        return s != "" && IPAddress.TryParse(s, out osef) && s.Contains(".");
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
        Debug.Log(port_str);
        NetworkServer.Listen(Convert.ToUInt16(port_str));
        isAtStartup = false;
    }

    // Create a client and connect to the server port
    public void SetupClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.Connect(ip_str, Convert.ToUInt16(port_str));
        isAtStartup = false;
    }

    // Create a local client and connect to the local server
    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
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
        SceneManager.LoadScene(1);

    }
    public void return2main()
    {
        coPanel.SetActive(false);
        mainpanel.SetActive(true);
    }

}
