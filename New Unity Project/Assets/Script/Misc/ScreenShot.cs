using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ScreenShot : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        enabled = hasAuthority;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.F5))
        {
            string filename = "Issidi:" + System.DateTime.Now.ToString();
            Application.CaptureScreenshot(filename);
        }
	}
}
