using UnityEngine;

[CreateAssetMenu(fileName = "NewCircleSlashSkillData", menuName = "Game/Skill/CircleSlashSkillData")]
public class CircleSlashSkillData : SkillData
{
    public float maxRange;
    public float minRange;
    public int damage;

    public float cooldown;

    public string animationTriggerName;
    public float animationTime; // animationTime 이후 데미지 들어감
}