using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class Button_TutorialMovie : MonoBehaviour
{
    // ここに動画が表示される
    [SerializeField] RawImage rawImgObj;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip[] videoClip;

    [SerializeField] TutorialPanel panel;
    // ボタン処理
    public void OnClick()
    {
        // ルールのみこっち（動画じゃないから）
        if (videoClip.Length == 0) {
            videoPlayer.clip = null; // 空にする
            rawImgObj.color=new Color(1,1,1,0);
            panel.ActiveBox();
            return; 
        }

        // その他
        // オブジェクトが非表示ならば
        if (rawImgObj.color.a == 0)
        {
            rawImgObj.color = new Color(1, 1, 1, 1);
        }
        videoPlayer.clip= videoClip[0];
        panel.ActiveBox();
        videoPlayer.Play();
        Debug.Log("AAA");
    }
}
