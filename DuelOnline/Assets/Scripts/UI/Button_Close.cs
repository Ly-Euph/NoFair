using UnityEngine;

public class Button_Close : MonoBehaviour
{
    [Header("��\���ɂ���I�u�W�F�N�g"), SerializeField] GameObject closeObj;
    public void OnClick()
    {
        closeObj.SetActive(false);
        Debug.Log("AAA");
    }
}
