using UnityEngine;

public class Button_Match : MonoBehaviour
{
    FadeInOut fadeScr; // フェード機能
    public void OnClick()
    {
        fadeScr = FadeInOut.CreateInstance();

        fadeScr.LoadScene("Game");
    }
}
