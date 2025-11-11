using UnityEngine;

public class AnimReady : MonoBehaviour
{
    public void AnimEnd()
    {
        DataSingleton_Offline.Instance.IsReady = true;
    }
}
