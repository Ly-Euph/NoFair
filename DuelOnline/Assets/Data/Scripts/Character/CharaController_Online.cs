using UnityEngine;
using Fusion;

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

    // 同期用変数
    [Networked] public int AnimNum { get; set; }
    private int _prevAnimNum = -1;


    // プレイヤー１ならtrue
    // プレイヤー1になるのはHost
    [SerializeField] bool isPlayer1 = true;

    public bool GetisPlnum => isPlayer1;

    [SerializeField]CharaController_Online Pl1;
    [SerializeField]CharaController_Online Pl2;

    void Start()
    {
        Debug.Log($"[{name}] HasInputAuthority={Object.HasInputAuthority} PlayerRef={Runner.LocalPlayer}");
    }

    public override void Spawned()
    {
        // ローカルの入力クリアはここで
        if (Object.HasInputAuthority)
        {
            animNum = 0;
            AnimNum = 0;
        }
        hp = 4;
        // インスペクターで設定した値を元にデフォルトカラーを決める
        DefCol = new Color(r / 255f, g / 255f, b / 255f);
        // カラーチェンジ
        material.color = DefCol;
        // アイドル状態
        animNum = 0;
        AnimSet(animNum);
        // 防御判定を消しておく
        DefColBox.enabled = false;
        // 自分のキャラじゃなければ return
        if (!Object.HasInputAuthority){ return; }
        SetDepth();
    }
    public void SetDepth()
    {
        // 最優先カメラ
        plCam.depth = 10;
    }

    private void Update()
    {
        if (!Object.HasInputAuthority || Delayflg) { return; }
        InputController();
    }

    // ------------------------------------------------------------
    // FixedUpdateNetwork：FusionのTickごとに同期される処理
    // 全端末が同じ Tick で同じコードを実行する
    // → アニメ遅延・魔法遅延ゼロ
    // ------------------------------------------------------------
    public override void FixedUpdateNetwork()
    {
        // 死亡 or 準備中は何もさせない
        if (isDead || !DataSingleton_Online.Instance.IsReady)
            return;

        // HP同期はホストが書き込む
        // クライアント側は読み込む
        SyncHP();

        // 硬直中は待つだけ（Networked 変数は触らない）
        if (Delayflg)
        {
            ResetFlag();
            return;
        }

        // --------------------------------
        // 入力取得（自分の入力だけ）
        // --------------------------------
        if (Object.HasStateAuthority && GetInput(out NetworkInputData inputData))
        {
            // ホストなら Networked に書き込む
            if (inputData.ActionID != 0)
            {
                // 入力送信
                AnimNum = inputData.ActionID;
                RPC_PlayAnim(AnimNum);

                // リセット処理
                animNum = 0;
                GameLauncher.Instance.SetInputNum(animNum);

                Debug.Log("入力情報送信済み");
            }
        }
    }

  
    // ------------------------------------------------------------
    // HP同期処理（DataNetRelay使用）
    // Host = StateAuthority が書く
    // Client はそれを読む
    // ------------------------------------------------------------
    private void SyncHP()
    {
        var relay = DataNetRelay.Instance;
        var data = DataSingleton_Online.Instance;

        if (Object.HasStateAuthority)
        {
            // 死亡チェックは一度だけ
            if (!isDead && hp <= 0)
            {
                isDead = true;

                // 死亡アニメ通知（1回だけ）
                RPC_PlayAnim(6);
            }
            if (isPlayer1)
            {
                relay.Player1HP = hp;
                data.PlHP = hp;
            }
            else
            {
                relay.Player2HP = hp;
                data.EmHP = hp;
            }
        }
        else // クライアント側
        {
            hp = relay.Player2HP;
            data.EmHP = relay.Player1HP;
            data.PlHP = hp;
        }
    }

    public override void SAttack()
    {
        // MPチェック
        if (mp <= 0) { return; }
        else { mp-=1; }
        animNum = 1;
        DataSingleton_Online.Instance.PlMP = mp;
    }
    public override void LAttack()
    {
        // MPチェック
        if (mp <= 2) { return; }
        else { mp = mp - 3; }
        animNum = 2;
        DataSingleton_Online.Instance.PlMP = mp;
    }
    public override void Charge()
    {
        // MPチェック
        if (mp >= MAX) { return; }
        else { mp+=1; }
        animNum = 3;
        DataSingleton_Online.Instance.PlMP = mp;
    }
    public override void Block()
    {
        animNum = 4;
    }
    /// <summary>
    /// ダメージヒット処理
    /// </summary>
    /// <param name="isSmall">弱魔法ならtrue</param>
    public override void Damage(bool isSmall)
    {
        // 死んでいた場合は動かさない
        // クライアント側も動かさない
        if (isDead||!Object.HasStateAuthority) { return; }
        // 弾の判定
        if (isSmall)
        {
            hp--;
        }
        else
        {
            hp -= 1;
        }
        // HP送信
        if (isPlayer1)
        {
            DataNetRelay.Instance.RPC_SetHP("Player1", hp);
        }
        else
        {
            DataNetRelay.Instance.RPC_SetHP("Player2", hp);
        }
        RPC_PlayAnim(5);
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
        seconds -= Runner.DeltaTime;
        if (seconds < 0)
        {
            animNum = 0;
            AnimNum = 0;
            GameLauncher.Instance.SetInputNum(animNum);
            RPC_ResetFlag();
        }
    }

    // RPC関連通信処理
    // アニメーションセット
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayAnim(int animId)
    {
        AnimSet(animId);
    }
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_ResetFlag()
    {
        // カラーチェンジ
        material.color = DefCol;
        Delayflg = false;
    }
}
