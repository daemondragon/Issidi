using UnityEngine;
using UnityEngine.Networking;

public class Stats : NetworkBehaviour
{
    private ColorModifier[] colors = null;

    public enum Team
    {
        None = 0,
        Blue = 1,
        Orange = 2
    }
    [SyncVar]
    public string Name;
    [SyncVar]
    public bool paused;
    #region GetterSetter

    [SyncVar]
    private Team _team = Team.None;
    public Team team
    {
        get { return (_team); }
        set
        {
            if (colors == null)
                Start();
            else if (colors.Length <= 0)
                colors = GetComponentsInChildren<ColorModifier>(); ;

            _team = value;
            foreach (ColorModifier col in colors)
            {
                col.SetTeam(value);
            }

        }
    }
    [SyncVar]
    private float max_life;
    [SyncVar]
    private float life;
    public float MaxLife
    {
        get { return (max_life); }
        set
        {
            if (value < 0.0f)
                value = 0.0f;
            max_life = value;
        }
    }
    public float Life
    {
        get { return (life); }
        set
        {
            if (value > MaxLife)
                value = MaxLife;
            else if (value < 0.0f)
                value = 0.0f;
            life = value;
        }
    }

    //For the dash
    [SyncVar]
    private float max_mana;
    [SyncVar]
    private float mana;
    [SyncVar]
    private float mana_per_dash;
    public float MaxMana
    {
        get { return (max_mana); }
        set
        {
            if (value < 0.0f)
                value = 0.0f;
            max_mana = value;
        }
    }
    public float Mana
    {
        get { return (mana); }
        set
        {
            if (value > MaxMana)
                value = MaxMana;
            else if (value < 0.0f)
                value = 0.0f;
            mana = value;
        }
    }
    public float ManaPerDash
    {
        get { return (mana_per_dash); }
        set
        {
            if (value < 0.0f)
                value = 0.0f;
            mana_per_dash = value;
        }
    }

    [SyncVar]
    public float mana_per_second;

    [SyncVar]
    private int max_ammo;
    [SyncVar]
    private int ammo;
    public int MaxAmmo
    {
        get { return (max_ammo); }
        set
        {
            if (value < 0)
                value = 0;
            max_ammo = value;
        }
    }
    public int Ammo
    {
        get { return (ammo); }
        set
        {
            if (value > MaxAmmo)
                value = MaxAmmo;
            else if (value < 0)
                value = 0;
            ammo = value;
        }
    }

    public string WeaponName
    {
        get
        {
            Weapon w = GetComponent<Weapon>();
            if (w)
                return (w.Name);
            else
                return ("default");
        }
        private set
        {
            Weapon w = GetComponent<Weapon>();
            if (w)
                w.Name = value;
        }
    }

    #endregion

    void Start()
    {
        colors = GetComponentsInChildren<ColorModifier>();
        if (colors.Length <= 0)
            Debug.Log("Error in Character prefab : no ColorModifier found in " + this);

        MaxLife = 100.0f;
        Life = MaxLife;
        MaxMana = 100.0f;
        Mana = MaxMana;
        mana_per_second = MaxMana / 15.0f;//Mana full in 15 second 
        ManaPerDash = 30;
        Name = "Player";
        paused = false;
    }

    [Command]
    void Cmd_DestroyPlayer(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        Mana += mana_per_second * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
            paused = !paused;

        if (IsDead())
        {
            CameraDeplacement cam = GetComponentInChildren<CameraDeplacement>();
            if (cam)
            {
                cam.ActivateCamera(false);
            }
            else
                Debug.Log("No cam found");

            
            Cmd_DestroyPlayer(gameObject);
        }
    }

    public bool IsDead()
    {
        return (life <= 0.0f);
    }

    public float LifeRatio()
    {
        return (life / MaxLife);
    }

    public float ManaRatio()
    {
        return (mana / max_mana);
    }

    public float AmmoRatio()
    {
        return (ammo * 1.0f / max_ammo);
    }

    public bool CanDash()
    {
        return Mana >= ManaPerDash;
    }

    public void Dash()
    {
        Mana -= ManaPerDash;
    }

    public bool CanShoot()
    {
        return (ammo > 0);
    }

    public void Shoot()
    {
        Ammo--;
    }
}
