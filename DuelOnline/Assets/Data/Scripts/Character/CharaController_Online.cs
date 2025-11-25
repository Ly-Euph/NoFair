using UnityEngine;

public class CharaController_Online : CharacterBase
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

    // プレイヤー１ならtrue
    // プレイヤー1になるのはHost
    [SerializeField] bool isPlayer1 = true;

    int nowAnim = 0;

    [SerializeField]CharaController_Online Pl1;
    [SerializeField]CharaController_Online Pl2;
    private void Awake()
    {
        // インスペクターで設定した値を元にデフォルトカラーを決める
        DefCol = new Color(r / 255f, g / 255f, b / 255f);
        // カラーチェンジ
        material.color = DefCol;
        // アイドル状態
        animNum = 0;
        AnimSet(animNum);
        // 防御判定を消しておく
        DefColBox.enabled = false;
    }
    public void Update()
    {
        // 操作不能
        /* ----条件---- */
        // 自分の死亡時、相手の死亡時
        // ゲームマネージャーの対戦合図待ち
        if (isDead || !DataSingleton_Online.Instance.IsReady) { return; }
        // HPなくなったら
        if (hp <= 0)
        {
            animNum = 6;
            AnimSet(animNum);
            // カラーチェンジ
            material.color = ActionCol;
            isDead = true;
        }
        // お互いのプレイヤーが生存しているのなら
        else if (DataSingleton_Online.Instance.PlHP >= 1 && DataSingleton_Online.Instance.EmHP >= 1)
        {
            // 硬直時間
            if (Delayflg)
            {
                ResetFlag();
                Debug.Log("硬直中");
                return;
            }
            else
            {
                if (isPlayer1)
                {
                    if (GameLauncher.Instance.GetisHost)
                    {
                        InputController();
                        DataNetRelay.Instance.RPC_SetAnim("Player1", animNum);
                        if (DataNetRelay.Instance.Player2AnimNum != nowAnim)
                        {
                            nowAnim = DataNetRelay.Instance.Player2AnimNum;
                            Pl2.AnimSet(nowAnim);
                        }
                    }
                }
                else
                {
                    if(!GameLauncher.Instance.GetisHost)
                    {
                        InputController();
                        DataNetRelay.Instance.RPC_SetAnim("Player2", animNum);
                        if (DataNetRelay.Instance.Player1AnimNum != nowAnim)
                        {
                            nowAnim = DataNetRelay.Instance.Player1AnimNum;
                            Pl1.AnimSet(nowAnim);
                        }
                    }
                }
                    Debug.Log(mp);
            }
        }
    }

    public override void SAttack()
    {
        animNum = 1;
        // MPチェック
        if (mp <= 0) { return; }
        else { mp--; }
        DataSingleton_Online.Instance.PlMP = mp;
        AnimSet(animNum);
    }
    public override void LAttack()
    {
        animNum = 2;
        // MPチェック
        if (mp <= 2) { return; }
        else { mp = mp - 3; }
        DataSingleton_Online.Instance.PlMP = mp;
        AnimSet(animNum);
    }
    public override void Charge()
    {
        animNum = 3;
        // MPチェック
        if (mp >= MAX) { return; }
        else { mp++; }
        DataSingleton_Online.Instance.PlMP = mp;
        AnimSet(animNum);
    }
    public override void Block()
    {
        animNum = 4;
        AnimSet(animNum);
    }
    /// <summary>
    /// ダメージヒット処理
    /// </summary>
    /// <param name="isSmall">弱魔法ならtrue</param>
    public override void Damage(bool isSmall)
    {
        // 死んでいた場合は動かさない
        if (isDead) { return; }
        animNum = 5;
        if (isSmall)
        {
            hp--;
        }
        else
        {
            hp -= 1;
        }
        // UI反映
        if (isPlayer1)
        {
            DataSingleton_Online.Instance.PlHP = hp;
        }
        else
        {
            DataSingleton_Online.Instance.EmHP = hp;
        }
        AnimSet(animNum);
    }

    // アニメーション起動時のイベント
    void StartAnim(float frame)
    {
        Delayflg = true;
        Debug.Log("アニメーションの開始");
        Debug.Log(Delayflg + "A");
        seconds = frame / 60.0f;
        // カラーチェンジ（硬直中）
        material.color = ActionCol;
    }
    // 硬直時間の計算
    void ResetFlag()
    {
        seconds -= Time.deltaTime;
        if (seconds < 0)
        {
            animNum = 0;
            // カラーチェンジ
            material.color = DefCol;
            Delayflg = false;
        }
    }

}
