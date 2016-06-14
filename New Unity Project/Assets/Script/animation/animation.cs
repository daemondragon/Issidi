using UnityEngine;
using System.Collections;

public class animation : MonoBehaviour {

    public Animator anim;
    private float inputH;
    private float inputV;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        anim.SetFloat("inputH", inputH);
        anim.SetFloat("inputV", inputV);

    }
}
