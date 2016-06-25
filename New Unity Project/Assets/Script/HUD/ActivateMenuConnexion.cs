using UnityEngine;
using System.Collections;

public class ActivateMenuConnexion : MonoBehaviour
{
    public GameObject Menu_connexion;
	// Use this for initialization
	void Start ()
    {
        if (!Menu_connexion)
            Debug.Log("Error in ActivateMenuConnexion: no Menu_connexion attached");
        else
            Activate();
	}
	
    public void Activate(bool b = true)
    {
        if (Menu_connexion)
            Menu_connexion.SetActive(b);
    }
}
