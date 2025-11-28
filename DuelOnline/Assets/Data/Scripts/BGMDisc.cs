using UnityEngine;


public class BGMDisc : MonoBehaviour
{
    [Header("‰¹Šy”Ô†w’è"),SerializeField] private int playBGM = 0;
    private void Awake()
    {
        // BGMÄ¶
        AudioManager.Instance.PlayBGM(playBGM);
    }
}
