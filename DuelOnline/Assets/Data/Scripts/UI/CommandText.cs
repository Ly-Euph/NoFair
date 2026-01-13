using System.Collections;
using TMPro;
using UnityEngine;

public class CommandText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI commandText;

    private Coroutine textCoroutine;
    private const float TIME = 1.0f;
    // 入力したコマンドを表示する
    public void InputCommand(string text)
    {
        commandText.text = text;

        // すでに動いていたら止める
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }

        textCoroutine = StartCoroutine(ResetText());
    }

    private IEnumerator ResetText()
    {
        yield return new WaitForSeconds(TIME);
        commandText.text = "";
        textCoroutine = null;
    }
}
