using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The RectTransform with this script attached is zoomed in/out based on mouse hover state.
/// Scale and position adjustments are managed by the Animator component.
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(Animator))] // Animatorを必須コンポーネントに追加
public class RectTransformZoomer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Animatorを制御するシンプルなスクリプトに全面的に修正
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>(); // Animatorを取得
    }

    // マウスがボタンの上に進入したときに呼ばれる
    public void OnPointerEnter(PointerEventData eventData)
    {
        // IsHoveringパラメータをONにする
        animator.SetBool("IsHovering", true);
    }

    // マウスがボタンから退出したときに呼ばれる
    public void OnPointerExit(PointerEventData eventData)
    {
        // IsHoveringパラメータをOFFにする
        animator.SetBool("IsHovering", false);
    }

    // 他の全てのフィールドとメソッド（ZoomSpeed, Update, ScrollToZoomMap, OnDragなど）は削除
}