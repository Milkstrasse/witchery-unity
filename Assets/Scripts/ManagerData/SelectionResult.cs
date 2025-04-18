public struct SelectionResult
{
    public bool wasAdded;
    public bool hasTeam;
    public SelectedFighter leader;

    public SelectionResult(bool wasAdded, bool hasTeam, SelectedFighter leader)
    {
        this.wasAdded = wasAdded;
        this.hasTeam = hasTeam;
        this.leader = leader;
    }
}