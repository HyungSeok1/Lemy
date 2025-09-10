using DG.Tweening;
using UnityEngine;

public class UICanvasManager : PersistentSingleton<UICanvasManager>
{
    [SerializeField] private CanvasGroup uiCanvasGroup;
    [SerializeField] private float fadeDuration;

    protected override void Awake()
    {
        base.Awake();
    }

    public void FadeOutUI()
    {
        uiCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
    }

    public void FadeInUI()
    {
        uiCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.Linear).SetUpdate(true);
    }
}