using UnityEngine;
using TMPro;
using System.Collections;

public class Button_MULTI : MonoBehaviour
{
    // マッチング中のキャンバス
    [SerializeField] GameObject canvas;
    [SerializeField] TextMeshProUGUI tmPro;

    string text = "マッチング中";
    char dot = '.';
    // オンラインのマッチング

    public void Awake()
    {
       
    }
    public void OnClick()
    {
        var Ins = GameLauncher.Instance;
        // 念のため切断
        Ins.LeaveSession();
        // マッチングだよと分かりやすく
        canvas.SetActive(true);

        // マッチング処理
        Ins.Match();

        // 表示用コルーチン
        StartCoroutine(NowMatch(tmPro, dot));

    }

    private void Update()
    {
        var Ins = GameLauncher.Instance;
        if (Ins.GetMatchState)
        {
            StopAllCoroutines();
            tmPro.text = "対戦相手を確認";
            this.gameObject.GetComponent<Button_MULTI>().enabled = false;
        }
    }

    private IEnumerator NowMatch(TextMeshProUGUI tmp, char dot)
    {
        while (true)
        {
            // まずクリア
            tmp.text = text;

            for (int i = 0; i < 3; i++)
            {
                tmp.text += dot;
                yield return new WaitForSeconds(0.8f); // 表示速度
            }
        }
    }
}
