using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;
    public ParticleSystem hoverParticles;
    public RectTransform underline;

    [Range(1.0f, 1.5f)]
    public float hoverScaleMultiplier; // 마우스 오버 시 크기 배율
    [Range(1.0f, 1.5f)]
    public float clickScaleMultiplier; // 클릭 시 최대 크기 배율
    [Range(0f, 0.5f)]
    public float transitionDuration; // 전환 애니메이션 시간
    [Range(0f, 0.3f)]
    public float clickDuration;// 클릭 애니메이션 시간

    [SerializeField] private float targetUnderlineWidth;
    [SerializeField] private float underlineAnimduration;
    [SerializeField] private float underlineOffsetY;

    private Sequence underlineSeq;
    private Vector3 originalScale;
    private Vector3 hoverScale;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        originalScale = buttonText.rectTransform.localScale;
        hoverScale = originalScale * hoverScaleMultiplier;

        if (hoverParticles != null)
        {
            var emission = hoverParticles.emission;
            emission.rateOverTime = 0f;
        }

        underline.sizeDelta = new Vector2(0, underline.sizeDelta.y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.rectTransform.DOScale(hoverScale, transitionDuration).SetEase(Ease.OutQuad);

        if (hoverParticles != null)
        {
            var emission = hoverParticles.emission;
            emission.rateOverTime = 5f;
        }


        if (underlineSeq != null && underlineSeq.IsActive())
            underlineSeq.Kill();
        underlineSeq = DOTween.Sequence();
        underline.sizeDelta = new Vector2(0, underline.sizeDelta.y);
        underline.SetParent(gameObject.transform);
        underline.localPosition = new Vector3(0, -15f, 0);
        underlineSeq.Append(underline.DOSizeDelta(new Vector2(targetUnderlineWidth, underline.sizeDelta.y), underlineAnimduration).From(new Vector2(0, underline.sizeDelta.y)));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.rectTransform.DOScale(originalScale, transitionDuration).SetEase(Ease.OutQuad);

        if (hoverParticles != null)
        {
            var emission = hoverParticles.emission;
            emission.rateOverTime = 0f;
        }
        if (underlineSeq != null)
            underline.DOSizeDelta(new Vector2(0, underline.sizeDelta.y), underlineAnimduration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Sequence clickSequence = DOTween.Sequence();

        // 1. 글자가 빠르게 커졌다가 hover 크기로 돌아옴
        clickSequence.Append(buttonText.rectTransform.DOScale(originalScale * clickScaleMultiplier, clickDuration));
        clickSequence.Append(buttonText.rectTransform.DOScale(hoverScale, clickDuration));

        if (hoverParticles != null)
        {
            hoverParticles.Emit(20);
        }
    }

    private void OnDestroy()
    {
        buttonText.rectTransform.DOKill();
        buttonText.DOKill();
    }
}
