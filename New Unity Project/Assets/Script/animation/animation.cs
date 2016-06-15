using UnityEngine;
using System.Collections;

public class animation : MonoBehaviour {

    public Animator anim;
    private float inputH;
    private float inputV;
    private bool dash;
    private bool jump;
    private Deplacement deplacement;
    private float doubleJump;
    private bool keepJumping;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        deplacement = GetComponent<Deplacement>();
        doubleJump = 0f ;
        keepJumping = true;
	}
	
	// Update is called once per frame
	void Update () {

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");

        //dash = deplacement.onDash();
        // jump = deplacement.onJump()


        if (Input.GetKeyDown(KeyCode.E))
            dash = true;
        if (Input.GetKeyUp(KeyCode.E))
            dash = false;

        if (Input.GetKeyDown(KeyCode.R))
            jump = true;
        if (Input.GetKeyUp(KeyCode.S))
            jump = false;

        if (jump && Input.GetKeyDown(KeyCode.Y) && keepJumping)
        {
            doubleJump++;
            Debug.Log(keepJumping);
        }
        anim.SetFloat("Djump", doubleJump);
        anim.SetBool("dash", dash);
        anim.SetBool("jump", jump);
        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);

        if (doubleJump != 0)
        {
            keepJumping = false;
            doubleJump = 0;
        }
    }
}
