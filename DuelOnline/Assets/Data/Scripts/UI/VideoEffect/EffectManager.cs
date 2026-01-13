using UnityEngine;
public class EffectManager : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private FlashEffect flashEffect;
    [SerializeField] private VignetteEffect vignetteEffect;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private ChargeRiseEffect chargeEffect;
    [SerializeField] private CommandText commandText;

    public void CommandTextInput(string text)
    {
        commandText.InputCommand(text);
    }

    public void StartCharge()
    {
        chargeEffect.Play();
        flashEffect.Flash(Color.yellow * 0.8f, 0.4f);
    }

    public void EndCharge()
    {
        chargeEffect.Stop();
    }

    public void PlayDamage()
    {
        flashEffect.Flash(Color.red, 0.15f);
        StartCoroutine(cameraShake.Shake(0.4f, 0.2f));
    }

    public void PlayCharge()
    {
        flashEffect.Flash(Color.yellow * 0.8f, 1f);
    }

    public void PlayGuard()
    {
        flashEffect.Flash(new Color(0.3f, 0.6f, 1f, 0.25f), 1.2f);
    }

    public void PlayWeakMagic()
    {
        flashEffect.Flash(Color.blue * 0.6f, 0.4f);
    }

    public void PlayStrongMagic()
    {
        flashEffect.Flash(Color.red * 0.6f, 0.4f);
        StartCoroutine(cameraShake.Shake(0.4f, 0.8f));
    }

    public void SetLowHP(bool enable)
    {
        vignetteEffect.SetActive(enable);
    }
}
