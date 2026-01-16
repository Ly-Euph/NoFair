using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    // UI関連の物
    #region CharacterUI
    const int GaugeBASE = 4; // ゲージ配列の数
    [SerializeField] Image[] hpImg = new Image[GaugeBASE]; // 体力ゲージ
    [SerializeField] Image[] mpImg = new Image[GaugeBASE]; // マナゲージ
    [SerializeField] Image[] EmhpImg = new Image[GaugeBASE]; // 相手の体力ゲージ
    /* イメージのカラー指定        */
    /* trueは現在の数値の表現      */
    /* falseは失っている数値の表現 */
    Color hpCol_true = new Color(0f / 255f, 255f / 255f, 27f / 255f);
    Color hpCol_false = new Color(215f / 255f, 37f / 255f, 41f / 255f);
    Color mpCol_true = new Color(136f / 255f, 194f / 255f, 255f / 255f);
    Color mpCol_false = new Color(137f / 255f, 120f / 255f, 120f / 255f);
    #endregion
    // 時間表示
    [SerializeField] TextMeshProUGUI timerText;

    private int prevHp = -1;
    private int prevMp = -1;
    private int prevEmHp = -1;

    // UI更新
    private void Update()
    {
        var plhp = DataSingleton_Online.Instance.PlHP;
        var plmp = DataSingleton_Online.Instance.PlMP;
        var emhp = DataSingleton_Online.Instance.EmHP;

        if (plhp != prevHp || plmp != prevMp || emhp != prevEmHp)
        {
            ImageUpdate(plhp, plmp, emhp);
            prevHp = plhp;
            prevMp = plmp;
            prevEmHp = emhp;
        }
    }

    // イメージの切り替え
    private void ImageUpdate(int hp,int mp,int emhp)
    {
        // 数値の分だけtrueカラーにする
        for (int i = 0; i < GaugeBASE; i++)
        {
            hpImg[i].color = (i < hp) ? hpCol_true : hpCol_false;
            EmhpImg[i].color = (i < emhp) ? hpCol_true : hpCol_false;
            mpImg[i].color = (i < mp) ? mpCol_true : mpCol_false;
        }
    }

    /// <summary>
    /// 勝利者をテキスト表示
    /// </summary>
    /// <param name="isPlayer1">Player1=true,Player2=false</param>
    public void WinEff(bool isPlayer1)
    {
        // コンポーネント取得
        TextMeshProUGUI tmpText = GameObject.Find("GameText").GetComponent<TextMeshProUGUI>();
        // テキストカラーを表示状態へ
        tmpText.alpha = 1f;

        string plName = isPlayer1 ? "Player1" : "Player2"; // プレイヤーの判定
        string globalText = "WIN";// ここは共通テキスト
        string FusionText = plName + globalText;

        // 表示用コルーチン
        StartCoroutine(ShowWinSequence(tmpText, FusionText));
    }
    public void TimeOver()
    {
        // コンポーネント取得
        TextMeshProUGUI tmpText = GameObject.Find("GameText").GetComponent<TextMeshProUGUI>();
        string text = "TIME'S UP";
        tmpText.text=text;
        // テキストカラーを表示状態へ
        tmpText.alpha = 1f;
    }
    public void TimerTextUpdate(float timer)
    {
        // テキストへ反映
        timerText.text = Mathf.Floor(timer).ToString();
    }

    /// <summary>
    /// K.O → Player名＋改行＋WIN を一文字ずつ順に表示
    /// </summary>
    private IEnumerator ShowWinSequence(TextMeshProUGUI tmp, string fusionText)
    {
        // まずクリア
        tmp.text = "";

        // --- ① K.O を1文字ずつ表示 ---
        string ko = "K.O";
        foreach (char c in ko)
        {
            tmp.text += c;
            yield return new WaitForSeconds(0.05f); // 表示速度
        }

        // 少し待つ
        yield return new WaitForSeconds(0.5f);

        // --- ② FusionText をクリアして1文字ずつ表示 ---
        tmp.text = "";
        foreach (char c in fusionText)
        {
            if (c == 'W') { tmp.text+="\n"; }
            tmp.text += c;
            yield return new WaitForSeconds(0.05f); // 表示速度
        }
    }
}
