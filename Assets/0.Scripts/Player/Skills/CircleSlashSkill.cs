using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// Skill의 예시로 만들어진 클래스입니다.
/// 
/// Skill 로직 전체(스킬 실행, UI 등)은 이 클래스와 잘 동작하므로 이 구조를 따라 스킬을 만들어주시면 될 것 같습니다.
/// 
/// *일례로, remainingCooldown은 UI 로직에서 이 변수를 참조하여 남은 쿨다운을 시각화합니다.
/// 
/// </summary>

//TODO: 주변 베기 구현
public class CircleSlashSkill : MonoBehaviour, ISkill
{
    public SkillData skilldata => data;
    public CircleSlashSkillData data;

    // 내부 상태
    public bool CanExecute => remainingCooldown <= 0&& stackSystem.GetCurrentStackValue() >= data.stackGaugeCost;
    public float remainingCooldown;

    private StackSystem stackSystem;

    private void Start()
    {
        stackSystem=Player.Instance.stackSystem;

    }
    private void Update()
    {
        if (remainingCooldown > 0)
            remainingCooldown -= Time.deltaTime;
    }

    public void InitializeSkill()
    {
        remainingCooldown = data.cooldown;
    }

    public void ReleaseSkill()
    { }

    public void ExecuteSkill()
    {
        //애니메이션 실행 
        if (!CanExecute) return;

        remainingCooldown = data.cooldown;
        StartCoroutine(PerformSlash());
    }

    private IEnumerator PerformSlash()
    {
        // 스택 게이지 소모
        stackSystem.RemoveStackGauge(data.stackGaugeCost);

        // TODO: 애니메이션 추가 후 주석 해제 및 "animationTriggerName / animationTime" 을 SO에서 설정해줘야함
        //   GetComponentInParent<Player>().animator.SetTrigger(data.animationTriggerName);
        yield return new WaitForSeconds(data.animationTime);


        // (2) 적 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.maxRange);
        foreach (var col in hits)
        {
            if (!col.TryGetComponent<IDamageable>(out var damageable)) continue;

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < data.minRange) continue;

            // (3) 데미지 적용
            damageable.TakeDamage(data.damage + Player.Instance.stats.bonusATK);
        }
        print("CircleSlash 사용됨");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.maxRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.minRange);
    }

    // "오직 UI 코드에서 받아와서 쿨타임 시각화"하는 용도로만 쓰임
    public float GetNormalizedRemainingCooldown()
    {
        return remainingCooldown / data.cooldown;
    }
}
