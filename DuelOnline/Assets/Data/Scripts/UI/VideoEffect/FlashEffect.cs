using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Flash(Color color, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(color, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        float t = 0;
        image.color = color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = 0.7f - (t / duration);
            image.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        image.color = new Color(0, 0, 0, 0);
    }
}
