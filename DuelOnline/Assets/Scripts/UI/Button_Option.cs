using UnityEngine;

public class Button_Option : MonoBehaviour
{
    [Header("�\���ɂ���I�u�W�F�N�g"), SerializeField] GameObject closeObj;
    public void OnClick()
    {
        closeObj.SetActive(true);
        Debug.Log("AAA");
    }
}
