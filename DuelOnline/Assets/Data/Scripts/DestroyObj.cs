using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject,3.0f);
    }
}