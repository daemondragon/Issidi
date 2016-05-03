using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(Stats))]

public class Weapon : NetworkBehaviour
{
    public string Name;

    //weapon Setting
    public bool activate;
    public bool Automatic;
    public float Rate; // b.s-1

    private bool Pressed;
    private float lastshoot;

    public GameObject bullet_template;//Bullet to instantiate
    private Stats stats;
    private WeaponOrientation weapon;

    public int nb_bullets;
    public float bullet_per_second;
    private float temp_bullet;

    public Transform BulletStart;

    void Start()
    {
        enabled = isLocalPlayer;

        weapon = GetComponentInChildren<WeaponOrientation>();
        activate = true;
        stats = GetComponent<Stats>();
        temp_bullet = 0.0f;
        stats.MaxAmmo = nb_bullets;
        stats.Ammo = stats.MaxAmmo;

        if (!weapon)
            Debug.Log("Error in character prefab : no weapon orientation found in " + this);
    }

    [Command]
    void Cmd_InstantiateBullet(Vector3 position, Quaternion rotation, Vector3 up)
    {
        GameObject bullet = (GameObject)Instantiate(bullet_template, position, rotation);
        if (!bullet)
            return;

        NetworkServer.Spawn(bullet);

        //Set the velocity
        Bullet script = bullet.GetComponent<Bullet>();
        if (script)
            script.DelayedInitiate(stats.team, up);
        else
            Debug.Log("bullet doesn't have Bullet script in " + this);
    }

    private void Shoot()
    {
        if (!isLocalPlayer || !weapon)
            return;

       // Cmd_InstantiateBullet(weapon.getPosition() + weapon.getForward(), weapon.getRotation(), transform.up);

        Cmd_InstantiateBullet(BulletStart.position, BulletStart.rotation, transform.up);

        if (stats)
            stats.Shoot();//Lose ammo
    }

    void Update()
    {
        if (stats.IsDead() || stats.paused)
            return;

        temp_bullet += bullet_per_second * Time.deltaTime;
        while (temp_bullet >= 1.0f)
        {
            temp_bullet -= 1.0f;
            stats.Ammo++;
        }

        if (!(activate && stats.CanShoot()))
            return;

        if (Input.GetButton("Fire1"))
        {
            if (Automatic)
            {
                if (Time.time - lastshoot > 1 / Rate)
                {
                    lastshoot = Time.time;
                    Shoot();
                }
            }
            else if (!Pressed && Time.time - lastshoot > 1 / Rate)
            {
                lastshoot = Time.time;
                Pressed = true;
                Shoot();
            }
        }
        else
        {
            Pressed = false;
        }
    }
}