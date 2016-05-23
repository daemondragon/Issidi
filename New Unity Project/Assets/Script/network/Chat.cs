using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;



public class Chat : NetworkBehaviour
{
    public enum Type
    {//You can set differents colors for each type of message
        PlayerMessage,
        ServerInfo,//player connected, game will start...
        Command,//Kick player, stop game... (NOT implemented)
        Debug
    }

    public struct Message
    {
        public Type type;
        public string text;
        public string sender;
    }

    public class SyncMessages : SyncListStruct<Message> { }

    List<float> time_left = new List<float>();//To know how many second the message will stay in the screen
    SyncMessages messages = new SyncMessages();
    public SyncMessages Messages
    {
        get { return messages; }
    }

    float time_in_chat = 5.0f;
    int max_message = 10;

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        if (messages == null || time_left == null)
        {
            Debug.Log("Either messages or time_left is null :" + messages + ";" + time_left);
            return;
        }

        float delta_time = Time.deltaTime;
        for (int i = 0; i < messages.Count; i++)
        {
            time_left[i] += delta_time;
            if (time_left[i] > time_in_chat)
            {
                time_left.RemoveAt(i);
                messages.RemoveAt(i);
                i--;
            }
        }
    }

    [Command]
    public void Cmd_SendMessage(Type type, string message, string sender)
    {
        if (type == Type.Command)
            ;//Do something
        else
        {
            messages.Add(new Message
            {
                type = type,
                text = message,
                sender = sender
            });
            if (messages.Count > max_message)
                messages.RemoveAt(0);
        }
    }
}
