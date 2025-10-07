using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Action currentAction;    // ���݃J�[�\��������Ă���{�^���̏���
    private string currentButton = ""; // �f�o�b�O�p

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // ���N���b�N���ꂽ�Ƃ��Ɏ��s
        if (Input.GetMouseButtonDown(0)&&currentAction!=null)
        {
            if (currentAction != null)
            {
                Debug.Log($"[UIManager] {currentButton} ��Action�����s���܂�");
                currentAction.Invoke();
            }
            else
            {
                Debug.Log("[UIManager] �L����UI������܂���i��O��ԁj");
            }
        }
    }

    /// <summary>
    /// ���݃J�[�\�����������Ă���UI�̃A�N�V������o�^
    /// </summary>
    public void RegisterCurrentAction(string buttonName, Action action)
    {
        currentButton = buttonName;
        currentAction = action;
    }

    /// <summary>
    /// �J�[�\�������ꂽ���O�i������ԁj�ɖ߂�
    /// </summary>
    public void ClearCurrentAction()
    {
        currentButton = "";
        currentAction = null;
    }
}
