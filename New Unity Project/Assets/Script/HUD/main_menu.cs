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
   
   

    // Use this for initialization
    void Start()
    {
        mainpanel = GameObject.Find("main");
       


        mainpanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    
    }
  
    public void openPanelco()
    {
        SceneManager.LoadScene("jeremy");
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
 

}
