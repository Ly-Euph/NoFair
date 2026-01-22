using UnityEngine;
using System.Collections.Generic;
using Fusion;
public abstract class CharacterBase : NetworkBehaviour
{
    // パラメータ
    // hp,mp共に最大最小は同じ
    protected const int MAX = 4;
    protected const int MIN = 0;
    protected int hp = 4; //体力
    protected int mp = 1; //マナ
    protected bool isDead = false;

    protected const int ofsX_Cast = 3;  // 魔法陣エフェクトのオフセット値
    protected const int ofsX_Block = 7; // 防御エフェクトのオフセット値
    protected const float ofsZ = -4.8f;
    private Vector3 pos = new Vector3(-3, 5, 0); // Xは±を変更しますプレイヤー座標によって。

    /*エフェクト*/
    [SerializeField] GameObject eff_cursor;
    [SerializeField] GameObject eff_magicCircleS; // 魔法陣弱魔法
    [SerializeField] GameObject eff_magicCircleL; // 魔法陣強魔法
    [SerializeField] GameObject eff_charge;       // チャージエフェクト
    [SerializeField] GameObject eff_block;        // 防御エフェクト
    [SerializeField] GameObject eff_damage;       // ヒット時エフェクト

    private GameObject effBox;
    private GameObject effBox_sub;

    // プレイヤーカメラ
    [SerializeField]protected Camera plCam;
    // プレイヤーの防御あたり判定
    [SerializeField]protected BoxCollider DefColBox;
    // 魔法生成場所
    [SerializeField]protected GameObject BulletPos;
    // 魔法実態(L:強魔法,S:弱魔法)
    [SerializeField]protected GameObject LBulletPre;
    [SerializeField]protected GameObject SBulletPre;
    // アニメーションコンポーネント
    [SerializeField] Animator animator;
    // アニメーション
    protected string[] animName =
    {
       "Chara_IDLE","Chara_CAST_S","Chara_CAST_L",
       "Chara_CHARGE","Chara_BLOCK","Chara_DAMAGE",
       "Chara_DIE"
    };
    protected int animNum=0; // アニメーション取り扱い番号

    // 入力制御
    private List<Vector2> points = new List<Vector2>();
    private bool isDrawing = false;

    // 入力制御
    protected void InputController()
    {
        // 描画開始
        if (Input.GetMouseButtonDown(0)&&!isDrawing)
        {
            // エフェクト生成
            Vector3 startPos = GetMouseWorldPos();
            effBox = Instantiate(eff_cursor, startPos, plCam.transform.rotation);
            points.Clear();
            isDrawing = true;
        }

        // 描画中
        if (isDrawing)
        {
            effBox.transform.position = GetMouseWorldPos();
            points.Add(Input.mousePosition);
        }

        // 終了時に解析
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            Destroy(effBox);
            AnalyzeGesture(points);
            points.Clear();
            if (GameLauncher.Instance == null) { return; }
            GameLauncher.Instance.SetInputNum(animNum);
        }
    }

    // Camera基準でマウス座標をワールド座標に変換
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        // z距離をカメラからの適切な値に設定（エフェクトの見え方で調整）
        mousePos.z = 5f;
        return plCam.ScreenToWorldPoint(mousePos);
    }
    // 弱魔法
    public abstract void SAttack();
    // 強魔法
    public abstract void LAttack();
    // チャージ
    public abstract void Charge();
    // 防御
    public abstract void Block();
    // ダメージヒット
    public abstract void Damage(bool isSmall);

    // アニメーションイベント制御
    #region
    // アニメーション発動用
    protected void AnimSet(int animSetNum)
    {
        Quaternion rot = Quaternion.identity; // 初期化
        rot *= Quaternion.Euler(0f, 0f, 90f); // Zだけ90度回転
        // Animnumに合わせて音源はセットしてある
        switch (animSetNum)
        {
            case 1: // 弱魔法詠唱
                // プレイヤーＸ座標がマイナスならプレイヤー１
                pos.x = this.gameObject.transform.position.x <= 0 ? +ofsX_Cast : -ofsX_Cast; // プレイヤー判断
                // SE再生
                AudioManager.Instance.PlaySE(animSetNum);
                // 魔法陣エフェクト生成
                effBox_sub=
                    Instantiate(eff_magicCircleS, 
                    transform.position + new Vector3(pos.x, pos.y, pos.z),rot);
                Destroy(effBox_sub, 1.2f); // エフェクト削除タイミング
                break;
            case 2: // 強魔法詠唱
                // プレイヤーＸ座標がマイナスならプレイヤー１
                pos.x = this.gameObject.transform.position.x <= 0 ? +ofsX_Cast : -ofsX_Cast; // プレイヤー判断
                // SE再生
                AudioManager.Instance.PlaySE(animSetNum);
                // 魔法陣エフェクト生成
                effBox_sub =
                    Instantiate(eff_magicCircleL,
                    transform.position + new Vector3(pos.x, pos.y, pos.z), rot);
                Destroy(effBox_sub, 1.2f); // エフェクト削除タイミング
                break;
            case 3: // チャージ
                // SE再生
                AudioManager.Instance.PlaySE(animSetNum);
                effBox_sub = Instantiate(eff_charge, transform.position,Quaternion.identity);
                Destroy(effBox_sub, 2f);
                break;
            case 5: // ダメージ
                // Yオフセット
                effBox_sub = Instantiate(eff_damage, transform.position+new Vector3(0,1.5f,0), Quaternion.identity);
                Destroy(effBox_sub, 0.9f); // 仮の数値
                // SE再生
                AudioManager.Instance.PlaySE(animSetNum);

                // 直前のエフェクトを削除
                if (effBox_sub != null)
                {
                    Destroy(effBox_sub);
                }
                // 描画トレイルエフェクトも
                if (effBox != null)
                {
                    Destroy(effBox);
                }
                isDrawing = false;
                break;
        }
        animator.Play(animName[animSetNum]);
        Debug.Log("アニメーション変更");
    }
    // 弱魔法
    void GenerateSMagicEvent()
    {
        GameObject obj= Instantiate(SBulletPre, BulletPos.transform.position, Quaternion.identity);
        obj.transform.SetParent(BulletPos.transform);
        //自分プレイヤーオブジェクトと反対の位置を指定
        obj.GetComponent<Bullet>().SetDirectionByTargetX(-transform.position.x);
        // SE再生
        AudioManager.Instance.PlaySE(7);
    }
    // 強魔法
    void GenerateLMagicEvent()
    {
        GameObject obj = Instantiate(LBulletPre, BulletPos.transform.position, Quaternion.identity);
        obj.transform.SetParent(BulletPos.transform);
        //自分プレイヤーオブジェクトと反対の位置を指定
        obj.GetComponent<LBullet>().SetDirectionByTargetX(-transform.position.x);
        // SE再生
        AudioManager.Instance.PlaySE(7);
    }
    void DeadEvent()
    {
        animator.enabled = false;
    }
    // 防御アニメーションイベント設定用
  　void BlockAnimEventSet()
    {
        Quaternion rot = Quaternion.identity; // 初期化
        pos.x = this.gameObject.transform.position.x <= 0 ? +ofsX_Block : -ofsX_Block; // プレイヤー判断
        rot *= Quaternion.Euler(0f, 0f, 90f); // Zだけ90度回転
        // オブジェクトを有効化
        DefColBox.enabled = true;
        //エフェクト
        effBox_sub = Instantiate(
            eff_block,
            transform.position + new Vector3(pos.x, 8.5f, ofsZ),
            rot);
        Destroy(effBox_sub,1.6f);
        // SE再生
        AudioManager.Instance.PlaySE(4);
    }
    void DeleteBlockEvent()
    {
        DefColBox.enabled = false;
    }
    #endregion

    // 入力制御の関数
    #region 
    private void AnalyzeGesture(List<Vector2> pts)
    {
        if (pts.Count < 5) return;

        // --- 画面サイズ基準 ---
        float minScreen = Mathf.Min(Screen.width, Screen.height);

        Vector2 start = pts[0];
        Vector2 end = pts[^1];

        float totalX = Mathf.Abs(end.x - start.x) / minScreen;
        float totalY = Mathf.Abs(end.y - start.y) / minScreen;

        // --- 3️⃣ V字（折れ線） ---
        if (IsVShape(pts, minScreen))
        {
            Debug.Log("Command C: V字ジェスチャー");
            OnV();
            return;
        }

        // --- 4️⃣ 逆V（折れ線） ---
        if (IsRVShape(pts, minScreen))
        {
            Debug.Log("Command D: 逆Vジェスチャー");
            OnRV();
            return;
        }

        // --- 1️⃣ 横線（左右スワイプ） ---
        if (totalX > 0.15f && totalY < 0.08f)
        {
            Debug.Log("Command A: 横線ジェスチャー");
            OnHorizontal();
            return;
        }

        // --- 2️⃣ 縦線（上下スワイプ） ---
        if (totalY > 0.15f && totalX < 0.08f)
        {
            Debug.Log("Command B: 縦線ジェスチャー");
            OnVertical();
            return;
        }

       

        Debug.Log("ジェスチャー認識できず");
    }

    // ---- 判定関数 ----

    private bool IsVShape(List<Vector2> pts, float minScreen)
    {
        if (pts.Count < 10) return false;

        Vector2 start = pts[0];
        Vector2 middle = pts[pts.Count / 2];
        Vector2 end = pts[^1];

        // ジェスチャー自身のサイズ
        float width = GetMaxX(pts) - GetMinX(pts);
        float height = GetMaxY(pts) - GetMinY(pts);
        float size = Mathf.Max(width, height);

        // 小さすぎる入力は無視（誤タップ防止）
        if (size < Screen.height * 0.05f)
            return false;

        // 谷の深さ（∨）
        float valley = Mathf.Min(start.y, end.y) - middle.y;

        // 谷が十分に深いか（比率で判断）
        if (valley / size < 0.25f)
            return false;

        return true;
    }
   private bool IsRVShape(List<Vector2> pts, float minScreen)
    {
        if (pts.Count < 10) return false;

        Vector2 start = pts[0];
        Vector2 middle = pts[pts.Count / 2];
        Vector2 end = pts[^1];

        float width = GetMaxX(pts) - GetMinX(pts);
        float height = GetMaxY(pts) - GetMinY(pts);
        float size = Mathf.Max(width, height);

        if (size < Screen.height * 0.05f)
            return false;

        float peak = middle.y - Mathf.Max(start.y, end.y);

        if (peak / size < 0.25f)
            return false;

        return true;
    }

    private float GetMaxX(List<Vector2> pts) => Mathf.Max(pts.ConvertAll(p => p.x).ToArray());
    private float GetMinX(List<Vector2> pts) => Mathf.Min(pts.ConvertAll(p => p.x).ToArray());
    private float GetMaxY(List<Vector2> pts) => Mathf.Max(pts.ConvertAll(p => p.y).ToArray());
    private float GetMinY(List<Vector2> pts) => Mathf.Min(pts.ConvertAll(p => p.y).ToArray());

    // ---- 各ジェスチャーのアクション ----

    private void OnHorizontal()
    {
        Debug.Log("▶ 横アクション発動！");
        Charge(); //マナチャージ
    }

    private void OnVertical()
    {
        Debug.Log("▲ 縦アクション発動！");
        Block(); //防御
    }

    private void OnV()
    {
        Debug.Log("⚡ Vアクション発動！");
        LAttack(); //強魔法攻撃
    }

    private void OnRV()
    {
        Debug.Log("⭕ 逆Vアクション発動！");
        SAttack(); //弱魔法攻撃
    }
    #endregion
}