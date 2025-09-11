using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
                          IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas;
    private GameObject dragClone;

    public ISkill skill;
    public Image skillImage;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Tweener brownianTween;

    [Header("Brownian Motion Settings")]
    public float jitterRadius = 5f;    // 흔들릴 반경
    public float moveSpeed = 0.5f;      // 이동 속도 (px/s)
    private const float MIN_DURATION = 0.01f; // DOTween이 너무 빨리 끝나는 경우 최소 duration

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        originalScale = transform.localScale;
        originalPosition = ((RectTransform)transform).anchoredPosition;
    }

    private void OnEnable()
    {
        SkillSwapUI.OnSkillSwapUIClosed += ClearDragClone;
        SkillSwapUI.OnSkillSwapUIOpened += HandleSkillSwapUIOpened;
    }

    private void OnDisable()
    {
        SkillSwapUI.OnSkillSwapUIClosed -= ClearDragClone;
        SkillSwapUI.OnSkillSwapUIOpened -= HandleSkillSwapUIOpened;

        ClearDragClone();
        StopBrownianMotion();
    }

    // --- 드래그 로직 ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragClone = new GameObject("DragClone");
        dragClone.transform.SetParent(canvas.transform, false);
        dragClone.transform.SetAsLastSibling();

        Image cloneImage = dragClone.AddComponent<Image>();
        cloneImage.sprite = skillImage.sprite;
        cloneImage.raycastTarget = false;
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

    // --- 마우스 오버 효과 ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }

    // --- 브라운 운동 ---
    private void HandleSkillSwapUIOpened()
    {
        originalPosition = ((RectTransform)transform).anchoredPosition;
        StartBrownianMotion();
    }

    private void StartBrownianMotion()
    {
        StopBrownianMotion();
        MoveToRandomPosition();
    }

    private void MoveToRandomPosition()
    {
        RectTransform rt = (RectTransform)transform;
        Vector2 randomOffset = Random.insideUnitCircle * jitterRadius;
        Vector2 target = originalPosition + randomOffset;

        float distance = Vector2.Distance(rt.anchoredPosition, target);

        // moveSpeed <= 0이면 사실상 정지
        float duration = moveSpeed > 0f ? Mathf.Max(distance / moveSpeed, MIN_DURATION) : 999999f;

        brownianTween = rt.DOAnchorPos(target, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(MoveToRandomPosition);
    }

    private void StopBrownianMotion()
    {
        if (brownianTween != null && brownianTween.IsActive())
        {
            brownianTween.Kill();
            brownianTween = null;
        }
        ((RectTransform)transform).anchoredPosition = originalPosition;
    }
}
