using TMPro;
using UnityEngine;

public class VolumeData : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MasterText;
    [SerializeField] TextMeshProUGUI BgmText;
    [SerializeField] TextMeshProUGUI SeText;

    public static VolumeData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 設定のロード
        LoadInt();
        TextSet("ALL");
    }

    // それぞれの音量設定
    private int masterVolume = 10;
    private int bgmVolume = 10;
    private int seVolume = 10;

    private const int MAX = 10;
    private const int MIN = 0;
    // 外部から扱うゲッターセッター
    public int SetVol_Master
    {
        set
        {
            if (value == 1)
            {
                if (masterVolume == MAX) { return; }
                masterVolume += value;
            }
            else if(value == -1)
            {
                if (masterVolume == MIN) { return; }
                masterVolume += value;
            }
            // マスターボリュームだけはここで設定
            // 他は再生時にPlayerPrefsを読み込んでもらう
            TextSet("MASTER");
            AudioListener.volume = 0.1f * (float)masterVolume;
        }
        get { return masterVolume; }
    }
    public int SetVol_BGM
    {
        set 
        {
            if (value == 1)
            {
                if (bgmVolume == MAX) { return; }
                bgmVolume += value;
            }
            else if (value == -1)
            {
                if (bgmVolume == MIN) { return; }
                bgmVolume += value;
            }
            TextSet("BGM");
            // コンポーネントへ反映
            AudioManager.Instance.SetBGMVolume((float)PlayerPrefs.GetInt("bgmVolume")*0.1f);
        }
        get { return bgmVolume; }
    }
    public int SetVol_SE
    {
        set 
       {
            if (value == 1)
            {
                if (seVolume == MAX) { return; }
                seVolume += value;
            }
            else if (value == -1)
            {
                if (seVolume == MIN) { return; }
                seVolume += value;
            }
            TextSet("SE");
            // コンポーネントへ反映
            AudioManager.Instance.SetSEVolume((float)PlayerPrefs.GetInt("seVolume") * 0.1f);
        }
        get { return seVolume; }
    }

    // UIに反映
    private void TextSet(string VolType)
    {
        switch (VolType)
        {
            case "SE":
                // UIに反映
                SeText.text = seVolume.ToString();
                // セーブ
                SaveINT(VolType);
                break;
            case "BGM":
                BgmText.text = bgmVolume.ToString();
                SaveINT(VolType);
                break;
            case "MASTER":
                MasterText.text = masterVolume.ToString();
                SaveINT(VolType);
                break;
            case "ALL": // 最初のセットだけに使う
                MasterText.text = masterVolume.ToString();
                BgmText.text = bgmVolume.ToString();
                SeText.text = seVolume.ToString();
                break;
        }
    }

    // PlayerPrefsを使ったセーブ機能
    private void SaveINT(string VolType)
    {
        switch (VolType)
        {
            case "SE":
                PlayerPrefs.SetInt("seVolume", seVolume);
                break;
            case "BGM":
                PlayerPrefs.SetInt("bgmVolume", bgmVolume);
                break;
            case "MASTER":
                PlayerPrefs.SetInt("masterVolume", masterVolume);
                break;
        }
    }

    // PlayerPrefsを使ったロード機能
    private void LoadInt()
    {
        masterVolume = PlayerPrefs.GetInt("masterVolume", 10);
        bgmVolume = PlayerPrefs.GetInt("bgmVolume", 10);
        seVolume = PlayerPrefs.GetInt("seVolume", 10);
    }
}
