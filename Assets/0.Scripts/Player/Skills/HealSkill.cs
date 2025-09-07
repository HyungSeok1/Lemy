using System.Collections;
using UnityEngine;

/// <summary>
/// HealSkill class 스택게이지를 소모하여 플레이어의 체력 회복 스킬
/// </summary>

public class HealSkill : MonoBehaviour, ISkill
{
    public HealSkillData data;
    public SkillData skilldata => data;

    private StackSystem stackSystem;

    public bool CanExecute => stackSystem.GetCurrentStackValue() >= data.stackGaugeCost;

    public void ExecuteSkill()
    {
        if (!CanExecute) return;

        StartCoroutine(PerformHeal());
    }

    private IEnumerator PerformHeal()
    {
        yield return new WaitForSeconds(data.animationTime);

        // 스택 게이지 소모
        stackSystem.RemoveStackGauge(data.stackGaugeCost);
        // 플레이어 체력 회복
        Player.Instance.health.Heal(data.healAmount);
    }

    public void InitializeSkill()
    {
        stackSystem = Player.Instance.stackSystem;
        if (stackSystem == null)
        {
            Debug.LogError("StackSystem is not found. HealSkill cannot be initialized.");
            return;
        }
    }

    public float GetNormalizedRemainingCooldown()
    {
        return CanExecute ? 0f : 1f; // 스킬이 사용 가능하면 0, 아니면 1
    }

    public void ReleaseSkill()
    {
    }
}
