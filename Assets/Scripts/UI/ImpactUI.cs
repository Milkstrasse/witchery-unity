using UnityEngine;
using UnityEngine.UI;

public class ImpactUI : MonoBehaviour
{
    [SerializeField] private Transform impact;
    [SerializeField] private Image portrait;

    public void SetupUI(string fighter, bool rotated)
    {
        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter);

        transform.localPosition = new Vector3(0, rotated ? -160f : 160f, 0);
        impact.eulerAngles = rotated ? new Vector3(180f, 180f, 0f) : Vector3.zero;

        Time.timeScale = 0; //make sure turn doesn't end when playing animation

        LeanTween.moveLocal(portrait.gameObject, Vector3.zero, 0.2f).setOnComplete(PlaySound).setIgnoreTimeScale(true);
    }

    private void PlaySound()
    {
        AudioManager.singleton.PlayAttackSound();
        Time.timeScale = 1;
    }

    private void OnDisable()
    {
        portrait.transform.localPosition = new Vector3(-750f, -50f, 0);
    }
}
