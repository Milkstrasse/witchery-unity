using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    [SerializeField]
    private SelectionManager manager;
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TextMeshProUGUI iconTop;
    public PlayerSelectionUI playerTop;
    [SerializeField]
    private TextMeshProUGUI iconBottom;
    public PlayerSelectionUI playerBottom;

    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();

        manager.onStartedClient += UpdatePortrait;
        manager.onTimerChanged += UpdateTimer;

        playerTop.SetupUI(canvas);
        playerBottom.SetupUI(canvas);
    }

    private void UpdateTimer(int time)
    {
        LeanTween.value(slider.gameObject, slider.value, time/(float)GlobalManager.connectionTime, 1f ).setOnUpdate( (float val) => { slider.value = val; } );
    }

    private void UpdatePortrait(bool isHost)
    {
        if (isHost)
        {
            iconTop.text = "C";
            iconBottom.text = "H";
        }
        else
        {
            iconTop.text = "H";
            iconBottom.text = "C";
        }
    }

    public void SetReady(Button readyButton)
    {
        playerBottom.ready = manager.SetReady(!playerBottom.ready, playerBottom);

        if (playerBottom.ready)
        {
            playerBottom.ToggleSelection(true);
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
        }
        else
        {
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
            playerBottom.ToggleSelection(false);
        }
    }

    public void StopSelection()
    {
        GlobalManager.singleton.LoadScene("MenuScene");
    }

    private void OnDestroy()
    {
        manager.onTimerChanged -= UpdateTimer;
        manager.onStartedClient -= UpdatePortrait;
    }
}