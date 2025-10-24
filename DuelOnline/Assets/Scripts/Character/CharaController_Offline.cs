using UnityEngine;
public class CharaController_Offline : CharacterBase
{
    // �d�����Ԍv�Z
    float seconds = 0;
    // �d���t���O
    bool Delayflg = false;
    // �d�����Ԃ�������悤��
    [SerializeField] Material material;
    [SerializeField] float r, g, b;
    Color ActionCol = new Color(186f / 255f, 0f / 255f, 255f / 255f); // �s�����̃J���[
    Color DefCol; // �s���\���
    private void Awake()
    {
        // �C���X�y�N�^�[�Őݒ肵���l�����Ƀf�t�H���g�J���[�����߂�
        DefCol = new Color(r / 255f, g / 255f, b / 255f);
        // �J���[�`�F���W
        material.color = DefCol;
        // �A�C�h�����
        animNum = 0;
        AnimSet();
    }
    public void Update()
    {
        // �d������
        if (Delayflg) {
            ResetFlag();
            Debug.Log("�d����");
            return;
        }
        InputController();
        Debug.Log(mp);
    }
    public override void SAttack()
    {
        animNum = 1;
        // MP�`�F�b�N
        if (mp <= 0) { return; }
        else { mp--; }
        // ���f
        DataSingleton_Offline.Instance.PlMP = mp;
        AnimSet();
    }
    public override void LAttack()
    {
        animNum = 2;
        // MP�`�F�b�N
        if (mp <= 2) { return; }
        else { mp = mp - 3; }
        // ���f
        DataSingleton_Offline.Instance.PlMP = mp;
        AnimSet();
    }
    public override void Charge()
    {
        animNum = 3;
        // MP�`�F�b�N
        if (mp >= MAX) { return; }
        else { mp++; }
        // ���f
        DataSingleton_Offline.Instance.PlMP = mp;
        AnimSet();
    }
    public override void Block()
    {
        animNum = 4;
        AnimSet();
    }
    public override void Damage()
    {
        animNum = 5;
        AnimSet();
    }

    void StartAnim(float frame)
    {
        Delayflg = true;
        Debug.Log("�A�j���[�V�����̊J�n");
        Debug.Log(Delayflg+"A");
        seconds = frame / 60.0f;
        // �J���[�`�F���W�i�d�����j
        material.color = ActionCol;
    }
    // �d�����Ԃ̌v�Z
    void ResetFlag()
    {
        seconds -= Time.deltaTime;
        if(seconds<0)
        {
            // �J���[�`�F���W
            material.color = DefCol;
            Delayflg = false;
        }
    }

}
