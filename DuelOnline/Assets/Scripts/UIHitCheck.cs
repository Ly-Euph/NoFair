using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public class UIHitCheck : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    #region Field
    [Header("�J�[�\�����G�ꂽ���ɑI�𒆂Ƃ��ĕ\������"),SerializeField]
    GameObject obj_SelectPnl;
    // �o�^����{�^�����iInspector�Ŏw�� or ������gameObject���j
    [SerializeField] private string buttonName = "";
    private Action clickAction;
    #endregion

    #region Interface
    // �J�[�\����UI�ɓ������Ƃ��Ă΂��
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} �ɃJ�[�\�������܂���");

        // �\��
        obj_SelectPnl.SetActive(true);

        // UIManager�Ɋ֐��o�^
        UIManager.Instance.RegisterCurrentAction(buttonName, clickAction);
    }

    // �J�[�\����UI����o���Ƃ��Ă΂��
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} ����J�[�\��������܂���");

        // ��\��
        obj_SelectPnl.SetActive(false);

        // ��O�ɂ���
        UIManager.Instance.ClearCurrentAction();
    }
    #endregion

    void Awake()
    {
        // �ŏ��͔�\��
        obj_SelectPnl.SetActive(false);

        // ���O���ݒ�Ȃ�GameObject�����g��
        if (string.IsNullOrEmpty(buttonName))
            buttonName = gameObject.name;

        // �q�I�u�W�F�N�g����uOnClick�v�Ƃ������\�b�h�����X�N���v�g��T��
        MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>(true);

        foreach (var script in scripts)
        {
            MethodInfo method = script.GetType().GetMethod("OnClick", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                // ���I�ɌĂяo��Action���쐬
                clickAction = (Action)Delegate.CreateDelegate(typeof(Action), script, method);
                break;
            }
        }
    }

   
}
