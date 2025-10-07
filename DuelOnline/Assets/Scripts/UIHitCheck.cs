using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public class UIHitCheck : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    #region Field
    [Header("カーソルが触れた時に選択中として表示する"),SerializeField]
    GameObject obj_SelectPnl;
    // 登録するボタン名（Inspectorで指定 or 自動でgameObject名）
    [SerializeField] private string buttonName = "";
    private Action clickAction;
    #endregion

    #region Interface
    // カーソルがUIに入ったとき呼ばれる
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} にカーソルが乗りました");

        // 表示
        obj_SelectPnl.SetActive(true);

        // UIManagerに関数登録
        UIManager.Instance.RegisterCurrentAction(buttonName, clickAction);
    }

    // カーソルがUIから出たとき呼ばれる
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} からカーソルが離れました");

        // 非表示
        obj_SelectPnl.SetActive(false);

        // 例外にする
        UIManager.Instance.ClearCurrentAction();
    }
    #endregion

    void Awake()
    {
        // 最初は非表示
        obj_SelectPnl.SetActive(false);

        // 名前未設定ならGameObject名を使う
        if (string.IsNullOrEmpty(buttonName))
            buttonName = gameObject.name;

        // 子オブジェクトから「OnClick」というメソッドを持つスクリプトを探す
        MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>(true);

        foreach (var script in scripts)
        {
            MethodInfo method = script.GetType().GetMethod("OnClick", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                // 動的に呼び出すActionを作成
                clickAction = (Action)Delegate.CreateDelegate(typeof(Action), script, method);
                break;
            }
        }
    }

   
}
