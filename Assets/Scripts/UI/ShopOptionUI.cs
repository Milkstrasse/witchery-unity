using TMPro;
using UnityEngine;

public class ShopOptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI option;
    public Fighter fighter;
    public int outfit;

    public void SetupUI(Fighter fighter, int outfit)
    {
        this.fighter = fighter;
        this.outfit = outfit;
        
        option.text = fighter.name + " - " + fighter.outfits[outfit].name;
    }
}
