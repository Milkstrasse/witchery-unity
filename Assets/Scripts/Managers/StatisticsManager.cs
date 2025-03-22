using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public void ReturnToSelection()
    {
        GlobalManager.singleton.LoadScene("SelectionScene");
    }
}
