using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Deplacement))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NetworkOwner))]

public class FootstepSound : MonoBehaviour
{
    /*
    ** Loop every sound found in footstep_sound if walking
    */
    public List<AudioClip> footstep_sound;
    private AudioSource audio_source;
    private Deplacement deplacement;
    private int actual_sound;
    private NetworkOwner net;

	// Use this for initialization
	void Start () {
        deplacement = GetComponent<Deplacement>();
        audio_source = GetComponent<AudioSource>();
        net = GetComponent<NetworkOwner>();
        actual_sound = 0;

        GetComponent<AudioListener>().enabled = net.IsMine();
        enabled = net.IsMine();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (footstep_sound.Count > 0 && !audio_source.isPlaying && deplacement.IsWalking())
        {
            audio_source.clip = footstep_sound[actual_sound];
            audio_source.Play();

            //Loop to next sound
            actual_sound = (actual_sound + 1) % footstep_sound.Count;
        }
	}
}
