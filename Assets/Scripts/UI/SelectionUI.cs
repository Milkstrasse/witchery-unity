using UnityEngine;

public class SelectionUI : MonoBehaviour
{
    [SerializeField] private SelectionManager manager;
    
    [SerializeField] private PlayerSelectionUI playerTop;
    [SerializeField] private PlayerSelectionUI playerBottom;

    private void Start()
    {
        manager.onTimerChanged += playerBottom.SetTimer;
    }

    public void SetReady()
    {
        bool isReady = manager.SetReady();

        playerBottom.ToggleUI(!isReady);
    }

    public bool EditTeam(int fighterID)
    {
        return manager.EditTeam(fighterID);
    }

    public void StopSelection()
    {
        manager.ReturnToMenu();
    }

    private void OnDestroy()
    {
        manager.onTimerChanged -= playerBottom.SetTimer;
    }
}