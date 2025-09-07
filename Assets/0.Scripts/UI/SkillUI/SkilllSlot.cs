using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; // DoTween 네임스페이스 추가

/// <summary>
/// 새로 만든 스킬 UI
/// 스킬 슬롯 코드
/// </summary>
public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
                          IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas;
    private GameObject dragClone; // 드래그용 복사본

    public ISkill skill;
    public Image skillImage;

    private Vector3 originalScale; // 원래 스케일 저장

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        SkillSwapUI.OnSkillSwapUIClosed += ClearDragClone;
    }

    private void OnDisable()
    {
        SkillSwapUI.OnSkillSwapUIClosed -= ClearDragClone;
        ClearDragClone(); // 혹시 남아있으면 정리
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 복사본 생성
        dragClone = new GameObject("DragClone");
        dragClone.transform.SetParent(canvas.transform, false);
        dragClone.transform.SetAsLastSibling();

        Image cloneImage = dragClone.AddComponent<Image>();
        cloneImage.sprite = skillImage.sprite;
        cloneImage.raycastTarget = false; // 복사본은 Raycast 받지 않음
        dragClone.transform.position = skillImage.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone != null)
            dragClone.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ClearDragClone();
    }

    private void ClearDragClone()
    {
        if (dragClone != null)
        {
            Destroy(dragClone);
            dragClone = null;
        }
    }

    // 마우스가 슬롯에 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    // 마우스가 슬롯에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
}
