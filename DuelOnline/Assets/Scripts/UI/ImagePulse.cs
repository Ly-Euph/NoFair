using UnityEngine;
using UnityEngine.UI;

public class ImagePulseXY : MonoBehaviour
{
    [SerializeField] private Image targetImage;      // 対象のUIイメージ
    [Header("基本サイズ設定")]
    [SerializeField] private Vector2 baseSize = new Vector2(200f, 200f); // 基本サイズ
    [Header("揺れ幅設定")]
    [SerializeField] private Vector2 amplitude = new Vector2(30f, 50f);  // 振れ幅（±）
    [Header("変化速度（リズム）")]
    [SerializeField] private Vector2 speed = new Vector2(2f, 1.5f);      // 速度（軸ごと）

    private RectTransform rectTransform;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        rectTransform = targetImage.GetComponent<RectTransform>();
    }

    void Update()
    {
        // 各軸ごとにMathf.Sinで周期変化
        float width = baseSize.x + Mathf.Sin(Time.time * speed.x) * amplitude.x;
        float height = baseSize.y + Mathf.Sin(Time.time * speed.y) * amplitude.y;

        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
