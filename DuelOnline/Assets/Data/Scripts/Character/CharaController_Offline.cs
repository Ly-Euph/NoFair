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

    // 画面効果エフェクト
    private EffectManager effectManager;

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

    // 様子見関連
    private bool isThinking = false;
    private float thinkTimer = 0f;

    [SerializeField] private float thinkMin = 0.2f;
    [SerializeField] private float thinkMax = 0.6f;

    private float currentThinkTime = 0f;

    private bool saveAct = false; 

    #region BotLearning

    // AIの学習データ
    private BotAISaveData aiData;

    // 保存ファイル名
    private const string AI_FILE_NAME = "BotAI.json";

    // 学習倍率（aiData.learnedBias の参照）
    private float[] learnedBias;

    // 直前に選択した行動（学習用）
    private int lastActionIndex = -1;

    #endregion
    #endregion
    // デバッグプレビュー
    [SerializeField]private bool isPreview = false; 
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
        effectManager = GetComponentInChildren<EffectManager>();
        // Botのみの設定
        if (isBot)
        {
            BotAwake();
        }
    }
    public void Update()
    {
        if (isPreview)
        {
            // 硬直時間
            if (Delayflg)
            {
                ResetFlag();
                Debug.Log("硬直中");
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))      { animNum = 1; Delayflg = true; }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) { animNum = 2; Delayflg = true; }
            else if (Input.GetKeyDown(KeyCode.Alpha3)) { animNum = 3; Delayflg = true; }
            else if (Input.GetKeyDown(KeyCode.Alpha4)) { animNum = 4; Delayflg = true; }
            else if (Input.GetKeyDown(KeyCode.Alpha5)) { animNum = 5; Delayflg = true; }

            AnimSet(animNum);
            animNum = 0;
            return;
        }

        // プレイヤー1P2P判断
        // BOT学習
        if (!saveAct&&isBot)
        {
            if (!isHostPlayer)
            {
                if (DataSingleton_Offline.Instance.PlHP <= 0) { LearnFromBattle(true); saveAct = true; }
            }
        }

            // 操作不能
            /* ----条件---- */
            // 自分の死亡時、相手の死亡時
            // ゲームマネージャーの対戦合図待ち
            if (isDead||!DataSingleton_Offline.Instance.IsReady) { return; }
        // HPなくなったら
        if (hp <= 0)
        {
            // BOT側の負け判定を記録
            if (isBot)
            {
                if (!isHostPlayer)
                {
                    if (DataSingleton_Offline.Instance.PlHP <= 0) { LearnFromBattle(false); saveAct = true; }
                }
            }
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
                // クールタイム中
                if (isAct)
                {
                    CTTimer();
                    return;
                }

                // 様子見中
                if (isThinking)
                {
                    ThinkTimer();
                    return;
                }

                // 行動開始
                PerformAction();
                isThinking = false;
                isAct = true;
            }
            else
            {
                InputController();
                Debug.Log(mp);
            }
        }

        // 再チェック
        // 防御判定
        if (animNum != 4&&DefColBox.enabled==true) {
            DefColBox.enabled = false;
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
        effectManager.CommandTextInput("アイスボール！発動");
        effectManager.PlayWeakMagic();
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
        effectManager.CommandTextInput("ファイアーボール！発動");
        effectManager.PlayStrongMagic();
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
        effectManager.CommandTextInput("マナチャージ！");
        effectManager.StartCharge();
    }
    public override void Block()
    {
        animNum = 4;
        AnimSet(animNum);
        effectManager.CommandTextInput("ブロック！");
        effectManager.PlayGuard();
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
            if (hp <= 1) { effectManager.SetLowHP(true); }
            DataSingleton_Offline.Instance.PlHP = hp;
        }
        else
        {
            DataSingleton_Offline.Instance.EmHP = hp;
        }
        AnimSet(animNum);
        effectManager.PlayDamage();
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
        // 行動登録
        actions = new ActionDelegate[]
        {
        SAttack,   // 0
        LAttack,   // 1
        Charge,    // 2
        Block      // 3
        };

        // ★ 行動確率配列を初期化（超重要）
        actionProbabilities = new float[4];

        // 学習データをロード
        aiData = BotAIStorage.Load(AI_FILE_NAME);

        // 学習倍率を参照
        learnedBias = aiData.learnedBias;
    }

    /// <summary>
    /// 学習結果を行動確率に反映する
    /// </summary>
    private void ApplyLearnedBias()
    {
        for (int i = 0; i < actionProbabilities.Length; i++)
        {
            actionProbabilities[i] *= learnedBias[i];
        }
    }

    private void PerformAction()
    {
        UpdateProbabilities();
        ApplyLearnedBias();

        var validActions = new List<ActionDelegate>();
        var validProbs = new List<float>();
        var validActionIndices = new List<int>();

        for (int i = 0; i < actions.Length; i++)
        {
            if (IsActionUsable(i))
            {
                validActions.Add(actions[i]);
                validProbs.Add(actionProbabilities[i]);
                validActionIndices.Add(i);
            }
        }

        float total = 0f;
        foreach (var p in validProbs) total += p;

        // フェイルセーフ
        if (total <= 0f)
        {
            lastActionIndex = 2; // Charge
            Charge();
            return;
        }

        float rand = UnityEngine.Random.value * total;
        float sum = 0f;

        for (int i = 0; i < validActions.Count; i++)
        {
            sum += validProbs[i];
            if (rand <= sum)
            {
                lastActionIndex = validActionIndices[i];
                validActions[i].Invoke();
                break;
            }
        }
    }

    /// <summary>
    /// バトル結果から学習する
    /// </summary>
    public void LearnFromBattle(bool win)
    {
        if (lastActionIndex < 0) return;

        float reward = win ? 0.1f : -0.1f;

        learnedBias[lastActionIndex] += reward;
        learnedBias[lastActionIndex] =
            Mathf.Clamp(learnedBias[lastActionIndex], 0.5f, 2.0f);
        aiData.battleCount++;
        AISave();
    }

    private void AISave()
    {
        // エディタでは保存しない
        if (isBot && !Application.isEditor)
        {
            BotAIStorage.Save(AI_FILE_NAME, aiData);
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
            timer = 0f;
            isAct = false;

            // ★ ランダムな様子見時間を決定
            currentThinkTime = Random.Range(thinkMin, thinkMax);
            thinkTimer = 0f;
            isThinking = true;
        }
    }

    private void ThinkTimer()
    {
        thinkTimer += Time.deltaTime;
        if (thinkTimer >= currentThinkTime)
        {
            thinkTimer = 0f;
            isThinking = false; // 行動可能
        }
    }
    #endregion
}
