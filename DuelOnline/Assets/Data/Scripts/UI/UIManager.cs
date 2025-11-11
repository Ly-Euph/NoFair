using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
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
        if (Input.GetMouseButtonDown(0)&&currentAction!=null)
        {
            if (currentAction != null)
            {
                Debug.Log($"[UIManager] {currentButton} のActionを実行します");
                currentAction.Invoke();
                currentObj.SetActive(false);
                AudioManager.Instance.PlaySE(0);
            }
            else
            {
                Debug.Log("[UIManager] 有効なUIがありません（例外状態）");
            }
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
