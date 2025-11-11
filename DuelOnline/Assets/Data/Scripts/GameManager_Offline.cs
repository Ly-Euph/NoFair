using UnityEngine;
public class GameManager_Offline : MonoBehaviour
{
    // ここに最初の画面が出る
    // オブジェクトフォルダ
    [SerializeField] GameObject startObj;
    [SerializeField] GameObject gameCanvas;
    // 勝敗結果を表す列挙型
    public enum BattleResult
    {
        None,
        Player1Win,
        Player2Win,
        Draw
    }

    // 初期番号0
    private int switchNo = 0;
    void Start()
    {
        // 起動
        startObj.SetActive(true);
        // ゲーム中のUI最初は非表示
        gameCanvas.SetActive(false);
    }

    void Update()
    {
        switch(switchNo)
        {
            case 0: // 対戦前準備
                if (DataSingleton_Offline.Instance.IsReady)
                {
                    // 対戦画面用のオブジェクトを削除
                    startObj.SetActive(false);
                    // ゲーム中のUI表示
                    gameCanvas.SetActive(true);
                    // ネクスト処理
                    switchNo ++;
                }
                break;
            case 1: // ラウンド終了処理
                var result = CheckBattleResult();

                switch (result)
                {
                    case BattleResult.None:
                        return;

                    case BattleResult.Player1Win:
                        // P1勝利処理
                        Debug.Log("P1");
                        break;

                    case BattleResult.Player2Win:
                        // P2勝利処理
                        Debug.Log("P2");
                        break;

                    case BattleResult.Draw:
                        // 両者同時撃破
                        Debug.Log("W");
                        break;
                }
                break;
        }
       
    }

    // 勝利判定関数
    private BattleResult CheckBattleResult()
    {
        float p1HP = DataSingleton_Offline.Instance.PlHP;
        float p2HP = DataSingleton_Offline.Instance.EmHP;

        bool isDeadP1 = p1HP <= 0;
        bool isDeadP2 = p2HP <= 0;

        if (!isDeadP1 && !isDeadP2)
            return BattleResult.None;

        if (isDeadP1 && isDeadP2)
            return BattleResult.Draw;

        return isDeadP1 ? BattleResult.Player2Win : BattleResult.Player1Win;
    }

}
