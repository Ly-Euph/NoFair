using UnityEngine;
using UnityEngine.UI;

public class ChargeRiseEffect : MonoBehaviour
{
    [SerializeField] private RectTransform effectRect;
    [Header("Motion")]
    [SerializeField] private float amplitude = 40f;   // 上下幅
    [SerializeField] private float speed = 6f;        // 揺れ速度

    private bool isPlaying;
    private float baseY;
    private float time;
    float timer = 0;

    private void Awake()
    {
        baseY = effectRect.anchoredPosition.y;
    }


    public void Play()
    {
        // ▶ 開始時リセット
        time = 0f;
        effectRect.anchoredPosition =
            new Vector2(effectRect.anchoredPosition.x, baseY);

        gameObject.SetActive(true);
        isPlaying = true;
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

        time += Time.deltaTime * speed;
        float offsetY = Mathf.Sin(time) * amplitude;

        effectRect.anchoredPosition =
            new Vector2(effectRect.anchoredPosition.x, baseY + offsetY);

        if (timer>=0.6f)
        {
            timer = 0;
            Stop();
        }
    }
}
