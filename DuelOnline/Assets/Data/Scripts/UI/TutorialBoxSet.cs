using UnityEngine;

public class TutorialBoxSet : MonoBehaviour
{
    // ‚±‚±‚É‰Šú‰»‚ÌŠÖ”‚ğŒÄ‚Ño‚·
    [SerializeField] TutorialPanel tutorialScr;
    private void OnEnable()
    {
        tutorialScr.SettingPanel();   
    }
}
