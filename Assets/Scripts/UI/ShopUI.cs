using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopManager manager;

    [SerializeField] private ShopOptionUI shopPrefab;
    [SerializeField] private RectTransform shopParent;
    [SerializeField] private TextMeshProUGUI balance;

    private void Start()
    {
        balance.text = GlobalSettings.money.ToString();

        for (int i = 0; i < GlobalManager.singleton.fighters.Length; i++)
        {
            Fighter fighter = GlobalManager.singleton.fighters[i];
            for (int j = 0; j < fighter.outfits.Length; j++)
            {
                if (!GlobalSettings.unlocked[i, j])
                {
                    int jCopy = j;

                    ShopOptionUI option = Instantiate(shopPrefab, shopParent);
                    option.SetupUI(fighter, j);

                    option.GetComponent<Button>().onClick.AddListener(() => UnlockOutfit(option));
                }
            }
        }
    }

    private void UnlockOutfit(ShopOptionUI optionUI)
    {
        if (manager.UnlockOutfit(optionUI.fighter, optionUI.outfit))
        {
            Destroy(optionUI.gameObject);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        balance.text = GlobalSettings.money.ToString();
    }

    public void Reset()
    {
        manager.Reset();
        UpdateUI();
    }

    private void OnDestroy()
    {
        foreach (Transform child in shopParent)
        {
            child.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}
