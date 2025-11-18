using System.Collections.Generic;
using UnityEngine;

public class FadePoolManager : MonoBehaviour
{
    public static FadePoolManager Instance { get; private set; }

    [SerializeField] private GameObject fadePrefab; // プレハブをインスペクタで指定
    private List<FadeInOut> fadePool = new List<FadeInOut>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// プールから使えるFadeを取得
    /// </summary>
    public FadeInOut GetFade()
    {
        // 非アクティブのものがあれば使う
        foreach (var f in fadePool)
        {
            if (!f.gameObject.activeSelf)
            {
                f.gameObject.SetActive(true);
                return f;
            }
        }

        // なければ新しく作る
        GameObject obj = Instantiate(fadePrefab, transform);
        FadeInOut fade = obj.GetComponent<FadeInOut>();
        fadePool.Add(fade);
        return fade;
    }

    /// <summary>
    /// フェード終了後に呼ぶ（非アクティブにしてプールに戻す）
    /// </summary>
    public void ReleaseFade(FadeInOut fade)
    {
        fade.StopAllCoroutines(); // 念のためコルーチン停止
        fade.gameObject.SetActive(false);
    }
}
