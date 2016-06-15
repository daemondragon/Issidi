using UnityEngine;
using System.Collections;

public class animation : MonoBehaviour {

    public Animator anim;
    private float inputH;
    private float inputV;
    private bool dash;
    private bool jump;
    private Deplacement deplacement;
    private bool doubleJump;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        deplacement = GetComponent<Deplacement>();
        doubleJump = false;
	}
	
	// Update is called once per frame
	void Update () {

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
    
        //dash = deplacement.onDash();
        if (dash)
            anim.Play("Armature|dash" ,- 1, 0f);
        if (jump)
            anim.Play("Armature|Jump", -1, 0f);

        if (doubleJump)
            anim.Play("Armature|jump");
        anim.SetBool("dash", dash);
        anim.SetBool("jump", jump);
        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);

    }
}
