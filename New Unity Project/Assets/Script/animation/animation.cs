using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation_Info))]
public class animation : MonoBehaviour
{

    public Animator anim;
    private float inputH;
    private float inputV;
    private Animation_Info anim_info;
    private bool idling;
    private float inactiveTime;
    private bool celebration;
    private float dash_timer;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        anim_info = GetComponent<Animation_Info>();
        idling = false;
        inactiveTime = 0;
        celebration = false;
    }

    // Update is called once per frame
    void Update()
    {

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

        if (anim_info.on_dash)
            dash_timer += Time.deltaTime;
        else
            dash_timer = 0;
        if (dash_timer >= 0.9)
            anim_info.on_dash = false;

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");

        //dash = deplacement.onDash();
        // jump = deplacement.onJump()

        #region test

        if (Input.GetKeyDown(KeyCode.E))
            anim_info.on_dash = true;
        if (Input.GetKeyUp(KeyCode.E))
            anim_info.on_dash = false;

        if (Input.GetKeyDown(KeyCode.R))
            anim_info.jumping = true;
        if (Input.GetKeyUp(KeyCode.S))
            anim_info.jumping = false;

        anim_info.double_jump = Input.GetKeyDown(KeyCode.Y);
        anim_info.shot = Input.GetKey(KeyCode.T);
        #endregion test

        anim.SetBool("Djump", anim_info.double_jump);
        anim.SetBool("dash", anim_info.on_dash);
        anim.SetBool("jump", anim_info.jumping);
        anim.SetBool("shooting", anim_info.shot);
        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);
        anim.SetBool("idling", idling);
        anim.SetBool("celebration", celebration);
    }
}
