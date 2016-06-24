using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class test : NetworkBehaviour
{
    public float distanceVision = 30f;
    float timeLeft = 2.0f;
    float timeLeftCurrent;
    public float entre_attaque = 0.3f;
    float entre_attaque_Current;
    bool Detecter = false;
    float DistanceJoueur;
    Vector3 DirectionJoueur; // Le vecteur ayant pour origin la tourelle et pour direction la position du joueur
    RaycastHit Toucher;
    public GameObject proj;
    public int power = 100;

    public List<AudioClip> tire_sound;
    private AudioSource audio_source;
    private int actual_sound;

    Stats stat;
    public GameObject StartBullet;


    void Start()
    {
        entre_attaque_Current = entre_attaque;
        timeLeftCurrent = timeLeft;
        actual_sound = 0;
        stat = transform.root.GetComponent<Stats>();
    }

    // Update est appelée une foi par frame
    void Update()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            DistanceJoueur = Vector3.Distance(StartBullet.transform.position, Player.transform.position); // Calcul de la distance entre la tourelle et le joueur

            Detecter = (DistanceJoueur < distanceVision) ? Detection() : false; // Si le joueur est assez près, on verifie s'il est vu

            if (Detecter)
            {
                if (timeLeftCurrent > 0)
                {
                    Rotation();
                    timeLeftCurrent -= Time.deltaTime;
                }
                else
                {
                    Rotation();
                    Attaque();
                }
            }
            else
            {
                timeLeftCurrent = timeLeft;
            }
        }
    }

    // Fonction servant à gérer la detection de l'IA
    bool Detection()
    {
        DirectionJoueur = GameObject.FindGameObjectWithTag("Player").transform.position - this.transform.position;

        Physics.Raycast(this.transform.position, DirectionJoueur, out Toucher, distanceVision); // On tire un rayon vers le joueur
        return (Toucher.transform.tag == "Player"); // Si le rayon touche le joueur, il est vu (sinon, il a touché un mur)
    }

    //fonction servant a diriger la tourelle vers le joueur
    void Rotation()
    {
        Quaternion rotate = Quaternion.LookRotation(DirectionJoueur);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotate, Time.deltaTime * 2f);
        //this.transform.LookAt(DirectionJoueur);
    }

    void Attaque()
    {
        if (entre_attaque_Current > 0.0f)
        {
            entre_attaque_Current -= Time.deltaTime;
            //Debug.Log(entre_attaque_Current);
        }
        else
        {
            GameObject balle = (GameObject)Instantiate(proj, StartBullet.transform.position, Quaternion.identity);           
            NetworkServer.Spawn(balle);
            balle.GetComponent<Rigidbody>().AddForce(DirectionJoueur * power);
            balle.GetComponent<Bullet>().SetTower();            
            entre_attaque_Current = entre_attaque;      

        }
    }    

}
