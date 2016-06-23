using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD_solo : MonoBehaviour
{
    enum State
    {
        Play = 0,
        Pause,

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
    GameObject crosshair_panel;

    public string Name;

    Text timer_text;

    private Button btn_continue;
    bool have_find;

    bool sound_muted;
    GameObject speaker_on;
    GameObject speaker_off;

    GameObject weaponpanel;

    // Use this for initialization
    void Start()
    {
        panels = new GameObject[(int)State.Count];
        panels[(int)State.Play] = GameObject.Find("game_info");
        panels[(int)State.Pause] = GameObject.Find("pause_panel");

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
            timer_text = scores[0];

        }

        sound_muted = true;
        speaker_on = GameObject.Find("speaker_on");
        speaker_off = GameObject.Find("speaker_off");
        SwitchMuteSound();//Sound will be unmuted after this

        ChangeState(State.Play);

        have_find = false;

        timer_text.text = "Bienvenue dans le tutoriel";
    }

    void Initialisation(GameObject character)
    {
        stats = character.GetComponentInChildren<Stats>();//init des stats
        stats.CanMovePlayer = true;
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
        have_find = true;
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

    // Update is called once per frame
    void Update()
    {
        if (!have_find)
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
        UpdateState();
        DrawUpdate();
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

        
    }

    public void setInfoMessage(string message)
    {
        timer_text.text = message;
    }

    void UpdateState()
    {
        if (state == State.Play)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ChangeState(State.Pause);
        }
        else if (state == State.Pause)
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

        if (s == State.Pause)
            panels[(int)State.Play].SetActive(true);

        Cursor.visible = (s != State.Play);

        if (stats)
            stats.CanMovePlayer = (s == State.Play);

        if (s == State.Play)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    #region buttons
    public void unpause()
    {
        stats.CanMovePlayer = true;
        ChangeState(State.Play);
    }

    public void return2main()
    {
        SceneManager.LoadScene("menu_pricipal");
    }

    #endregion
}