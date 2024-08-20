using Mirror;

public struct TeamMessage : NetworkMessage
{
    public string name;
    public int[] fighterIDs;
}
