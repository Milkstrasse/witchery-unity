public struct SelectionResult
{
    public bool wasAdded;
    public bool hasTeam;
    public int leader;

    public SelectionResult(bool wasAdded, bool hasTeam, int leader)
    {
        this.wasAdded = wasAdded;
        this.hasTeam = hasTeam;
        this.leader = leader;
    }
}
