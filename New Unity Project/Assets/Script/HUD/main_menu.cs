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
    GameObject selct_map;
   
   

    // Use this for initialization
    void Start()
    {
        mainpanel = GameObject.Find("main");
        selct_map = GameObject.Find("selection_map");


        mainpanel.SetActive(true);
        selct_map.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    
    }
    public void open_slct()
    {
        selct_map.SetActive(true);
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
    public void retour()
    {
        selct_map.SetActive(false);
    }
   
    public void openMap2()
    {
        SceneManager.LoadScene(3);
    }
 

}
