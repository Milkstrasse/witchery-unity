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

    public void SetReady(int index)
    {
        bool isReady = manager.SetReady(index);
        
        if (GlobalManager.singleton.maxPlayers < 2)
        {
            playerTop.ToggleUI(isReady);
        }
        
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