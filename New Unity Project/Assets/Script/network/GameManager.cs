using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    enum State
    {
        WaitingForPlayer,
        StartOfGame,
        InGame,
        EndOfGame
    }
    [SyncVar]
    State state;

    [SyncVar]
    float timer;
    float second_timer;
    [SyncVar]
    float game_time;

    [SyncVar]
    int orange_score;
    [SyncVar]
    int blue_score;

    // Use this for initialization
    void Start()
    {
        if (!hasAuthority)
            return;

        state = State.WaitingForPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        UpdateState();
    }

    void UpdateState()
    {
        float delta_time = Time.deltaTime;
        switch (state)
        {
            case State.WaitingForPlayer:
                timer += delta_time;
                if (timer > 0.5f)
                {
                    if (HaveEnoughtPlayer())
                    {
                        state = State.StartOfGame;
                        GetComponent<Chat>().Cmd_SendMessage(Chat.Type.ServerInfo, "Starting game...", "server");
                    }
                    timer = 0.0f;
                }
                break;
            case State.StartOfGame:
                timer += delta_time;
                if (timer > 5.0f)
                {
                    StartGame(15, 00);
                    state = State.InGame;
                    second_timer = 0.0f;
                    GetComponent<Chat>().Cmd_SendMessage(Chat.Type.ServerInfo, "Game start", "server");
                }
                break;
            case State.InGame:
                timer -= delta_time;
                second_timer += delta_time;
                if (timer <= 0.0f || blue_score > 25 || orange_score > 25)
                {
                    state = State.EndOfGame;
                    GetComponent<Chat>().Cmd_SendMessage(Chat.Type.ServerInfo, "End of game", "server");
                    timer = 0.0f;
                }
                if (second_timer > 10.0)
                {
                    if (!HaveEnoughtPlayer())
                    {
                        state = State.WaitingForPlayer;
                        GetComponent<Chat>().Cmd_SendMessage(Chat.Type.ServerInfo, "Not enought player, stoping game", "server");
                    }
                    else
                        second_timer = 0.0f;
                }
                break;
            case State.EndOfGame:
                timer += delta_time;
                if (timer >= 10)
                {
                    state = State.WaitingForPlayer;
                    GetComponent<Chat>().Cmd_SendMessage(Chat.Type.ServerInfo, "Waiting for player", "server");
                }
                break;
        }
    }

    #region utils

    public void StartGame(float duration_minutes, float duration_secondes = 0.0f)
    {
        game_time = duration_minutes * 60.0f + duration_secondes;
        timer = game_time;

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
            player.GetComponent<Stats>().ReturnToSpawn();
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
        return (timer / game_time);
    }

    public string getStringTime()
    {
        return ((int)(timer / 60) + "min " + ((int)(timer) % 60) + "sec");
    }

    [Command]
    public void Cmd_increaseOrangeScore()
    {
        orange_score++;
    }

    [Command]
    public void Cmd_increaseBlueScore()
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
    #endregion
}
