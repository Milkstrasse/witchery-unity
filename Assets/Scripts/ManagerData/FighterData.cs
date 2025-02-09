[System.Serializable]
public class FighterData
{
    private bool[] outfits;

    public int timesUsedPrimary;
    public int timesUsedSecondary;
    public int timesWonPrimary;
    public int timesWonSecondary;

    public FighterData(Fighter fighter)
    {
        outfits = new bool[fighter.outfits.Length];
    }

    public void UnlockFighter()
    {
        outfits[0] = true;
    }

    public void SetOutfit(int index, bool unlocked)
    {
        outfits[index] = unlocked;
    }

    public bool IsUnlocked(int index = 0)
    {
        return outfits[index];
    }
}
