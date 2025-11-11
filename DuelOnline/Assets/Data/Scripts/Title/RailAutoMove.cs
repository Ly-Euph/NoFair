using UnityEngine;

public class RailAutoMove : MonoBehaviour
{
    public Transform[] waypoints; // レール上の制御点
    public float speed = 5f;      // 移動速度
    private float t = 0f;         // 曲線上の位置パラメータ
    private int segment = 0;      // 現在の区間

    void Update()
    {
        if (waypoints.Length < 2) return;

        // 次の区間のインデックス計算
        int p0 = Mathf.Max(segment - 1, 0);
        int p1 = segment;
        int p2 = (segment + 1) % waypoints.Length;
        int p3 = (segment + 2) % waypoints.Length;

        // Catmull-Romスプライン補間
        Vector3 newPos = CatmullRom(waypoints[p0].position, waypoints[p1].position,
                                    waypoints[p2].position, waypoints[p3].position, t);
        transform.position = newPos;

        // 向きを進行方向に合わせる
        Vector3 nextPos = CatmullRom(waypoints[p0].position, waypoints[p1].position,
                                     waypoints[p2].position, waypoints[p3].position, t + 0.01f);
        transform.LookAt(nextPos);

        // tを速度に応じて進める
        t += speed * Time.deltaTime / Vector3.Distance(waypoints[p1].position, waypoints[p2].position);
        if (t >= 1f)
        {
            t = 0f;
            segment = (segment + 1) % waypoints.Length; // ループ
        }
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Catmull-Romスプライン公式
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * ((2f * p1) +
                       (-p0 + p2) * t +
                       (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                       (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }
}
