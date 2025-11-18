using UnityEngine;

public class Button_Match : MonoBehaviour
{
    public void OnClick()
    {
        FadePoolManager.Instance.GetFade().LoadScene("Game");
    }
}
