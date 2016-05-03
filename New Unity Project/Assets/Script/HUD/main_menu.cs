using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class main_menu : MonoBehaviour
{

    GameObject mainpanel;
    GameObject coPanel;

    // Use this for initialization
    void Start()
    {
        mainpanel = GameObject.Find("main");
        coPanel = GameObject.Find("menu_connexion");
        coPanel.SetActive(false);
        mainpanel.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void openPanelco()
    {/*
        mainpanel.SetActive(false);
        coPanel.SetActive(true);
    */
        Application.LoadLevel("jeremy");
    }
    public void launchsolo()
    {/*
        mainpanel.SetActive(false);
        coPanel.SetActive(true);
    */
        //Application.LoadLevel(2);
        SceneManager.LoadScene(2);//SceneManager.LoadScene(2);
    }
    public void quit()
    {
        Application.Quit();
    }
    public void option()
    {

    }
}
