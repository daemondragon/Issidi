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
    public GameObject character_factory;

    #region GetterSetter
    [SyncVar]
    public bool need_respawn;
    [Command]
    void Cmd_SetNeedRespawn(bool b)
    {
        need_respawn = b;
    }

    [SyncVar]
    string _name;
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    [SyncVar]
    bool can_move_player;
    public bool CanMovePlayer
    {
        get { return can_move_player; }
        set { can_move_player = value; }
    }

    [SyncVar]
    Team _team;
    public Team team
    {
        get { return (_team); }
        set
        {
            _team = value;
            RecolorPlayer();
        }
    }

    [SyncVar]
    float max_life;
    [SyncVar]
    float life;

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
    float max_energy;
    [SyncVar]
    float energy;
    [SyncVar]
    float energy_per_dash;
    [SyncVar]
    float energy_per_second;

    public bool can_dash_in_multiple_direction;

    public float MaxEnergy
    {
        get { return (max_energy); }
        set
        {
            if (value < 0.0f)
                value = 0.0f;
            max_energy = value;
        }
    }
    public float Energy
    {
        get { return (energy); }
        set
        {
            if (value > MaxEnergy)
                value = MaxEnergy;
            else if (value < 0.0f)
                value = 0.0f;
            energy = value;
        }
    }
    public float EnergyPerDash
    {
        get { return (energy_per_dash); }
        set
        {
            if (value < 0.0f)
                value = 0.0f;
            energy_per_dash = value;
        }
    }
    public float EnergyPerSecond
    {
        get { return (energy_per_second); }
        set { energy_per_second = value; }
    }

    [SyncVar]
    int max_ammo;
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
        MaxEnergy = 100.0f;
        Energy = MaxEnergy;
        EnergyPerSecond = MaxEnergy / 15.0f;//Mana full in 15 second 
        EnergyPerDash = 30;
        Name = "Player";
        can_move_player = true;
        can_dash_in_multiple_direction = true;
    }

    public void RecolorPlayer()
    {
        if (colors == null)
            Start();
        else if (colors.Length <= 0)
            colors = GetComponentsInChildren<ColorModifier>(); ;

        foreach (ColorModifier col in colors)
        {
            col.SetTeam(_team);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        Energy += EnergyPerSecond * Time.deltaTime;

        if (need_respawn)
        {
            ReturnToSpawn();
            Cmd_SetNeedRespawn(false);
        }

        if (IsDead())
            KillPlayer();

    }

    #region spawning

    public void ReturnToSpawn()
    {
        GameObject[] spawns = null;

        if (team == Team.None)
            spawns = GameObject.FindGameObjectsWithTag("WhiteSpawn");
        else if (team == Team.Blue)
            spawns = GameObject.FindGameObjectsWithTag("BlueSpawn");
        else if (team == Team.Orange)
            spawns = GameObject.FindGameObjectsWithTag("OrangeSpawn");

        if (spawns == null || spawns.Length <= 0)
        {
            transform.position = new Vector3(0, 0, 0);
            Debug.Log("Error : no " + team + " spawn register in CharacterFactory, player will spawn at (0;0;0)");
        }
        else //Spawn at a random location
        {
            Spawner spawn = spawns[Random.Range(0, spawns.Length)].GetComponent<Spawner>();
            if (spawn)
            {
                transform.position = spawn.getSpawnPosition(gameObject);
                transform.rotation = spawn.getSpawnRotation(gameObject);
                Deplacement deplacement = GetComponent<Deplacement>();
                if (deplacement)
                {
                    deplacement.CorrectRotation();
                    deplacement.sens = deplacement.findGlobalAxis(transform.up);
                    deplacement.SetOnGround(true);
                }
            }
        }
    }

    [Command]
    void Cmd_CreateCharacterFactory()
    {
        if (character_factory)
        {
            GameObject temp = Instantiate(character_factory);
            NetworkServer.ReplacePlayerForConnection(connectionToClient, temp, playerControllerId);
        }
        else
            Debug.Log("no character factory attached to " + this + ". Player can't respawn");
    }

    public void KillPlayer()
    {
        Cmd_CreateCharacterFactory();
        Cmd_DestroyPlayer(gameObject);
    }

    [Command]
    void Cmd_DestroyPlayer(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    #endregion

    #region utils

    public bool IsDead()
    {
        return (life <= 0.0f);
    }

    public float LifeRatio()
    {
        return (life / MaxLife);
    }

    public float EnergyRatio()
    {
        return (Energy / MaxEnergy);
    }

    public float AmmoRatio()
    {
        return (ammo * 1.0f / max_ammo);
    }

    public bool CanDash()
    {
        return Energy >= EnergyPerDash;
    }

    public void Dash()
    {
        Energy -= EnergyPerDash;
    }

    public bool CanShoot()
    {
        return (ammo > 0);
    }

    public void Shoot()
    {
        Ammo--;
    }

    #endregion
}
