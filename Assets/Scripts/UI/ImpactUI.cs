using UnityEngine;
using UnityEngine.UI;

public class ImpactUI : MonoBehaviour
{
    [SerializeField] private Transform impact;
    [SerializeField] private Image portrait;

    private int target;

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

    private void OnDisable()
    {
        portrait.transform.localPosition = new Vector3(-750f, -50f, 0);
    }
}
