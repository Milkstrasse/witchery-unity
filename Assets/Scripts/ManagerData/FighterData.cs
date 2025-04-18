[System.Serializable]
public class FighterData
{
    private bool[] outfits;

    public int timesUsedPrimary { get; private set; }
    public int timesUsedSecondary { get; private set; }
    public int timesWonPrimary;
    public int timesWonSecondary;

    public FighterData(Fighter fighter)
    {
        outfits = new bool[fighter.outfits.Length];
    }

    public void IncreasePrimaryUse()
    {
        timesUsedPrimary++;

        if (timesUsedPrimary + timesUsedSecondary >= 3 && !outfits[1])
        {
            UnlockOutfit(1);
        }
    }

    public void IncreaseSecondaryUse()
    {
        timesUsedSecondary++;

        if (timesUsedPrimary + timesUsedSecondary >= 3 && !outfits[1])
        {
            UnlockOutfit(1);
        }
    }

    public void UnlockFighter()
    {
        outfits[0] = true;
    }

    public void UnlockOutfit(int index)
    {
        outfits[index] = true;
    }

    public bool IsUnlocked(int index = 0)
    {
        return outfits[index];
    }
}