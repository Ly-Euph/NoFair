using UnityEngine;
using UnityEngine.UI;

public class VignetteEffect : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetActive(bool enable)
    {
        image.color = enable
            ? new Color(1, 0, 0, 0.15f)
            : new Color(0, 0, 0, 0);
    }
}
