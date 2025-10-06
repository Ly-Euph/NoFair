using UnityEngine;

public class RailAutoMove : MonoBehaviour
{
    public Transform[] waypoints; // ���[����̐���_
    public float speed = 5f;      // �ړ����x
    private float t = 0f;         // �Ȑ���̈ʒu�p�����[�^
    private int segment = 0;      // ���݂̋��

    void Update()
    {
        if (waypoints.Length < 2) return;

        // ���̋�Ԃ̃C���f�b�N�X�v�Z
        int p0 = Mathf.Max(segment - 1, 0);
        int p1 = segment;
        int p2 = (segment + 1) % waypoints.Length;
        int p3 = (segment + 2) % waypoints.Length;

        // Catmull-Rom�X�v���C�����
        Vector3 newPos = CatmullRom(waypoints[p0].position, waypoints[p1].position,
                                    waypoints[p2].position, waypoints[p3].position, t);
        transform.position = newPos;

        // ������i�s�����ɍ��킹��
        Vector3 nextPos = CatmullRom(waypoints[p0].position, waypoints[p1].position,
                                     waypoints[p2].position, waypoints[p3].position, t + 0.01f);
        transform.LookAt(nextPos);

        // t�𑬓x�ɉ����Đi�߂�
        t += speed * Time.deltaTime / Vector3.Distance(waypoints[p1].position, waypoints[p2].position);
        if (t >= 1f)
        {
            t = 0f;
            segment = (segment + 1) % waypoints.Length; // ���[�v
        }
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // Catmull-Rom�X�v���C������
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * ((2f * p1) +
                       (-p0 + p2) * t +
                       (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                       (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }
}
