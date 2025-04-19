using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PlayerOverUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent title;
    [SerializeField] private LocalizeStringEvent description;
    [SerializeField] private Image portrait;
    [SerializeField] private RawImage background;
    [SerializeField] private Image triangle;

    [SerializeField] private Material victory;
    [SerializeField] private Material defeat;

    public void UpdateUI(PlayerObject player)
    {
        title.StringReference.SetReference("StringTable", player.hasWon ? "victory" : "defeat");

        AsyncOperationHandle<Sprite> AssetHandle = Addressables.LoadAssetAsync<Sprite>(GlobalData.fighters[player.fighterIDs[0].fighterID].name + "-" + GlobalData.fighters[player.fighterIDs[0].fighterID].outfits[player.fighterIDs[0].outfit].name);
        AssetHandle.Completed += Sprite_Completed;

        background.material = player.hasWon ? victory : defeat;
        triangle.material = player.hasWon ? victory : defeat;

        (description.StringReference["amount"] as IntVariable).Value = player.roundsPlayed + 1;
        description.RefreshString();
    }

    private void Sprite_Completed(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            portrait.sprite = handle.Result;
        }
    }
}