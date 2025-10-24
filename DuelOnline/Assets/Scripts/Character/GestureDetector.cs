using System.Collections.Generic;
using UnityEngine;

public class GestureDetector : MonoBehaviour
{
    private List<Vector2> points = new List<Vector2>();
    private bool isDrawing = false;

    void Update()
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

        // --- 3️⃣ Z字（折れ線） ---
        if (IsZShape(pts))
        {
            Debug.Log("Command C: Z字ジェスチャー");
            OnZ();
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

    private bool IsZShape(List<Vector2> pts)
    {
        // Z字: X増加、Y減少 → X増加、Y減少を含む
        int mid = pts.Count / 2;
        Vector2 first = pts[0];
        Vector2 middle = pts[mid];
        Vector2 last = pts[^1];

        bool diag1 = (middle.x > first.x + 50 && middle.y < first.y - 30);
        bool diag2 = (last.x > middle.x + 50 && last.y < middle.y - 30);

        return diag1 && diag2;
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
        
    }

    private void OnVertical()
    {
        Debug.Log("▲ 縦アクション発動！");
    }

    private void OnZ()
    {
        Debug.Log("⚡ Zアクション発動！");
    }

    private void OnCircle()
    {
        Debug.Log("⭕ 円アクション発動！");
    }
}
