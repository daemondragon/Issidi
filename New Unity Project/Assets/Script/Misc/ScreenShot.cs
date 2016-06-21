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
	    if (Input.GetKeyDown(KeyCode.F3))
        {
            string filename = "Issidi_(" + System.DateTime.Now.ToLongDateString() + " "
                    + System.DateTime.Now.Hour + "h "
                    + System.DateTime.Now.Minute + "m " 
                    + System.DateTime.Now.Second + "s )" + ".png";
            Application.CaptureScreenshot(filename);
        }
	}
}
