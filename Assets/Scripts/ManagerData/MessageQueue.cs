using System.Collections.Generic;
using Mirror;

public class MessageQueue
{
    private List<NetworkMessage> messages;
    private int priorityIndex;

    public MessageQueue()
    {
        priorityIndex = 0;
        messages = new List<NetworkMessage>();
    }

    public bool Push(NetworkMessage message, bool isPriority)
    {
        if (isPriority)
        {
            messages.Insert(priorityIndex, message);
        }
        else
        {
            messages.Add(message);
        }

        return messages.Count == 2;
    }

    public NetworkMessage Pop()
    {
        NetworkMessage message = messages[0];
        messages.RemoveAt(0);

        if (messages.Count == 0)
        {
            priorityIndex = 0;
        }
        else
        {
            priorityIndex = messages.Count - 1;
        }

        return message;
    }

    public int GetLength()
    {
        return messages.Count;
    }
}
