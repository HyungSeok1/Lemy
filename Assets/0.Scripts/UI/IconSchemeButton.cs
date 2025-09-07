using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 스킬 선택창을 띄우면 각각 나오는 버튼(눌려지진 않고, 커서를 올려두고 선택창을 닫으면 선택되는 것)에 쓰이는 스크립트입니다.
/// 
/// 생성될 당시 SkillSelector에서 참조를 가지고 있습니다. (생성 즉시 참조 SkillSelector에 저장됨)
/// 
/// </summary>
/// 

public class IconSchemeButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public IControlScheme schemeReference = null;
    public bool isHovered = false; //마우스 커서가 위에 있는지 확인하는 변수

    [SerializeField] float enlargeFactor = 1.2f;
    [SerializeField] float duration = 0.2f;
    RectTransform rt;
    Vector3 originalScale;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        originalScale = rt.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rt.DOScale(originalScale * enlargeFactor, duration);
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rt.DOScale(originalScale, duration);
        isHovered = false;
    }

    //지금은 바로 사라지는데, 나중엔 이런식으로 할수도?
    public void Disappear()
    {
        //서서히 Fade

        //그 후 Destroy
    }
}
