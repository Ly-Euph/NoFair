using System.Collections.Generic;
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

    // NPCかどうか基本的にはfalseかな
    [SerializeField]bool isBot = false;
    [SerializeField] bool isHostPlayer = false;
    
    #region Bot
    private delegate void ActionDelegate();
    private ActionDelegate[] actions;
    private float[] actionProbabilities; // 各行動の割合（合計1.0）
    private float timer = 0;
    // NPC専用で行動時間を制御する
    private bool isAct = true;
    #endregion
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

        // Botのみの設定
        if (isBot)
        {
            BotAwake();
        }
    }
    public void Update()
    {
        // 操作不能
        /* ----条件---- */
        // 自分の死亡時、相手の死亡時
        // ゲームマネージャーの対戦合図待ち
        if (isDead||!DataSingleton_Offline.Instance.IsReady) { return; }
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
        else if(DataSingleton_Offline.Instance.PlHP>=1&&DataSingleton_Offline.Instance.EmHP>=1)
        {
            // 硬直時間
            if (Delayflg)
            {
                ResetFlag();
                Debug.Log("硬直中");
                return;
            }
            if (isBot)
            {
                if (isAct)
                {
                    CTTimer();
                    return;
                }
                PerformAction();
                isAct = true;
            }
            else
            {
                InputController();
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
        // 反映
        if (isHostPlayer)
        {
            DataSingleton_Offline.Instance.PlMP = mp;
        }
        AnimSet(animNum);
    }
    public override void LAttack()
    {
        animNum = 2;
        // MPチェック
        if (mp <= 2) { return; }
        else { mp = mp - 3; }
        // 反映
        if (isHostPlayer)
        {
            DataSingleton_Offline.Instance.PlMP = mp;
        }
        AnimSet(animNum);
    }
    public override void Charge()
    {
        animNum = 3;
        // MPチェック
        if (mp >= MAX) { return; }
        else { mp++; }
        // 反映
        if (isHostPlayer)
        {
            DataSingleton_Offline.Instance.PlMP = mp;
        }
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
        if (isHostPlayer)
        {
            DataSingleton_Offline.Instance.PlHP = hp;
        }
        else
        {
            DataSingleton_Offline.Instance.EmHP = hp;
        }
        AnimSet(animNum);
    }

    // アニメーション起動時のイベント
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

    #region BotMethod
    private void BotAwake()
    {
        // 行動を配列に登録
        actions = new ActionDelegate[] { SAttack, LAttack, Charge, Block };

        // 各行動の基本確率
        actionProbabilities = new float[] { 0.3f, 0.2f, 0.3f, 0.2f };
    }
    private void PerformAction()
    {
        // 状況に応じて確率を更新
        UpdateProbabilities();

        // 使用可能な行動をリストアップ
        var validActions = new List<ActionDelegate>();
        var validProbs = new List<float>();

        for (int i = 0; i < actions.Length; i++)
        {
            if (IsActionUsable(i))
            {
                validActions.Add(actions[i]);
                validProbs.Add(actionProbabilities[i]);
            }
        }

        // 確率に応じてランダム選択
        float total = 0;
        foreach (var p in validProbs) total += p;
        float rand = UnityEngine.Random.value * total;
        float sum = 0;
        for (int i = 0; i < validActions.Count; i++)
        {
            sum += validProbs[i];
            if (rand <= sum)
            {
                validActions[i].Invoke();
                break;
            }
        }
    }

    /// <summary>
    /// 状況に応じて確率を更新
    /// </summary>
    private void UpdateProbabilities()
    {
        float sAttack = 0.3f;
        float lAttack = 0.2f;
        float charge = 0.3f;
        float block = 0.2f;

        // --- HPが少ないほど防御・強攻撃寄りに ---
        if (hp <= 2)
        {
            sAttack = 0.2f;
            lAttack = 0.3f;
            charge  = 0.3f;
            block   = 0.2f;
        }
        // --- MPが少ないほどチャージ優先 ---
        if (mp < 1)
        {
            sAttack = 0.2f;
            lAttack = 0.1f;
            charge  = 0.4f;
            block   = 0.3f;
        }

        // 正規化（合計1.0にする）
        float total = sAttack + lAttack + charge + block;
        actionProbabilities[0] = sAttack / total;
        actionProbabilities[1] = lAttack / total;
        actionProbabilities[2] = charge / total;
        actionProbabilities[3] = block / total;
    }

    private bool IsActionUsable(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0: return mp >= 1; // SAttack
            case 1: return mp >= 3; // LAttack
            case 2: return true;    // Charge
            case 3: return true;    // Block
            default: return false;
        }
    }

    // クールタイム管理
    private void CTTimer()
    {
        timer += Time.deltaTime;
        if (timer >= 0.7f)
        {
            timer = 0;
            isAct = false;
        }
    }
    #endregion
}
