using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public enum State
    {
        WaitingForPlayer,
        StartOfGame,
        InGame,
        EndOfGame
    }
    [SyncVar]
    State state;

    [SyncVar]
    float decreasing_timer;
    [SyncVar]
    float game_time;

    [SyncVar]
    int orange_score;
    [SyncVar]
    int blue_score;

    [SyncVar]
    public string server_ip_adress;
    [SyncVar]
    public string port;

    int score_to_win = 25;

    // Use this for initialization
    void Start()
    {
        if (!hasAuthority)
            return;

        orange_score = 0;
        blue_score = 0;
        state = State.WaitingForPlayer;

        server_ip_adress = Network.player.ipAddress;
        port = NetworkServer.listenPort.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetKeyDown(KeyCode.Keypad8))
            decreasing_timer = 5.0f;

        UpdateState();
    }

    void UpdateState()
    {
        float delta_time = Time.deltaTime;
        decreasing_timer -= delta_time;

        switch (state)
        {
            case State.WaitingForPlayer:
                if (decreasing_timer <= 0.0f)
                {
                    if (HaveEnoughtPlayer())
                    {
                        state = State.StartOfGame;
                        decreasing_timer = 5.0f;
                    }
                    else
                        decreasing_timer = 0.5f;
                }
                break;
            case State.StartOfGame:
                if (decreasing_timer < 0.0f)
                {
                    StartGame(15, 00);
                    state = State.InGame;
                }
                break;
            case State.InGame:
                if (decreasing_timer <= 0.0f || blue_score >= score_to_win || orange_score >= score_to_win)
                {
                    state = State.EndOfGame;
                    decreasing_timer = 10.0f;
                }
                break;
            case State.EndOfGame:
                if (decreasing_timer <= 0.0f)
                    state = State.WaitingForPlayer;
                break;
        }
    }

    #region utils

    public void StartGame(float duration_minutes, float duration_secondes = 0.0f)
    {
        game_time = duration_minutes * 60.0f + duration_secondes;
        decreasing_timer = game_time;

        orange_score = 0;
        blue_score = 0;

        ReturnAllPlayersToSpawn();
    }

    bool HaveEnoughtPlayer()
    {
        Vector2 result = CountPlayer();
        return (result.x > 0 && result.y > 0);
    }

    void ReturnAllPlayersToSpawn()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<Stats>().need_respawn = true;
        }
    }

    Vector2 CountPlayer()
    {
        Vector2 result = new Vector2(0, 0);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Stats stats = player.GetComponent<Stats>();
            if (stats)
            {
                if (stats.team == Stats.Team.Orange)
                    result.x++;
                else if (stats.team == Stats.Team.Blue)
                    result.y++;
            }
        }
        return result;
    }

    public float getTimeRatio()
    {
        return (decreasing_timer / game_time);
    }

    public string getStringTime()
    {
        int minutes = (int)(decreasing_timer / 60);

        return ((minutes > 0 ? minutes + "min " : "") + ((int)(decreasing_timer) % 60) + "sec");
    }

    [Server]
    public void IncreaseOrangeScore()
    {
        orange_score++;
    }

    [Server]
    public void IncreaseBlueScore()
    {
        blue_score++;
    }

    public int getOrangeScore()
    {
        return orange_score;
    }

    public int getBlueScore()
    {
        return blue_score;
    }

    public State getState()
    {
        return state;
    }
    #endregion
}
