using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FadeInOut : MonoBehaviour
{
    private static FadeInOut instance;
    private Image fadeImage;
    private float fadeDuration = 1.0f;


    public static FadeInOut CreateInstance()
    {
        GameObject prefab = Resources.Load<GameObject>("FadeInOutPrefab"); // プレハブをロード
        GameObject instance = Instantiate(prefab); // 生成
        return instance.GetComponent<FadeInOut>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        fadeImage = GetComponentInChildren<Image>();
        fadeImage.enabled = false;
    }

    public void SetFadeDuration(float duration)
    {
        fadeDuration = duration;
    }

    public void LoadScene(string sceneName)
    {
        fadeImage.enabled = true;
        StartCoroutine(FadeOutAndChangeScene(sceneName));
    }

    private IEnumerator FadeOutAndChangeScene(string sceneName)
    {
        fadeImage.enabled = true;
        yield return Fade(0, 1); // フェードアウト
        if (GameLauncher.Instance != null && GameLauncher.Instance.GetMatchState == true)
        {
            if (GameLauncher.Instance.GetisHost)
            {
                GameLauncher.Instance.ChangeSceneOnline(sceneName);
                Debug.Log("オンラインでのシーンロード");
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("オフラインでのシーンロード");
        }
        yield return new WaitForSeconds(0.1f);
        yield return Fade(1, 0); // フェードイン

        // プールに戻す
        FadePoolManager.Instance.ReleaseFade(this);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}

