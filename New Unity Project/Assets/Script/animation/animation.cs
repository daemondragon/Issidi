using UnityEngine;
using System.Collections;

public class animation : MonoBehaviour {

    public Animator anim;
    private float inputH;
    private float inputV;
    private bool dash;
    private Deplacement deplacement;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        deplacement = GetComponent<Deplacement>();
	}
	
	// Update is called once per frame
	void Update () {

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Keypad3))
            dash = true;
        if (Input.GetKeyUp(KeyCode.Keypad3))
            dash = false;
        //dash = deplacement.onDash();

        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);

    }
}
