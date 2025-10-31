using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The RectTransform with this script attached is zoomed in/out based on mouse hover state.
/// Scale and position adjustments are managed by the Animator component.
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(Animator))] // Animator��K�{�R���|�[�l���g�ɒǉ�
public class RectTransformZoomer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Animator�𐧌䂷��V���v���ȃX�N���v�g�ɑS�ʓI�ɏC��
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>(); // Animator���擾
    }

    // �}�E�X���{�^���̏�ɐi�������Ƃ��ɌĂ΂��
    public void OnPointerEnter(PointerEventData eventData)
    {
        // IsHovering�p�����[�^��ON�ɂ���
        animator.SetBool("IsHovering", true);
    }

    // �}�E�X���{�^������ޏo�����Ƃ��ɌĂ΂��
    public void OnPointerExit(PointerEventData eventData)
    {
        // IsHovering�p�����[�^��OFF�ɂ���
        animator.SetBool("IsHovering", false);
    }

    // ���̑S�Ẵt�B�[���h�ƃ��\�b�h�iZoomSpeed, Update, ScrollToZoomMap, OnDrag�Ȃǁj�͍폜
}