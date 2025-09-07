using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 
/// SlashSkill 구현 클래스입니다. 
/// 
/// 스킬을 사용하면 스킬 이펙트 프리팹을 생성하고, 
/// data 내부의 animationTime이 지나면 실제 공격이 이루어짐 
/// 그 전에 움직이거나, 멈추면 공격 캔슬 취급으로, 프리팹이 삭제되고 애니메이션이 리셋됨. 
/// 
/// 움직이면서 공격은 아직 애니메이션 구현이 안되어있음. 스프라이트 생기면 추가해야함 
/// 
/// + 바라보는 방향의 반대 조준하고 스킬쓰면 모션 이상할 수 있음. 수정 필요.... 
/// 
/// </summary> 
public class SlashSkill : MonoBehaviour, ISkill
{
    public SkillData skilldata => data; // 스크립터블 오브젝트에서 데이터 받아오기 
    [SerializeField] public SlashSkillData data;

    // 내부 변수 
    List<Collider2D> hits; // 피격 리스트 
    GameObject skill; // 스킬 이펙트 프리팹 
    Collider2D skillCol; // 이펙트의 콜라이더 
    Coroutine skillCoroutine;
    bool movingChecker; // 플레이어의 상태 전환 체크 용도 
    bool alreadySlashed; // 이미 피격한 이후인지 체크 (버그 있을수도....) 
    Animator animator;
    private StackSystem stackSystem;

    public bool CanExecute => remainingCooldown <= 0 && stackSystem.GetCurrentStackValue() >= data.stackGaugeCost;
    public float remainingCooldown;

    private void Start()
    {
        movingChecker = Player.Instance.IsMoving;
        animator = GetComponentInParent<Player>().animator;
        alreadySlashed = false;
        stackSystem = Player.Instance.stackSystem;
    }
    private void Update()
    {
        if (remainingCooldown > 0)
            remainingCooldown -= Time.deltaTime;

        // =======================================================================
        // [수정됨] 스킬(skill) 오브젝트가 존재하면 플레이어의 위치를 따라가도록 설정합니다.
        if (skill != null)
        {
            skill.transform.position = transform.position;
        }
        // =======================================================================

        if (movingChecker != Player.Instance.IsMoving && animator != null) // 상태 전환 체크  
        {
            movingChecker = Player.Instance.IsMoving;
            animator.SetBool("isSlashSkillUpToDown", false);
            animator.SetBool("isSlashSkillDownToUp", false);
            animator.SetBool("isSlashingFlying", false);


            if (!alreadySlashed) // 공격 판정 이후면 그냥 냅두고, 그 전이면 스킬 이펙트 삭제 
            {
                if (skillCoroutine != null) StopCoroutine(skillCoroutine);
                Destroy(skill);
            }
        }
    }
    public void InitializeSkill()
    {
        remainingCooldown = data.cooldown;
    }

    public void ExecuteSkill()
    {
        if (!CanExecute) return;

        remainingCooldown = data.cooldown;
        skillCoroutine = StartCoroutine(PerformSlash());
    }

    private IEnumerator PerformSlash()
    {
        // 스택 게이지 소모
        stackSystem.RemoveStackGauge(data.stackGaugeCost);

        alreadySlashed = false;
        animator = GetComponentInParent<Player>().animator;

        // 공격방향 감지 
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Player.Instance.currentMousePosition);
        Vector2 dir = mouseWorld - (Vector2)transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        SoundManager.Instance.PlaySFX("frontSlash2", 0.05f);

        int randomIndex = Random.Range(0, 2); // 공격 모션 2개중 랜덤선택 

        animator.SetBool("isSlashingFlying", true); // 비행 중 모션은 하나뿐이니 그냥 켜도 ok 

        if (randomIndex == 0)
        {
            // Slash From Up To Down 

            animator.SetBool("isSlashSkillUpToDown", true);

            // =========================================================================================
            // [수정됨] 마지막 인자인 parent(transform)를 제거해서 회전이 따라가지 않도록 합니다.
            skill = Instantiate(data.slashPrefabUpToDown, transform.position, rotation); // 이펙트 생성 
            // =========================================================================================
        }
        else
        {
            //Slash From Down To Up  
            animator.SetBool("isSlashSkillDownToUp", true);

            // =======================================================================================
            // [수정됨] 마지막 인자인 parent(transform)를 제거해서 회전이 따라가지 않도록 합니다.
            skill = Instantiate(data.slashPrefabDownToUp, transform.position, rotation); // 이펙트 생성 
            // =======================================================================================
        }
        skillCol = skill.GetComponent<Collider2D>(); // 콜라이더 받아오기 
        Destroy(skill, data.effectDuration); // 일정시간 후 삭제 


        // 실제 피격 순간까지 살짝 기다리기 
        // 현재 약 0.8초면 딱 맞음 
        yield return new WaitForSeconds(data.animationTime); //0.1초쯤 피격판정 들어감 
        SlashDamaging();

        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isSlashSkillUpToDown", false);
        animator.SetBool("isSlashSkillDownToUp", false);
        animator.SetBool("isSlashingFlying", false);


    }
    public float GetNormalizedRemainingCooldown()
    {
        return remainingCooldown / data.cooldown;
    }

    public void SlashDamaging() // 스킬 콜라이더 키고, 공격 판정, 그 후 콜라이더 끄기... 이것을 딱 피격 판정 순간에 실행시켜야 함. 
    {
        alreadySlashed = true;
        skillCol.enabled = true;

        hits = new List<Collider2D>();
        hits.Clear();

        int count = skillCol.Overlap(hits);
        foreach (var col in hits)
        {
            Debug.Log($"충돌된 오브젝트: {col.name}"); // 테스트용 

            if (!col.TryGetComponent<IDamageable>(out var damageable)) continue;

            damageable.TakeDamage(data.damage + Player.Instance.stats.bonusATK);
        }

        skillCol.enabled = false;
    }

    public void ReleaseSkill()
    {
    }
}