using UnityEngine;
using UnityEngine.Video;

public class TutorialPanel : MonoBehaviour
{
    // オブジェクト
    [SerializeField] GameObject RuleBox;
    [SerializeField] GameObject BlockBox;
    [SerializeField] GameObject ChargeBox;
    [SerializeField] GameObject FireBox;
    [SerializeField] GameObject IceBox;

    private GameObject[] BoxTbl;

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
    public void ActiveBox()
    {
        // 自身のクリップ名から表示するオブジェクトを指定する

        if (videoPlayer.clip.name == null) { video = ""; }
        else{ video = videoPlayer.clip.name; }
        switch (video)
        {
            case "":
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
        for (int i = 0; i < BoxTbl.Length; i++)
        {
            BoxTbl[i].SetActive(false);
        }
        BoxTbl[number].SetActive(true);
    }
}