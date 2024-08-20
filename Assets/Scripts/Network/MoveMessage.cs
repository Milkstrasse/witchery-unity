using Mirror;

public struct MoveMessage : NetworkMessage
{
    public int cardIndex;
    public bool cardPlayed;
    public bool toRemove;
}
