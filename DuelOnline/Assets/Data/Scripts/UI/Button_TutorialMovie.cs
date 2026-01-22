using UnityEngine;
using UnityEngine.Video;

public class Button_TutorialMovie : MonoBehaviour
{
    // ここに動画が表示される
    [SerializeField] GameObject rawImgObj;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip[] videoClip;
    // ボタン処理
    public void OnClick()
    {
        // オブジェクトが非表示ならば
        if (!rawImgObj.activeSelf) { rawImgObj.SetActive(true); }
        videoPlayer.clip= videoClip[0];
        videoPlayer.Play();
        Debug.Log("AAA");
    }
}
