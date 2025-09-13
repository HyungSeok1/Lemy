using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaleEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;

    [Range(1.0f, 1.5f)]
    public float hoverScaleMultiplier = 1.1f; // 마우스 오버 시 크기 배율
    [Range(1.0f, 1.5f)]
    public float clickScaleMultiplier = 1.2f; // 클릭 시 최대 크기 배율
    [Range(0f, 0.5f)]
    public float transitionDuration = 0.2f; // hover 전환 시간
    [Range(0f, 0.3f)]
    public float clickDuration = 0.1f; // 클릭 애니메이션 시간

    private Vector3 originalScale;
    private Vector3 hoverScale;

    private void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponent<TextMeshProUGUI>();

        originalScale = buttonText.rectTransform.localScale;
        hoverScale = originalScale * hoverScaleMultiplier;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.rectTransform.DOScale(hoverScale, transitionDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.rectTransform.DOScale(originalScale, transitionDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(buttonText.rectTransform.DOScale(originalScale * clickScaleMultiplier, clickDuration));
        clickSequence.Append(buttonText.rectTransform.DOScale(hoverScale, clickDuration));
    }

    private void OnDestroy()
    {
        buttonText.rectTransform.DOKill();
    }
}
