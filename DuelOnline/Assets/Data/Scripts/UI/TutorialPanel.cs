using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    // オブジェクト
    [SerializeField] GameObject RuleBox;
    [SerializeField] GameObject BlockBox;
    [SerializeField] GameObject ChargeBox;
    [SerializeField] GameObject FireBox;
    [SerializeField] GameObject IceBox;

    private GameObject[] BoxTbl;

    [SerializeField] RawImage rawImgObj;


    // クリップ名を保管
    private string video;
    // ビデオクリップを取得する
    [SerializeField] VideoPlayer videoPlayer;
    private void Awake()
    {
        BoxTbl = new GameObject[]
        {
        RuleBox,
        BlockBox,
        ChargeBox,
        FireBox,
        IceBox
        };
    }

    /// <summary>
    /// 遊び方ボタンを押したときに表示するパネルを初期化すること
    /// </summary>
    public void SettingPanel()
    {
        if (BoxTbl == null ) {
         BoxTbl = new GameObject[]
         {
           RuleBox,
           BlockBox,
           ChargeBox,
           FireBox,
           IceBox
          };
        }
        rawImgObj.color = new Color(1, 1, 1, 0);
        // 入ってる動画も消しておく
        videoPlayer.clip = null;
        video = "";
        // ルールを表示する
        ActiveFunc(0);
    }

    public void ActiveBox()
    {
        // 自身のクリップ名から表示するオブジェクトを指定する

        if (videoPlayer.clip== null) { video = ""; }
        else
        {
            video = videoPlayer.clip.name;
        }
        switch (video)
        {
            case "":
                rawImgObj.color = new Color(1, 1, 1, 0);
                ActiveFunc(0);
                break;
            case "Block":
                ActiveFunc(1);
                break;
            case "Charge":
                ActiveFunc(2);
                break;
            case "Fire":
                ActiveFunc(3);
                break;
            case "Ice":
                ActiveFunc(4);
                break;
        }
    }

    // 全て表示後に特定の物を表示にする
    private void ActiveFunc(int number)
    {
        // その他
        // オブジェクトが非表示ならば
        if (rawImgObj.color.a == 0&&number!=0)
        {
            rawImgObj.color = new Color(1, 1, 1, 1);
        }
        for (int i = 0; i < BoxTbl.Length; i++)
        {
            BoxTbl[i].SetActive(false);
        }
        BoxTbl[number].SetActive(true);
    }
}