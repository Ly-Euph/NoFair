using UnityEngine;

public class Button_Option : MonoBehaviour
{
    [Header("表示にするオブジェクト"), SerializeField] GameObject closeObj;
    public void OnClick()
    {
        closeObj.SetActive(true);
        Debug.Log("AAA");
    }
}
