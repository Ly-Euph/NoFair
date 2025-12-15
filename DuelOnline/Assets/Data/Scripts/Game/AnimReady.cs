using UnityEngine;

public class AnimReady : MonoBehaviour
{
    public void AnimEnd()
    {
        // オフラインとオンラインモードの判定
        if (DataSingleton_Offline.Instance != null)
            DataSingleton_Offline.Instance.IsReady = true;
        if (DataSingleton_Online.Instance != null)
            DataSingleton_Online.Instance.IsReady = true;
    }
}
