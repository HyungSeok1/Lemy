using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonScaleEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;

    [Range(1.0f, 1.5f)]
    public float hoverScaleMultiplier = 1.1f; // 마우스 오버 시 크기 배율
    [Range(1.0f, 1.5f)]
    public float clickScaleMultiplier = 1.2f; // 클릭 시 최대 크기 배율
    [Range(0f, 0.5f)]
    public float transitionDuration = 0.2f; // hover 전환 시간
    [Range(0f, 0.3f)]
    public float clickDuration = 0.1f; // 클릭 애니메이션 시간


    [Header("Events")]
    [SerializeField] private UnityEvent onClick = new UnityEvent(); // Inspector에서 할당 가능


    private Vector3 originalScale;
    private Vector3 hoverScale;


    private void Awake()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        originalScale = rect.localScale;
        hoverScale = originalScale * hoverScaleMultiplier;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOScale(hoverScale, transitionDuration).SetEase(Ease.OutQuad).SetUpdate(true); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOScale(originalScale, transitionDuration).SetEase(Ease.OutQuad).SetUpdate(true); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(rect.DOScale(originalScale * clickScaleMultiplier, clickDuration).SetUpdate(true));
        clickSequence.Append(rect.DOScale(hoverScale, clickDuration).SetUpdate(true));


        // UnityEvent 호출 (버튼처럼)
        onClick?.Invoke();
    }

    private void OnDestroy()
    {
        rect.DOKill();
    }

    private void OnEnable()
    {
        rect.DOKill();
        rect.localScale = originalScale;
    }

    private void OnDisable()
    {
        rect.DOKill();
        rect.localScale = originalScale;
    }
}
