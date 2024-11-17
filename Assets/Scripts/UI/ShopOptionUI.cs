using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOptionUI : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Button button;

    [SerializeField] private RawImage background;
    [SerializeField] private Image triangle;
    [SerializeField] private Material neutral;
    [SerializeField] private Material highlighted;

    public Fighter fighter;
    public int outfit;

    public bool isUnlockable;

    public void SetupUI(Fighter fighter, int outfit)
    {
        this.fighter = fighter;
        this.outfit = outfit;

        if (outfit > 0)
        {
            isUnlockable = SaveManager.savedData.unlocked[fighter.fighterID, 0] && !SaveManager.savedData.unlocked[fighter.fighterID, outfit];
        }
        else
        {
            isUnlockable = !SaveManager.savedData.unlocked[fighter.fighterID, 0];
        }

        button.interactable = isUnlockable && SaveManager.savedData.money >= fighter.outfits[outfit].cost;

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-" + fighter.outfits[outfit].name);
        title.text = fighter.name;
        cost.text = $"{fighter.outfits[outfit].cost} SP";

        background.material = isUnlockable ? highlighted : neutral;
        triangle.material = isUnlockable ? highlighted : neutral;
    }

    public bool CheckStatus()
    {
        bool lastStatus = isUnlockable;

        if (outfit > 0)
        {
            isUnlockable = SaveManager.savedData.unlocked[fighter.fighterID, 0] && !SaveManager.savedData.unlocked[fighter.fighterID, outfit];
        }
        else
        {
            isUnlockable = !SaveManager.savedData.unlocked[fighter.fighterID, 0];
        }

        button.interactable = isUnlockable && SaveManager.savedData.money >= fighter.outfits[outfit].cost;

        background.material = isUnlockable ? highlighted : neutral;
        triangle.material = isUnlockable ? highlighted : neutral;

        return lastStatus != isUnlockable;
    }
}
