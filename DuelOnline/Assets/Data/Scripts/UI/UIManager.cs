using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject Eff;

    public static UIManager Instance { get; private set; }
    private Action currentAction;    // 現在カーソルが乗っているボタンの処理
    private string currentButton = ""; // デバッグ用
    private GameObject currentObj;   // パネルの非表示

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // 左クリックされたときに実行
        if (Input.GetMouseButtonDown(0) && currentAction != null)
        {
            // エフェクト生成（3D Transform 用）
            if (Eff != null)
            {
                // ① マウス位置取得
                Vector3 mousePos = Input.mousePosition;

                // ② カメラからどれくらい前に出すか（距離）
                //   ボタンが UI なので z を固定しないとカメラ上に表示されない
                mousePos.z = 2f; // カメラから2m前に出す例（必要に応じて調整）

                // ③ ワールド座標に変換
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

                // ④ エフェクトをワールド座標に移動して再生
                GameObject effObj = Instantiate(Eff, worldPos, Quaternion.identity);

                // 数秒後に破棄（任意）
                Destroy(effObj, 1.2f);
            }

            Debug.Log($"[UIManager] {currentButton} のActionを実行します");
            currentAction.Invoke();
            currentObj.SetActive(false);
            AudioManager.Instance.PlaySE(0);
        }
    }

    /// <summary>
    /// 現在カーソルが当たっているUIのアクションを登録
    /// </summary>
    public void RegisterCurrentAction(string buttonName, Action action,GameObject obj)
    {
        currentButton = buttonName;
        currentAction = action;
        currentObj = obj;
    }

    /// <summary>
    /// カーソルが離れたら例外（無効状態）に戻す
    /// </summary>
    public void ClearCurrentAction()
    {
        currentButton = "";
        currentAction = null;
    }
}
