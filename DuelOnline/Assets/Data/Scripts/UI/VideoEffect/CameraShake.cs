using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float power, float time)
    {
        Vector3 basePos = transform.localPosition;
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            Vector2 offset2D = Random.insideUnitCircle * power;
            Vector3 offset3D = new Vector3(offset2D.x, offset2D.y, 0f);

            transform.localPosition = basePos + offset3D;
            yield return null;
        }

        transform.localPosition = basePos;
    }
}
