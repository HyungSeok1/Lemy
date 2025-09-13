using UnityEngine;

[CreateAssetMenu(fileName = "NewHealSkillData", menuName = "Game/Skill/HealSkillData")]
public class HealSkillData : SkillData
{
    public int healAmount;

    public int cooldown;

    public ParticleSystem healParticle;

    // 애니메이션 관련 변수
    public string animationTriggerName;
    public float animationTime; // 애니메이션이 끝난 후 데미지가 적용됩니다.
}