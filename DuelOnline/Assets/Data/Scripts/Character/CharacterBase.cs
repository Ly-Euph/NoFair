using UnityEngine;
using System.Collections.Generic;
public abstract class CharacterBase : MonoBehaviour
{
    // hp,mp共に最大最小は同じ
    protected const int MAX = 4;
    protected const int MIN = 0;

    protected int hp = 4; //体力
    protected int mp = 1; //マナ

    protected bool isDead = false;

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
        if (Input.GetMouseButtonDown(0))
        {
            points.Clear();
            isDrawing = true;
        }

        // 描画中
        if (isDrawing)
        {
            points.Add(Input.mousePosition);
        }

        // 終了時に解析
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            AnalyzeGesture(points);
        }
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
    protected void AnimSet(int AnimNum)
    {
        animator.Play(animName[AnimNum]);
        // Animnumに合わせて音源はセットしてある
        switch (AnimNum)
        {
            case 1: // 弱魔法詠唱
                // SE再生
                AudioManager.Instance.PlaySE(AnimNum);
                break;
            case 2: // 強魔法詠唱
                // SE再生
                AudioManager.Instance.PlaySE(AnimNum);
                break;
            case 3: // チャージ
                // SE再生
                AudioManager.Instance.PlaySE(AnimNum);
                break;
            case 5:
                // SE再生
                AudioManager.Instance.PlaySE(AnimNum);
                break;
        }
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
        // オブジェクトを有効化
        DefColBox.enabled = true;
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

        Vector2 start = pts[0];
        Vector2 end = pts[^1];
        float xDiff = end.x - start.x;
        float yDiff = end.y - start.y;

        float totalX = Mathf.Abs(pts[^1].x - pts[0].x);
        float totalY = Mathf.Abs(pts[^1].y - pts[0].y);

        // --- 1️⃣ 横線（左右スワイプ） ---
        if (totalX > 150 && totalY < 80)
        {
            Debug.Log("Command A: 横線ジェスチャー");
            OnHorizontal();
            return;
        }

        // --- 2️⃣ 縦線（上下スワイプ） ---
        if (totalY > 150 && totalX < 80)
        {
            Debug.Log("Command B: 縦線ジェスチャー");
            OnVertical();
            return;
        }

        // --- 3️⃣ V字（折れ線） ---
        if (IsVShape(pts))
        {
            Debug.Log("Command C: V字ジェスチャー");
            OnV();
            return;
        }

        // --- 4️⃣ 円（ループ） ---
        if (IsCircle(pts))
        {
            Debug.Log("Command D: 円ジェスチャー");
            OnCircle();
            return;
        }

        Debug.Log("ジェスチャー認識できず");
    }

    // ---- 判定関数 ----

    private bool IsVShape(List<Vector2> pts)
    {
        if (pts.Count < 10) return false;

        int mid = pts.Count / 2;
        Vector2 start = pts[0];
        Vector2 middle = pts[mid];
        Vector2 end = pts[^1];

        bool downThenUp = middle.y < start.y - 40 && end.y > middle.y + 40;
        float xMove = Mathf.Abs(end.x - start.x);

        return downThenUp && xMove < 150f; // 横ずれが少ないほど良い
    }
    private bool IsCircle(List<Vector2> pts)
    {
        // 始点と終点が近く、X・Yともにある程度動いている
        Vector2 start = pts[0];
        Vector2 end = pts[^1];
        float distance = Vector2.Distance(start, end);
        float width = Mathf.Abs(GetMaxX(pts) - GetMinX(pts));
        float height = Mathf.Abs(GetMaxY(pts) - GetMinY(pts));

        return distance < 50 && width > 100 && height > 100;
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

    private void OnCircle()
    {
        Debug.Log("⭕ 円アクション発動！");
        SAttack(); //弱魔法攻撃
    }
    #endregion
}