using UnityEngine;

public class Button_Close : MonoBehaviour
{
    [Header("マルチのキャンセルボタンかどうか"),SerializeField] bool isMulti = false;
    [Header("非表示にするオブジェクト"), SerializeField] GameObject closeObj;
    public void OnClick()
    {
        if (isMulti)
        {
            // セッションから抜けておく
            GameLauncher.Instance.LeaveSession();
        }
        else
        {
            closeObj.SetActive(false);
        }
        Debug.Log("AAA");
    }
}
