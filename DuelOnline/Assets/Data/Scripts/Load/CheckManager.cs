using UnityEngine;
using Mygame.Checker;
using Mygame.TextLoad;
using TMPro;

public class CheckManager : MonoBehaviour
{
    // ネット状況
    private enum NetState
    {
        NULL,
        OFFLINE,
        ONLINE,
    }
    NetState Nstate = NetState.NULL;

    [Header("ユーザーに知らせるために使うTMPro"), SerializeField]
    TextMeshProUGUI tmp;

    // スイッチ処理の番号
    private int process = 0;

    private float counter = 0;

    // 外部スクリプト
    ConnectionChecker checker;
    LoadText load;
    void Start()
    {
        // インスタンス生成
        checker = new ConnectionChecker();
        load = new LoadText();

        // 接続の確認
        StartCoroutine(checker.ICheck(OnNetworkCheck));
    }
    private void OnNetworkCheck(bool isOnline)
    {
        Nstate = isOnline ? NetState.ONLINE : NetState.OFFLINE;
    }

    void Update()
    {
            switch (process)
            {
            case 0: // 接続状況を確認中(テキスト表示のコルーチン開始)
                // 確認中であることをユーザーへ知らせるためのテキスト表示
                StartCoroutine(load.ILoadNow(tmp));
                process++;
            break;
            case 1: // 接続状況を確認中
                // NULLならまだ確認がおわってないのでリターン
                if (Nstate == NetState.NULL) { return; }
                // 少し間を空けて
                counter += Time.deltaTime;
                if (counter >= 3.0f)
                {
                    // 確認出来たら次の処理へ
                    StopAllCoroutines();
                    tmp.text = Nstate == NetState.ONLINE ?
                        "ネット接続を確認\nオンラインモードへ切り替えます。"
                        : "ネット接続を確認出来ませんでした。\nオフラインモードへ切り替えます。";
                    process++;
                }
            break;
            case 2: // タイトルへフェード
                FadePoolManager.Instance.GetFade().LoadScene("Title");
                process++;
            break;
            }
    }
}
