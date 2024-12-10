using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private MenuManager manager;
    private ShopOptionUI[] options;

    private void Start()
    {
        manager.OnShopOptionsCreated += SetupShopOptions;

        options = transform.GetComponentsInChildren<ShopOptionUI>();
    }

    private void SetupShopOptions(SelectedFighter[] fighters)
    {
        for (int i = 0; i < fighters.Length; i++)
        {
            options[i].SetupUI(GlobalData.fighters[fighters[i].fighterID], fighters[i].outfit);
        }
    }

    public void UnlockOutfit(int index)
    {
        if (manager.UnlockOutfit(options[index].fighter, options[index].outfit))
        {
            AudioManager.singleton.PlayPositiveSound();
        }
    }

    private void OnDestroy()
    {
        manager.OnShopOptionsCreated -= SetupShopOptions;
    }
}
