using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    float remaining_time;
    [SyncVar]
    float game_time;

    [SyncVar]
    int orange_score;
    [SyncVar]
    int blue_score;

    [SyncVar]
    int nb_orange_player;
    [SyncVar]
    int nb_blue_player;

    // Use this for initialization
    void Start()
    {
        if (hasAuthority)
            StartGame(15, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        remaining_time -= Time.deltaTime;
    }

    public void StartGame(float duration_minutes, float duration_secondes = 0.0f)
    {
        game_time = duration_minutes * 60.0f + duration_secondes;
        remaining_time = game_time;

        orange_score = 0;
        blue_score = 0;
    }

    public float getTimeRatio()
    {
        return (remaining_time / game_time);
    }

    public string getStringTime()
    {
        return ((int)(remaining_time / 60) + "min " + ((int)(remaining_time) % 60) + "sec");
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
}
