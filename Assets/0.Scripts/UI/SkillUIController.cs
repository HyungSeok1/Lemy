using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  구버전 스킬 UI
/// </summary>
public class SkillUIController : MonoBehaviour
{
    [SerializeField] private Image iconImage0;
    [SerializeField] private Image cooldownIndicatorImage0;
    [SerializeField] private Image iconImage1;
    [SerializeField] private Image cooldownIndicatorImage1;

    private PlayerSkillController playerSkillController;

    void Start()
    {
        playerSkillController = Player.Instance.playerSkillController;

        playerSkillController.OnSkillChanged += SkillChangedEvent;
    }

    private void Update()
    {
        if (playerSkillController.currentSkills[0] != null)
            cooldownIndicatorImage0.fillAmount = playerSkillController.currentSkills[0].GetNormalizedRemainingCooldown();
        if (playerSkillController.currentSkills[1] != null)
            cooldownIndicatorImage1.fillAmount = playerSkillController.currentSkills[1].GetNormalizedRemainingCooldown();
    }

    // 스킬 로직 스크립트에있는 event에 등록하는 메서드입니다.
    private void SkillChangedEvent(int slot)
    {
        if (slot == 0)
        {
            // 스킬 장착시 vfx, 애니메이션 등 효과 들어가면 여기서 수정하면됨
            iconImage0.sprite = playerSkillController.currentSkills[0].skilldata.icon;
            iconImage0.enabled = true;
        }
        else if (slot == 1)
        {
            // 스킬 장착시 vfx, 애니메이션 등 효과 들어가면 여기서 수정하면됨
            iconImage1.sprite = playerSkillController.currentSkills[1].skilldata.icon;
            iconImage1.enabled = true;
        }
    }
}
