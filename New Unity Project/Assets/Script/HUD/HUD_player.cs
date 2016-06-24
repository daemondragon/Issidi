using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;


public class HUD_player : NetworkBehaviour
{
    enum State
    {
        Selection = 0,
        Play,
        Pause,
        Chat,
        Death,

        Count//Don't add something after it
    }

    State state;

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
    GameObject input_tchat;
    GameObject option_panel;
    GameObject gamecontroller;
    

    InputField msg_input;
    Text msg_tchat;
    Text display_msg;
    Scrollbar scrollbar;
    public int chat_fade=7;

    Text scoreB;
    int scoreb;
    Text scoreO;
    int scoreo;
    Image score_blue;
    Image score_orange;

    Text orange_win;
    Text blue_win;
    Text tie;

    public string Name;

    Text timer_text;

    private Button btn_continue;

    GameObject playbtn;
    GameObject selectionpanel;
    GameObject weaponpanel;

    GameObject selectbtnO;
    GameObject selectbtnS;
    GameObject selectbtnB;


    public bool istchating;
    public float time_chat;

    GameObject croix_rouge_A;
    GameObject croix_rouge_P;

    public bool isAtStartup = true;
    NetworkClient myClient;

    InputField[] inputs;
    InputField Name_input;
    InputField address;
    InputField port;

    Button[] buttons;



    string ip_str;
    string port_str;
    string name_str;

    //Pour récuperer le bon character
    bool have_find;
    GameObject player;

    //To recolor every players every x time
    float recolor_time = 0.0f;

    int team = -1; // 1 pour orange , 0 pour spectat, 2 pour bleu
    int weaponType = -1; // dans 0 ordre snip, lance-flame , obus de 0 a 2

    bool sound_muted;
    GameObject speaker_on;
    GameObject speaker_off;

    GameManager game_manager = null;

    // Use this for initialization
    void Start()
    {
        panels = new GameObject[(int)State.Count];
        panels[(int)State.Play] = GameObject.Find("game_info");
        panels[(int)State.Selection] = GameObject.Find("select_perso");
        panels[(int)State.Death] = GameObject.Find("death_panel");
        panels[(int)State.Pause] = GameObject.Find("pause_panel");
        panels[(int)State.Chat] = GameObject.Find("tchatbox");
    
        // tchat

        input_tchat = GameObject.Find("tchatbox");
        msg_input = input_tchat.GetComponentInChildren<InputField>();
        gamecontroller = GameObject.FindGameObjectWithTag("GameController");
        istchating = false;
        display_msg = GameObject.Find("msg_display").GetComponent<Text>();
        time_chat = chat_fade;
        scrollbar = input_tchat.GetComponentInChildren<Scrollbar>();

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
            Text[] scores = score_panel.GetComponentsInChildren<Text>();
            scoreB = scores[1];
            scoreO = scores[0];
            timer_text = scores[2];

            scoreB.text = scoreb.ToString();
            scoreO.text = scoreo.ToString();

            Image[] score_bars = score_panel.GetComponentsInChildren<Image>();
            score_orange = score_bars[2];
            score_blue = score_bars[1];

        }

        blue_win = GameObject.Find("blue_win").GetComponent<Text>();
        orange_win = GameObject.Find("orange_win").GetComponent<Text>();
        tie = GameObject.Find("tie").GetComponent<Text>();

        //select perso gestion
        playbtn = GameObject.Find("play_btn");
        selectbtnO = GameObject.Find("Oselect");
        selectbtnS = GameObject.Find("Sselect");
        selectbtnB = GameObject.Find("Bselect");

        selectbtnO.SetActive(false);
        selectbtnS.SetActive(false);
        selectbtnB.SetActive(false);

        playbtn.SetActive(false);

        sound_muted = true;
        speaker_on = GameObject.Find("speaker_on");
        speaker_off = GameObject.Find("speaker_off");
        SwitchMuteSound();//Sound will be unmuted after this

        game_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        ChangeState(State.Selection);
    }

    void Initialisation(GameObject character)
    {
        stats = character.GetComponentInChildren<Stats>();//init des stats
        have_find = true;
        stats.CanMovePlayer = true;

        player = character;
        Name = stats.Name;

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

    // Update is called once per frame
    void Update()

    {
        if (team != -1 && weaponType != -1)
            playbtn.SetActive(true);

        UpdateState();
        RecolorAllPlayer();

        if (state == State.Play || state == State.Pause || state == State.Chat)
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
        if (state == State.Play && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            time_chat = 0f;
            ChangeState(State.Chat);
            stats.CanMovePlayer = false;
            input_tchat.SetActive(true);

            msg_input.ActivateInputField();
            msg_input.Select();
            msg_input.MoveTextStart(true);


            msg_input.text += " ";
           

        }
        if (state == State.Play)
            time_chat += Time.deltaTime;
        if (time_chat >= (float)chat_fade)
        {
            input_tchat.SetActive(false);
                 }
        else
        {
            input_tchat.SetActive(true);
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

    public void SwitchMuteSound()
    {
        sound_muted = !sound_muted;

        speaker_on.SetActive(!sound_muted);
        speaker_off.SetActive(sound_muted);

        //Mute sound
        if (sound_muted)
            AudioListener.volume = 0.0f;
        else
            AudioListener.volume = 1.0f;
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
        }

        if (game_manager)
        {
            string display_string = "";
            switch (game_manager.getState())
            {
                case GameManager.State.WaitingForPlayer:
                    scoreo = 0;
                    scoreb = 0;
                    display_string = "Waiting for players.";
                    break;
                case GameManager.State.StartOfGame:
                    scoreo = 0;
                    scoreb = 0;
                    display_string = "Starting game in: " + game_manager.getStringTime();
                    break;
                case GameManager.State.InGame:
                    scoreo = game_manager.getOrangeScore();
                    scoreb = game_manager.getBlueScore();
                    display_string = "Time left: " + game_manager.getStringTime();
                    break;
                case GameManager.State.EndOfGame:
                    scoreo = game_manager.getOrangeScore();
                    scoreb = game_manager.getBlueScore();
                    display_string = "The End.";
                    break;
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

            timer_text.text = display_string;

            if (game_manager.getState() == GameManager.State.EndOfGame)
            {
                if (scoreo == scoreb)
                    tie.enabled = true;
                else if (scoreo > scoreb)
                    orange_win.enabled = true;
                else
                    blue_win.enabled = true;
            }
            else
            {
                orange_win.enabled = false;
                blue_win.enabled = false;
                tie.enabled = false;
            }

            displayChat();
        }

        if (Input.GetKey(KeyCode.Keypad7))
        {
            stats.Life = stats.MaxLife;
            stats.Energy = stats.MaxEnergy;
            stats.Ammo = stats.MaxAmmo;
        }
        else if (Input.GetKey(KeyCode.Keypad9))
        {
            stats.KillPlayer();
        }
    }

    void UpdateState()
    {
        if (state == State.Play)
        {
            if (have_find && (!stats || stats.IsDead()))
                ChangeState(State.Death);
            else if (Input.GetKeyDown(KeyCode.Escape))
                ChangeState(State.Pause);
        }
        else if (state == State.Pause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ChangeState(State.Play);
        }
        else if (state == State.Chat)
        {
        
            if (Input.GetKeyDown(KeyCode.Escape))
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

 
        if (s == State.Pause || s == State.Chat)
            panels[(int)State.Play].SetActive(true);

        Cursor.visible = (s != State.Play);
        if (stats)
            stats.CanMovePlayer = (s == State.Play);

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

        if (s == State.Pause && game_manager)
            GameObject.Find("ip_adress").GetComponent<InputField>().text = game_manager.server_ip_adress + ":" + game_manager.port;
    }

    public void send_tchat()

    {
        if (msg_input.text != null)
        {
            string msg = msg_input.text.ToString();

            gamecontroller.GetComponent<Chat>().Cmd_SendMessage(Chat.Type.ServerInfo, msg, Name);
        }

        ChangeState(State.Play);
        time_chat = 0f;

    }

    public void displayChat()
    {
        Chat.SyncMessages historyMessage = gamecontroller.GetComponent<Chat>().Messages;
        display_msg.text = "";
        foreach (var msg in historyMessage)
        {
            display_msg.text += "\n" + msg.sender + " : " + msg.text;
        }

    }
    public void new_message()
    {
        time_chat = 0f;
    }

    #region buttons
    public void unpause()
    {
        stats.CanMovePlayer = true;
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
            factory.GetComponent<CharacterFactory>().Cmd_CreatePlayer(weaponType, selected_team);
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
    public void blasterselect()
    {
        weaponType = 3;
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

    #region serveur

    // Create a server and listen on a port
    public void SetupServer()
    {
        Debug.Log(port_str);
        NetworkServer.Listen(Convert.ToUInt16(port_str));
        isAtStartup = false;
    }

    // Create a client and connect to the server port
    public void SetupClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.Connect(ip_str, Convert.ToUInt16(port_str));
        isAtStartup = false;
    }

    // Create a local client and connect to the local server
    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
    }
    // client function
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
    }
    public void Create_join()
    {

        SetupServer();

        SetupLocalClient();
        ChangeState(State.Selection);

    }
    private void button_set(bool b)
    {

        buttons[1].interactable = b;

    }

    private bool address_valid(string s)
    {
        IPAddress osef;
        return s != "" && IPAddress.TryParse(s, out osef) && s.Contains(".");
    }

    #endregion
}