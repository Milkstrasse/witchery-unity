public struct SelectionResult
{
    public bool wasAdded;
    public bool hasTeam;

    public SelectionResult(bool wasAdded, bool hasTeam)
    {
        this.wasAdded = wasAdded;
        this.hasTeam = hasTeam;
    }
}
