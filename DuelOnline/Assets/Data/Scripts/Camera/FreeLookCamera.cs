using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float fastSpeed = 3f; // Shift加速

    [Header("視点")]
    [SerializeField] private float mouseSensitivity = 2.5f;
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float maxPitch = 89f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Vector3 rot = transform.eulerAngles;
        yaw = rot.y;
        pitch = rot.x;
    }

    void Update()
    {
        HandleMove();
        HandleLook();
    }

    // -------------------------
    // 移動処理
    // -------------------------
    void HandleMove()
    {
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastSpeed;

        Vector3 move =
            transform.forward * Input.GetAxis("Vertical") +
            transform.right * Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.E)) move += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) move += Vector3.down;

        transform.position += move * speed * Time.deltaTime;
    }

    // -------------------------
    // 視点操作
    // -------------------------
    void HandleLook()
    {
        if (!Input.GetMouseButton(1)) return; // 右クリック中のみ

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 5f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 5f * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
