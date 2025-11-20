using UnityEngine;

public class Button_Close : MonoBehaviour
{
    [Header("非表示にするオブジェクト"), SerializeField] GameObject closeObj;
    public void OnClick()
    {
        // セッションから抜けておく
        GameLauncher.Instance.LeaveSession();
        closeObj.SetActive(false);
        Debug.Log("AAA");
    }
}
