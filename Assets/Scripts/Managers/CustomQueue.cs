using System.Collections.Generic;
using Mirror;

public class CustomQueue
{
    private List<NetworkMessage> messages;

    public CustomQueue()
    {
        messages = new List<NetworkMessage>();
    }

    public bool AddToQueue(NetworkMessage message, bool isPriority)
    {
        if (isPriority)
        {
            messages.Insert(0, message);
        }
        else
        {
            messages.Add(message);
        }

        return messages.Count == GlobalManager.singleton.maxPlayers;
    }

    public NetworkMessage PopFromQueue()
    {
        NetworkMessage message = messages[0];
        messages.RemoveAt(0);

        return message;
    }

    public int GetLength()
    {
        return messages.Count;
    }
}
