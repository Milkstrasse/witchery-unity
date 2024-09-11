using System.Collections.Generic;
using Mirror;

public class CustomQueue
{
    private List<NetworkMessage> messages;
    private int priorityIndex;

    public CustomQueue()
    {
        priorityIndex = 0;
        messages = new List<NetworkMessage>();
    }

    public bool AddToQueue(NetworkMessage message, bool isPriority)
    {
        if (isPriority)
        {
            messages.Insert(priorityIndex, message);
        }
        else
        {
            messages.Add(message);
        }

        priorityIndex = messages.Count;

        return messages.Count == 2;
    }

    public NetworkMessage PopFromQueue()
    {
        NetworkMessage message = messages[0];
        messages.RemoveAt(0);

        if (messages.Count < 2)
        {
            priorityIndex = 0;
        }
        else
        {
            priorityIndex = messages.Count;
        }

        return message;
    }

    public int GetLength()
    {
        return messages.Count;
    }
}
