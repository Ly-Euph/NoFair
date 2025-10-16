using Fusion;
using UnityEngine;

public class PlayerAvatar : NetworkBehaviour
{
    private NetworkCharacterController characterController;

    public override void Spawned()
    {
        characterController = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        // �ړ�
        var inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        characterController.Move(inputDirection);
        // �W�����v
        if (Input.GetKey(KeyCode.Space))
        {
            characterController.Jump();
        }
    }
}