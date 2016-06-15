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
    private bool idling;
    private float inactiveTime;
    private bool celebration;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        deplacement = GetComponent<Deplacement>();
        doubleJump = 0f ;
        keepJumping = true;
        idling = false;
        inactiveTime = 0;
        celebration = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.anyKeyDown)
        {
            inactiveTime = 0;
            idling = false;
            if (celebration)
            celebration = false;
        }
        inactiveTime += Time.deltaTime;
        
        if (inactiveTime >= 3.0)
            idling = true;
        if ((Input.GetKeyDown(KeyCode.V)))
            celebration = true;

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
                    }
        anim.SetFloat("Djump", doubleJump);
        anim.SetBool("dash", dash);
        anim.SetBool("jump", jump);
        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);
        anim.SetBool("idling", idling);
        anim.SetBool("celebration", celebration);

        if (doubleJump != 0)
        {
            keepJumping = false;
            doubleJump = 0;
        }
        if (jump == false)
            keepJumping = true;
    }
}
