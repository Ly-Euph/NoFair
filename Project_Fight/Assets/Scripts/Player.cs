using UnityEngine;

/// <summary>
/// プレイヤーの基本操作（移動・ジャンプ）を管理するスクリプト
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;         // 通常移動速度
    public float jumpForce = 5f;         // ジャンプ力
    public float gravity = -9.81f;       // 重力

    [Header("カメラの参照")]
    public Transform cameraTransform;    // カメラのTransform（方向に合わせて移動）

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // 当たり判定だけをオフにする（CharacterControllerの有効無効）
        GetComponent<CharacterController>().enabled = false;
        // カメラが未指定ならMainCameraを使用
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// プレイヤーの移動とジャンプ処理
    /// </summary>
    void HandleMovement()
    {
        // 地面に接しているかを判定
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に吸着させる微小な力
        }

        // 入力取得
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // カメラの向きを基準に方向を決定（Y軸のみ考慮）
        Vector3 moveDir = cameraTransform.right * h + cameraTransform.forward * v;
        moveDir.y = 0f; // 水平移動のみ
        moveDir.Normalize();

        // 実際の移動
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // ジャンプ
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // 重力を加える
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
