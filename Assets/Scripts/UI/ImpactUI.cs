using UnityEngine;
using UnityEngine.UI;

public class ImpactUI : MonoBehaviour
{
    [SerializeField] private Transform impact;
    [SerializeField] private Image portrait;

    private int target;

    private void Awake()
    {
        LeanTween.moveLocal(portrait.gameObject, new Vector3(-750f, -50f, 0f), 0.1f);
    }

    public void SetupUI(int target, string fighter, string outfit, bool rotated)
    {
        this.target = target;

        portrait.sprite = Resources.Load<Sprite>("Sprites/" + fighter + "-" + outfit);

        //transform.localPosition = new Vector3(0, rotated ? -160f : 160f, 0);
        impact.eulerAngles = rotated ? new Vector3(180f, 180f, 0f) : Vector3.zero;

        Time.timeScale = 0; //make sure turn doesn't end when playing animation

        LeanTween.moveLocal(portrait.gameObject, Vector3.zero, 0.2f).setOnComplete(PlaySound).setIgnoreTimeScale(true);
    }

    private void PlaySound()
    {
        if (target == 0)
        {
            AudioManager.singleton.PlayHealSound();
        }
        else
        {
            AudioManager.singleton.PlayAttackSound();
        }

        Time.timeScale = 1;
    }

    public void ToggleVisibility(bool isVisible)
    {
        if (isVisible)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            portrait.transform.localPosition = new Vector3(-750f, -50f, 0f);
            transform.localScale = Vector3.zero;
        }
    }
}