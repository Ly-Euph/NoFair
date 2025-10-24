using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    // UI�֘A�̕�
    #region CharacterUI
    const int GaugeBASE = 4; // �Q�[�W�z��̐�
    [SerializeField] Image[] hpImg = new Image[GaugeBASE]; // �̗̓Q�[�W
    [SerializeField] Image[] mpImg = new Image[GaugeBASE]; // �}�i�Q�[�W
    [SerializeField] Image[] EmhpImg = new Image[GaugeBASE]; // ����̗̑̓Q�[�W
    /* �C���[�W�̃J���[�w��        */
    /* true�͌��݂̐��l�̕\��      */
    /* false�͎����Ă��鐔�l�̕\�� */
    Color hpCol_true = new Color(0f / 255f, 255f / 255f, 27f / 255f);
    Color hpCol_false = new Color(215f / 255f, 37f / 255f, 41f / 255f);
    Color mpCol_true = new Color(136f / 255f, 194f / 255f, 255f / 255f);
    Color mpCol_false = new Color(137f / 255f, 120f / 255f, 120f / 255f);
    #endregion

    private int prevHp = -1;
    private int prevMp = -1;
    private int prevEmHp = -1;

    // UI�X�V
    private void Update()
    {
        var plhp = DataSingleton_Offline.Instance.PlHP;
        var plmp = DataSingleton_Offline.Instance.PlMP;
        var emhp = DataSingleton_Offline.Instance.EmHP;

        if (plhp != prevHp || plmp != prevMp || emhp != prevEmHp)
        {
            ImageUpdate(plhp, plmp, emhp);
            prevHp = plhp;
            prevMp = plmp;
            prevEmHp = emhp;
        }
    }

    // �C���[�W�̐؂�ւ�
    private void ImageUpdate(int hp,int mp,int emhp)
    {
        // ���l�̕�����true�J���[�ɂ���
        for (int i = 0; i < GaugeBASE; i++)
        {
            hpImg[i].color = (i < hp) ? hpCol_true : hpCol_false;
            EmhpImg[i].color = (i < emhp) ? hpCol_true : hpCol_false;
            mpImg[i].color = (i < mp) ? mpCol_true : mpCol_false;
        }
    }
}
