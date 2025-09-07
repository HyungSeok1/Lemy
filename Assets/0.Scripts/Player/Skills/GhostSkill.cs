using UnityEngine;

public class GhostSkill : MonoBehaviour, ISkill
{
    public GhostSkillData data;
    public SkillData skilldata => data;
    public bool CanExecute => stackSystem.GetCurrentStackValue() >= data.stackGaugeCost;

    private StackSystem stackSystem;
    private bool isSkillEnabled;
    private float savedDEF;

    private void Update()
    {
        if (isSkillEnabled)
            stackSystem.RemoveStackGauge(data.stackGaugeCostPerSecond * Time.deltaTime);

        // 켜져있을때, 스택 다 소진했으면 스킬 OFF
        if (isSkillEnabled && stackSystem.GetCurrentStackValue() < data.stackGaugeCostPerSecond)
            ExecuteSkill();
    }

    public void ExecuteSkill()
    {
        if (!CanExecute) return;

        // toggle skill
        if (!isSkillEnabled) // 꺼진상태
        {
            // 스택 사용
            stackSystem.RemoveStackGauge(25);

            //스킬 on
            isSkillEnabled = true;

            // 벽 통과 on
            Physics2D.IgnoreLayerCollision(11, 12, true);

            // 방어력 감소
            savedDEF = Player.Instance.stats.percentDEF * data.damageDecreaseRate;
            Player.Instance.stats.percentDEF -= savedDEF;

            // ghostSkillSpeed를 maxspeed의 speedupCoef배로 즉시 바꾸기
            Player.Instance.movement.ghostSkillSpeed = Player.Instance.movement.GetMaxSpeed() * data.speedupCoef;

            // 데미지 입었을때 스택 25 감소
            Player.Instance.health.OnDamaged += ReduceStack;


        }
        else // 켜진상태
        {
            //스킬 off
            isSkillEnabled = false;

            // 벽 통과 off
            Physics2D.IgnoreLayerCollision(11, 12, false);

            // 방어력 다시 증가
            Player.Instance.stats.percentDEF += savedDEF;

            // ghostSkillSpeed를  0
            Player.Instance.movement.ghostSkillSpeed = 0f;

            // 복구
            Player.Instance.health.OnDamaged -= ReduceStack;

        }

    }

    private void ReduceStack(float _)
    {
        stackSystem.RemoveStackGauge(data.stackGaugeDecreateAmount);
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
