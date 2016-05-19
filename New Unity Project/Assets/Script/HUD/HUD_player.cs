using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD_player : NetworkBehaviour
{
    enum State
    {
        Selection = 0,
        Play,
        Pause,
        Death,

        Count//Don't add something after it
    }

    State state;

    public GameObject character_factory;

    private Stats stats;//The stats to render
    private Image healthbar;
    private Image nrjbar;
    private Image ammo_count;

    GameObject[] panels;

    GameObject stats_bars;
    GameObject weapon_panel;
    GameObject score_panel;
    GameObject pause_menu;
    GameObject death_panel;
    GameObject select_panel;
    GameObject crosshair_panel;
    Text scoreB;
    int scoreb;
    Text scoreO;
    int scoreo;
    Image score_blue;
    Image score_orange;
<<<<<<< HEAD
    public string name;
=======

    Text timer_text;

>>>>>>> df6d9dc12c44d82ec60156b319f007f829faa2db
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

    //To recolor every players every x time
    float recolor_time = 0.0f;

    int team = -1; // 1 pour orange , 0 pour spectat, 2 pour bleu
    int weaponType = -1; // dans 0 ordre snip, lance-flame , obus de 0 a 2


    GameManager game_manager = null;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(hasAuthority);
        enabled = hasAuthority;

        if (!hasAuthority)
            return;

        Cmd_CreateCharacterFactory();

        panels = new GameObject[(int)State.Count];
        panels[(int)State.Play] = GameObject.Find("game_info");
        panels[(int)State.Selection] = GameObject.Find("select_perso");
        panels[(int)State.Death] = GameObject.Find("death_panel");
        panels[(int)State.Pause] = GameObject.Find("pause_panel");
        name = stats.Name;
       


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

        // score gestion
        score_panel = GameObject.Find("score_panel");
        if (score_panel)
        {
            scoreo = 0;
            scoreb = 0;
            Text[] scores = score_panel.GetComponentsInChildren<Text>();
            scoreB = scores[0];
            scoreO = scores[1];
            timer_text = scores[2];

            scoreB.text = scoreb.ToString();
            scoreO.text = scoreo.ToString();

            Image[] score_bars = score_panel.GetComponentsInChildren<Image>();
            score_orange = score_bars[1];
            score_blue = score_bars[2];

        }

        //select perso gestion
        playbtn = GameObject.Find("play_btn");
        selectbtnO = GameObject.Find("Oselect");
        selectbtnS = GameObject.Find("Sselect");
        selectbtnB = GameObject.Find("Bselect");

        selectbtnO.SetActive(false);
        selectbtnS.SetActive(false);
        selectbtnB.SetActive(false);

        playbtn.SetActive(false);

        ChangeState(State.Selection);

        game_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Initialisation(GameObject character)
    {
        stats = character.GetComponentInChildren<Stats>();//init des stats
        have_find = true;
        stats.paused = false;

        player = character;

        Text[] weapon_name = weapon_panel.GetComponentsInChildren<Text>();
        weapon_name[1].text = stats.WeaponName; // insert weapon name to be fair to other oponent else they stand no chance!

        //chrosshair gestion
        crosshair_panel = GameObject.Find("crosshair_panel");
        Image[] all_crosshair = crosshair_panel.GetComponentsInChildren<Image>();
        foreach (var img in all_crosshair)
        {
            img.gameObject.SetActive(img.gameObject.name == stats.WeaponName);
        }
    }

    [Command]
    void Cmd_CreateCharacterFactory()
    {
        if (character_factory)
            NetworkServer.ReplacePlayerForConnection(connectionToClient, Instantiate(character_factory), playerControllerId);
        else
            Debug.Log("Can't create CharacterFactory, So player can't spawn!");
    }

    // Update is called once per frame
    void Update()
    {
        if (team != -1 && weaponType != -1)
            playbtn.SetActive(true);

        UpdateState();
        RecolorAllPlayer();

        if (state == State.Play || state == State.Pause)
            DrawUpdate();

        if (state == State.Play && !have_find)
        {
            //Try to find the good player
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            {
                NetworkOwner net = obj.GetComponent<NetworkOwner>();
                if (net && net.IsMine())
                {
                    Initialisation(obj);
                    break;
                }
            }
        }
    }

    void RecolorAllPlayer()
    {
        if (recolor_time < 1.0f)
        {
            recolor_time += Time.deltaTime;
            return;
        }

        recolor_time = 0.0f;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            Stats s = obj.GetComponent<Stats>();
            if (s)
                s.RecolorPlayer();
        }
    }

    void DrawUpdate()
    {
        if (stats)
        {
            healthbar.fillAmount = stats.LifeRatio();
            Text pdv = healthbar.GetComponentInChildren<Text>();
            pdv.text = ((int)(Mathf.Ceil(stats.Life))).ToString() + "/" + stats.MaxLife.ToString();

            nrjbar.fillAmount = stats.EnergyRatio();
            Text point_nrj = nrjbar.GetComponentInChildren<Text>();
            point_nrj.text = ((int)(stats.Energy)).ToString() + "/" + ((int)(stats.MaxEnergy)).ToString();

            ammo_count.fillAmount = (float)stats.Ammo / (float)stats.MaxAmmo;
            Text amo = ammo_count.GetComponentInChildren<Text>();
            amo.text = stats.Ammo.ToString() + "/" + stats.MaxAmmo.ToString();

            // test
            if (Input.GetKeyDown(KeyCode.T))
            {
                stats.Life -= 20;
                stats.Energy -= 30;
                stats.Ammo -= 2;
                scoreo += 100;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                stats.Life += 10;
                scoreb++;
            }
        }

        if (game_manager)
        {
            scoreo = game_manager.getOrangeScore();
            scoreb = game_manager.getBlueScore();

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

            timer_text.text = game_manager.getStringTime();
        }
    }

    void UpdateState()
    {
        if (state == State.Play)
        {
            if (have_find && (!stats || stats.IsDead()))
                ChangeState(State.Death);
            else if (stats && stats.paused)
                ChangeState(State.Pause);
        }
        else if (state == State.Pause)
        {
            if (stats && !stats.paused)
                ChangeState(State.Play);
        }
    }

    void ChangeState(State s)
    {
        state = s;
        for (int i = 0; i < (int)State.Count; i++)
        {
            panels[i].SetActive((int)s == i);
        }

        if (s == State.Pause)
            panels[(int)State.Play].SetActive(true);

        Cursor.visible = (s != State.Play);

        if (s == State.Play)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        if (s == State.Selection)
        {
            have_find = false;
            player = null;
            stats = null;
        }
    }

    #region buttons
    public void unpause()
    {
        stats.paused = false;
        ChangeState(State.Play);
    }

    public void quit()
    {
        Application.Quit();
    }
    public void return2main()
    {
        SceneManager.LoadScene("menu_pricipal");
    }

    public void reload_level()
    {
        if (player && stats)
            stats.KillPlayer();

        ChangeState(State.Selection);
    }

    public void Play()
    {
        Stats.Team selected_team = Stats.Team.None;
        if (team == 1)
            selected_team = Stats.Team.Orange;
        else if (team == 2)
            selected_team = Stats.Team.Blue;

        GameObject factory = null;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CharacterFactory"))
        {
            if (obj.GetComponent<CharacterFactory>().hasAuthority)
            {
                factory = obj;
                break;
            }
        }

        if (factory)
            factory.GetComponent<CharacterFactory>().Cmd_CreatePlayer(weaponType, selected_team, true);
        else
            Debug.Log("Can't spawn player because you forgotten to add CharacterFactory Prefab to Player.Stats or to HUD_Player");

        ChangeState(State.Play);
    }

    public void snipselect()
    {
        weaponType = 0;
    }
    public void obusselect()
    {
        weaponType = 2;
    }
    public void falmeselect()
    {
        weaponType = 1;
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
    #endregion
}