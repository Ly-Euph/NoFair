using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;  // 移動速度
    [SerializeField] private GameObject eff_BlockHit; // 防御された
    private Vector3 direction;

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    // ターゲットX座標から方向を設定
    public void SetDirectionByTargetX(float targetX)
    {
        if (transform.position.x < targetX)
        {
            // 右向き
            direction = Vector3.right;
            transform.rotation = Quaternion.identity; // 見た目を右向きに
        }
        else
        {
            // 左向き
            direction = Vector3.left;
            transform.rotation = Quaternion.Euler(0, 180, 0); // 見た目を左向きに反転
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // 自分自身や味方の弾を除外したいならここで判定を追加
        if (other.CompareTag("Player"))
        {
            // プレイヤーのスクリプトを取得
            CharacterBase player = other.GetComponent<CharacterBase>();
            if (player != null)
            {
                Debug.Log("HIT");
                player.Damage(true); // ← ヒット処理を呼ぶ
            }

            DestroyBullet();
        }
        else if (other.CompareTag("Defense")) // 基本的には相殺される
        {
            // SE再生
            AudioManager.Instance.PlaySE(8);
            GameObject obj = Instantiate(eff_BlockHit, transform.position, Quaternion.identity);
            DestroyBullet();
        }
        else
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
