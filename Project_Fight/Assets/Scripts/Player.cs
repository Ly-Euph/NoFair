using UnityEngine;

/// <summary>
/// �v���C���[�̊�{����i�ړ��E�W�����v�j���Ǘ�����X�N���v�g
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 5f;         // �ʏ�ړ����x
    public float jumpForce = 5f;         // �W�����v��
    public float gravity = -9.81f;       // �d��

    [Header("�J�����̎Q��")]
    public Transform cameraTransform;    // �J������Transform�i�����ɍ��킹�Ĉړ��j

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // �����蔻�肾�����I�t�ɂ���iCharacterController�̗L�������j
        GetComponent<CharacterController>().enabled = false;
        // �J���������w��Ȃ�MainCamera���g�p
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
    /// �v���C���[�̈ړ��ƃW�����v����
    /// </summary>
    void HandleMovement()
    {
        // �n�ʂɐڂ��Ă��邩�𔻒�
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // �n�ʂɋz������������ȗ�
        }

        // ���͎擾
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // �J�����̌�������ɕ���������iY���̂ݍl���j
        Vector3 moveDir = cameraTransform.right * h + cameraTransform.forward * v;
        moveDir.y = 0f; // �����ړ��̂�
        moveDir.Normalize();

        // ���ۂ̈ړ�
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // �W�����v
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // �d�͂�������
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
