using UnityEngine;

public class GameManager_Offline : MonoBehaviour
{
    // ここに最初の画面が出る
    // オブジェクトフォルダ
    [SerializeField] GameObject startObj;
    [SerializeField] GameObject gameCanvas;

    [SerializeField] CharaController_Offline charaScr;

    void Start()
    {
        // 起動
        startObj.SetActive(true);
        // ゲーム中のUI最初は非表示
        gameCanvas.SetActive(false);
    }

    void Update()
    {
        if(DataSingleton_Offline.Instance.IsReady)
        {
            // 対戦画面用のオブジェクトを削除
            startObj.SetActive(false);
            // ゲーム中のUI表示
            gameCanvas.SetActive(true);
        }
    }
}
