using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시간-> 공간, 시간-> 시간만 고려함. 
/// 공간-> 공간, 공간-> 시간의 경우, Kill 및 재생만 고려함
/// 
/// </summary>
public class AutoMessagePanel : MonoBehaviour
{

    public TMP_Text autoMessage;
    public Image autoImage;
    public CanvasGroup messageGroup;
    public CanvasGroup imageGroup;

    public float displayTime; // 기본 3f
    public float fadeDuration; // 기본 0.5f

    private bool isDisplaying = false;
    private CanvasGroup displayingGroup;
    Sequence seq;

    /// <summary>
    /// 메시지를 표시하는 메서드입니다.
    /// </summary>
    public void ShowMessageByTime(string message, Sprite sprite, OutputType type)
    {
        seq?.Kill();
        seq = DOTween.Sequence();
        displayingGroup?.DOKill();           // 이전 표시 그룹에 걸린 트윈 정리

        if (isDisplaying)
            seq.Append(displayingGroup.DOFade(0f, 0.25f));  // 먼저 기존 메시지 빠르게 사라짐

        isDisplaying = true;
        CanvasGroup targetGroup = displayingGroup = (type == OutputType.Message ? messageGroup : imageGroup);

        if (type == OutputType.Message)
            seq.AppendCallback(() => autoMessage.text = message);
        else if (type == OutputType.Image)
            seq.AppendCallback(() =>
            {
                autoImage.sprite = sprite;
                autoImage.SetNativeSize();
            });

        seq.Append(targetGroup.DOFade(1f, fadeDuration));
        seq.AppendInterval(displayTime);
        seq.Append(targetGroup.DOFade(0f, fadeDuration));
        seq.AppendCallback(() => isDisplaying = false);
    }

    /// <summary>
    /// 메시지를 표시하는 메서드입니다.
    /// </summary>
    public void ShowMessageByZone(string message, Sprite sprite, OutputType type)
    {
        seq?.Kill();
        seq = DOTween.Sequence();
        displayingGroup?.DOKill();           // 이전 표시 그룹에 걸린 트윈 정리

        if (isDisplaying)
            seq.Append(displayingGroup.DOFade(0f, 0.25f));  // 먼저 기존 메시지 빠르게 사라짐

        isDisplaying = true;
        CanvasGroup targetGroup = displayingGroup = (type == OutputType.Message ? messageGroup : imageGroup);

        if (type == OutputType.Message)
            seq.AppendCallback(() => autoMessage.text = message);
        else if (type == OutputType.Image)
            seq.AppendCallback(() => autoImage.sprite = sprite);

        seq.Append(targetGroup.DOFade(1f, fadeDuration));
    }

    /// <summary>
    /// 메시지를 숨기는 메서드입니다.
    /// </summary>
    public void HideMessageByZone()
    {
        seq?.Kill();
        seq = DOTween.Sequence();
        displayingGroup?.DOKill();           // 이전 표시 그룹에 걸린 트윈 정리

        seq.Append(displayingGroup.DOFade(0f, fadeDuration));
        isDisplaying = false;
    }

}
