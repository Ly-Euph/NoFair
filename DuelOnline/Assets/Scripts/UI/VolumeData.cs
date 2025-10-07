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

        TextSet();
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
        }
        get { return seVolume; }
    }

    private void FixedUpdate()
    {
        TextSet();
    }

    private void TextSet()
    {
        MasterText.text = masterVolume.ToString();
        BgmText.text = bgmVolume.ToString();
        SeText.text = seVolume.ToString();
    }
}
