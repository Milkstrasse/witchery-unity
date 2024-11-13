using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOptionUI : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cost;

    public Fighter fighter;
    public int outfit;
    public Button button;

    public void SetupUI(Fighter fighter, int outfit)
    {
        this.fighter = fighter;
        this.outfit = outfit;

        if (outfit > 0)
        {
            button.interactable = SaveManager.savedData.unlocked[fighter.fighterID, 0] && !SaveManager.savedData.unlocked[fighter.fighterID, outfit];
        }
        else
        {
            button.interactable = !SaveManager.savedData.unlocked[fighter.fighterID, 0];
        }

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter.name + "-" + fighter.outfits[outfit].name);
        title.text = fighter.name;
        cost.text = $"{fighter.outfits[outfit].cost} SP";
    }

    public void CheckStatus()
    {
        if (!button.interactable)
        {
            button.interactable = SaveManager.savedData.unlocked[fighter.fighterID, 0] && !SaveManager.savedData.unlocked[fighter.fighterID, outfit];
        }
    }
}
