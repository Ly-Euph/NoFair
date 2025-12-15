using UnityEngine;
using UnityEngine.UI;

public class ChargeRiseEffect : MonoBehaviour
{
    [SerializeField] private RectTransform effectRect;
    [SerializeField] private float speed = 200f;

    // 開始位置（下側）
    [SerializeField] private float startY = -600f;
    private bool isPlaying;

    float timer = 0;

    public void Play()
    {
        // 位置リセット
        effectRect.anchoredPosition = new Vector2(
            effectRect.anchoredPosition.x,
            startY
        );

        isPlaying = true;
        gameObject.SetActive(true);
    }

    public void Stop()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying) return;
        timer += Time.deltaTime;
        Vector2 pos = effectRect.anchoredPosition;
        pos.y += speed * Time.deltaTime;

        // 上に行ききったら下に戻す（ループ）
        if (pos.y > effectRect.sizeDelta.y)
        {
            pos.y = -effectRect.sizeDelta.y;
        }

        effectRect.anchoredPosition = pos;

        if(timer>=0.6f)
        {
            timer = 0;
            Stop();
        }
    }
}
