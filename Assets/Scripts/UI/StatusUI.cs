using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI icon;

    public void SetupEffect(StatusEffect effect)
    {
        icon.text = effect.duration.ToString();
    }
}
