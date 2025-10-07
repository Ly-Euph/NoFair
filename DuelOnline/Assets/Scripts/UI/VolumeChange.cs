using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChange : MonoBehaviour
{
    enum KEY
    {
        MASTER,BGM,SE
    }
    [SerializeField] bool IsUP = false; // âπó Çè„Ç∞ÇÈÇÃÇ©
    [Header("âπó ÇÃéÌóﬁ"),SerializeField] KEY volKey;
    private int KEYNUM;

    public void OnClick()
    {
        KEYNUM = IsUP == true? 1 : -1;

        switch(volKey)
        {
            case KEY.MASTER:
                VolumeData.Instance.SetVol_Master = KEYNUM;
                break;
            case KEY.BGM:
                VolumeData.Instance.SetVol_BGM = KEYNUM;
                break;
            case KEY.SE:
                VolumeData.Instance.SetVol_SE = KEYNUM;
                break;
        }
    }
}
