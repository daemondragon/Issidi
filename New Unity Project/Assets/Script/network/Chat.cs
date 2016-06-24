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

    SyncMessages messages = new SyncMessages();
    public SyncMessages Messages
    {
        get { return messages; }
    }

    int max_message = 30;

    // Update is called once per frame
    void Update()
    {
    }

    [Command]
    public void Cmd_SendMessage(Type type, string message, string sender)
    {
        if (type != Type.Command)
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
