using UnityEngine;
public class CharaController_Offline : CharacterBase
{
    // 硬直時間計算
    float seconds = 0;
    // 硬直フラグ
    bool Delayflg = false;
    // 硬直時間が分かるように
    [SerializeField] Material material;
    [SerializeField] float r, g, b;
    Color ActionCol = new Color(186f / 255f, 0f / 255f, 255f / 255f); // 行動中のカラー
    Color DefCol; // 行動可能状態
    private void Awake()
    {
        // インスペクターで設定した値を元にデフォルトカラーを決める
        DefCol = new Color(r / 255f, g / 255f, b / 255f);
        // カラーチェンジ
        material.color = DefCol;
        // アイドル状態
        animNum = 0;
        AnimSet();
    }
    public void Update()
    {
        // 硬直時間
        if (Delayflg) {
            ResetFlag();
            Debug.Log("硬直中");
            return;
        }
        InputController();
        Debug.Log(mp);
    }
    public override void SAttack()
    {
        animNum = 1;
        // MPチェック
        if (mp <= 0) { return; }
        else { mp--; }
        // 反映
        DataSingleton_Offline.Instance.PlMP = mp;
        AnimSet();
    }
    public override void LAttack()
    {
        animNum = 2;
        // MPチェック
        if (mp <= 2) { return; }
        else { mp = mp - 3; }
        // 反映
        DataSingleton_Offline.Instance.PlMP = mp;
        AnimSet();
    }
    public override void Charge()
    {
        animNum = 3;
        // MPチェック
        if (mp >= MAX) { return; }
        else { mp++; }
        // 反映
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
        Debug.Log("アニメーションの開始");
        Debug.Log(Delayflg+"A");
        seconds = frame / 60.0f;
        // カラーチェンジ（硬直中）
        material.color = ActionCol;
    }
    // 硬直時間の計算
    void ResetFlag()
    {
        seconds -= Time.deltaTime;
        if(seconds<0)
        {
            // カラーチェンジ
            material.color = DefCol;
            Delayflg = false;
        }
    }

}
