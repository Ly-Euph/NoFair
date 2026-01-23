using UnityEngine;
using UnityEngine.Video;
public class Button_TutorialMovie : MonoBehaviour
{
    // ここに動画が表示される
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip videoClip;

    [SerializeField] TutorialPanel panel;
    // ボタン処理
    public void OnClick()
    {
        // ルールのみこっち（動画じゃないから）
        if (videoClip == null) {
            videoPlayer.clip = null; // 空にする
            panel.ActiveBox();
            return; 
        }
        // 設定
        videoPlayer.clip= videoClip;
        // 説明とかのテキストパネルボックスを表示
        panel.ActiveBox();
        // 動画再生
        videoPlayer.Play();
        Debug.Log("AAA");
    }
}
