using UnityEngine;
using UnityEngine.UI;

public class CanvasUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler scaler;

    private void Awake()
    {
        UpdateScale();
    }

    public void UpdateScale()
    {
        canvas.scaleFactor = Screen.width/scaler.referenceResolution.x * GlobalData.uiScale;
    }
}
