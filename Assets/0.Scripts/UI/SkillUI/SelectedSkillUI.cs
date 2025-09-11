using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; // DoTween 네임스페이스 추가

/// <summary>
/// 새로 만든 스킬 UI
/// 선택한 스킬 슬롯 코드
/// </summary>
public class SelectedSkillUI : MonoBehaviour, IDropHandler,
                                IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image skillImage;
    [SerializeField] private int selectedSkillUIIndex; // 스킬 창 번호. 오른쪽 마우스가 0이고 Q W E 가 각각 1 2 3

    public ISkill selectedSkill; // 본인 창에 장착된 스킬데이터

    private Vector3 originalScale;

    void Awake()
    {
        if (skillImage == null)
        {
            skillImage = GetComponentInChildren<Image>();
            if (skillImage == null)
            {
                Debug.LogWarning($"{name} 오브젝트에서 SkillImage를 찾지 못했습니다.");
            }
        }

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        SkillSwapUI.OnSkillSwapUIOpened += InitializeSelectedSKill;
    }
    private void OnDisable()
    {
        SkillSwapUI.OnSkillSwapUIOpened -= InitializeSelectedSKill;
    }

    private void InitializeSelectedSKill()
    {
        // 장착된 스킬 데이터 선택
        selectedSkill = Player.Instance.playerSkillController.currentSkills[selectedSkillUIIndex];
        if (selectedSkill?.skilldata?.icon != null)
            skillImage.sprite = selectedSkill.skilldata.icon;
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillSlot draggedItem = eventData.pointerDrag.GetComponent<SkillSlot>();
        if (draggedItem != null && draggedItem.skillImage != null)
        {
            // 드래그한 아이템의 이미지를 자신의 슬롯에 적용
            skillImage.sprite = draggedItem.skillImage.sprite;
            skillImage.color = Color.white; // 원래 투명 등 처리된 경우 색상 초기화
        }

        // 스킬 변경부
        selectedSkill = draggedItem.skill;
        Player.Instance.playerSkillController.ChangeSkill(selectedSkill, selectedSkillUIIndex);

        // 드롭하면 크기 원래대로
        transform.DOScale(originalScale, 0.15f).SetEase(Ease.InOutBack);
    }


    // 마우스(혹은 드래그 프리뷰)가 슬롯 위로 올라왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 드래그 중일 때만 커지게 하고 싶다면 조건 추가
        if (eventData.pointerDrag != null)
        {
            transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
    }
}
