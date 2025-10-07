using UnityEngine;

public class Button_Tutorial : MonoBehaviour
{
    [Header("表示にするオブジェクト"), SerializeField] GameObject closeObj;
    public void OnClick()
    {
        closeObj.SetActive(true);
        Debug.Log("AAA");
    }
}
