using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
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

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + GlobalData.fighters[player.fighterIDs[0].fighterID].name + "-" + GlobalData.fighters[player.fighterIDs[0].fighterID].outfits[player.fighterIDs[0].outfit].name);

        background.material = player.hasWon ? victory : defeat;
        triangle.material = player.hasWon ? victory : defeat;

        (description.StringReference["amount"] as IntVariable).Value = player.energy;
        description.RefreshString();
    }
}
