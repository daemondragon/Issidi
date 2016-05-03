using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HUD_player1 : NetworkBehaviour
{
    private Stats stats;//The stats to render
    private Image healthbar;
    private Image nrjbar;
    private Image ammo_count;

    GameObject stats_bars;
    GameObject weapon_panel;
   
    GameObject pause_menu;
    GameObject death_panel;
    GameObject select_panel;
    GameObject crooshair;
    Text scoreB;
    int scoreb;
    Text scoreO;
    int scoreo;
    Image score_blue;
    Image score_orange;

    CameraDeplacement cam;
    private Button btn_continue;

    GameObject playbtn;
    GameObject selectionpanel;
    GameObject weaponpanel;

    GameObject selectbtnO;
    GameObject selectbtnS;
    GameObject selectbtnB;


    //Pour récuperer le bon character
    bool have_find;
    GameObject player;
    bool can_grap_player;

    int team = -1; // 1 pour orange , 0 pour spectat, 2 pour bleu
    int weaponType = -1; // dans l ordre snip, lance-flame , obus de 1 a 3

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(hasAuthority);
        enabled = hasAuthority;
        Debug.Log(hasAuthority);

        if (!hasAuthority)
            return;

        have_find = false;
        can_grap_player = true;

        stats_bars = GameObject.Find("stats_bars");
        if (stats_bars)
        {
            Image[] bars = stats_bars.GetComponentsInChildren<Image>();
            healthbar = bars[1];
            nrjbar = bars[2];
        }

        weapon_panel = GameObject.Find("weapon_panel");
        if (weapon_panel)
        {
            Text[] weapon_name = weapon_panel.GetComponentsInChildren<Text>();
            weapon_name[1].text = "default";//Merci de toujours me parler de tes couilles, comme ça, je corrige des bugs simplement (fuck you unity)

            Image[] image_ammo = weapon_panel.GetComponentsInChildren<Image>();
            ammo_count = image_ammo[1];
        }

        // pause gestion
        pause_menu = GameObject.Find("pause_panel");
        if (pause_menu)
            pause_menu.SetActive(false);

        // death gestion
        death_panel = GameObject.Find("death_panel");
        if (death_panel)
            death_panel.SetActive(false);

        // score gestion
      


        //select perso gestion
        playbtn = GameObject.Find("play_btn");
        selectbtnO = GameObject.Find("Oselect");
        selectbtnS = GameObject.Find("Sselect");
        selectbtnB = GameObject.Find("Bselect");

        selectbtnO.SetActive(false);
        selectbtnS.SetActive(false);
        selectbtnB.SetActive(false);

        playbtn.SetActive(false);


        select_panel = GameObject.Find("selec_perso");
        crooshair = GameObject.Find("crosshair");

        stats_bars.SetActive(false);
      
        weapon_panel.SetActive(false);
        crooshair.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Initialisation(GameObject character)
    {
        stats = character.GetComponentInChildren<Stats>();//init des stats
        cam = character.GetComponentInChildren<CameraDeplacement>();
        have_find = true;
        stats.paused = false;

        player = character;

        Text[] weapon_name = weapon_panel.GetComponentsInChildren<Text>();
        weapon_name[1].text = stats.WeaponName; // insert weapon name to be fair to other oponent else they stand no chance!
    }

    // Update is called once per frame
    void Update()
    {
        if (team != -1 && weaponType != -1)
            playbtn.SetActive(true);

        if (have_find)
            DrawUpdate();
        else
        {
            if (!can_grap_player)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }

            //Try to find the good player
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                obj.GetComponent<Stats>().team = obj.GetComponent<Stats>().team;
                //parce qu'il est désactivé s'il n'est pas à nous
                NetworkOwner net = obj.GetComponent<NetworkOwner>();
                if (net && net.IsMine())
                {
                    Debug.Log(obj);
                    Initialisation(obj);
                    break;
                }
            }
        }

    }

    void DrawUpdate()
    {
        if (stats)
        {
            healthbar.fillAmount = stats.LifeRatio();
            Text pdv = healthbar.GetComponentInChildren<Text>();
            pdv.text = ((int)(Mathf.Ceil(stats.Life))).ToString() + "/" + stats.MaxLife.ToString();

            nrjbar.fillAmount = stats.Mana / stats.MaxMana;
            Text point_nrj = nrjbar.GetComponentInChildren<Text>();
            point_nrj.text = ((int)(stats.Mana)).ToString() + "/" + ((int)(stats.MaxMana)).ToString();

            ammo_count.fillAmount = (float)stats.Ammo / (float)stats.MaxAmmo;
            Text amo = ammo_count.GetComponentInChildren<Text>();
            amo.text = stats.Ammo.ToString() + "/" + stats.MaxAmmo.ToString();

            // test
            if (Input.GetKeyDown(KeyCode.T))
            {
                stats.Life -= 20;
                stats.Mana -= 30;
                stats.Ammo -= 2;
                scoreo += 100;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                stats.Life += 10;
                scoreb++;
            }

            if (stats.IsDead())
            {
                death_panel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                have_find = false;
            }
            else
            {
                Cursor.visible = stats.paused || !have_find;
                if (Cursor.visible)
                    Cursor.lockState = CursorLockMode.None;
                pause_menu.SetActive(stats.paused);
                if (stats.paused)
                {
                    player.GetComponentInChildren<CameraDeplacement>().ActivateCamera(false);
                }
            }
        }

        if (scoreo == scoreb && scoreb == 0)
        {
            score_orange.fillAmount = 0.5f;
            score_blue.fillAmount = 0.5f;
        }
        else
        {
            score_orange.fillAmount = (float)scoreo / (float)(scoreb + scoreo);
            score_blue.fillAmount = (float)scoreb / (float)(scoreb + scoreo);
        }

        scoreB.text = scoreb.ToString();
        scoreO.text = scoreo.ToString();
    }

    public void unpause()
    {
        stats.paused = false;
        player.GetComponentInChildren<CameraDeplacement>().ActivateCamera(true);
    }

    public void quit()
    {
        Application.Quit();
    }
    public void return2main()
    {
        Application.LoadLevel("menu_pricipal");
    }
    public void resume()
    {
        pause_menu.SetActive(false);
    }

    [Command]
    void Cmd_DestroyPlayer(GameObject obj)
    {
        NetworkServer.Destroy(obj);
        obj = null;
    }

    public void reload_level()
    {
        if (player)
            Cmd_DestroyPlayer(player);

        have_find = false;
        player = null;
        stats = null;
        can_grap_player = false;

        select_panel.SetActive(true);
        death_panel.SetActive(false);
        pause_menu.SetActive(false);
        stats_bars.SetActive(false);
       
        weapon_panel.SetActive(false);
    }

    public void Play()
    {
        CharacterFactory factory = GetComponent<CharacterFactory>();
        if (!factory)
        {
            Debug.Log("no CharacterFactor attached to " + this);
            return;
        }

        Stats.Team selected_team = Stats.Team.None;

        if (team == 1)
            selected_team = Stats.Team.Orange;
        else if (team == 2)
            selected_team = Stats.Team.Blue;

        factory.Cmd_CreatePlayer(selected_team, weaponType - 1);//-1 car mes indices commencent à zéro


        select_panel.SetActive(false);
        death_panel.SetActive(false);
        pause_menu.SetActive(false);
        stats_bars.SetActive(true);
      
        weapon_panel.SetActive(true);
        crooshair.SetActive(true);
        can_grap_player = true;
    }

    public void snipselect()
    {
        weaponType = 1;
    }
    public void obusselect()
    {
        weaponType = 3;
    }
    public void falmeselect()
    {
        weaponType = 2;
    }
    public void Oselection()
    {
        selectbtnS.SetActive(false);
        selectbtnB.SetActive(false);

        team = 1;
        selectbtnO.SetActive(true);
    }
    public void Sselection()
    {
        selectbtnO.SetActive(false);
        selectbtnB.SetActive(false);

        team = 0;
        selectbtnS.SetActive(true);
    }
    public void Bselection()
    {
        selectbtnS.SetActive(false);
        selectbtnO.SetActive(false);

        team = 2;
        selectbtnB.SetActive(true);
    }
}