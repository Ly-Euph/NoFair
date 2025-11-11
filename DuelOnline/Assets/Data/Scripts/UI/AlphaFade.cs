using UnityEngine;
using TMPro;
using System.Collections;

public class AlphaFade : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private float fadeDuration = 1.5f;

    private void Start()
    {
        // 最初は透明にしておく
        Color c = tmpText.color;
        c.a = 0f;
        tmpText.color = c;

        // フェードイン開始
        StartCoroutine(FadeText(false));
    }

    /// <summary>
    /// true = フェードイン, false = フェードアウト
    /// </summary>
    private IEnumerator FadeText(bool fadeIn)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float time = 0f;

        Color c = tmpText.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            tmpText.color = c;
            yield return null;
        }

        // 最終値を確実に反映
        c.a = endAlpha;
        tmpText.color = c;
    }
}
