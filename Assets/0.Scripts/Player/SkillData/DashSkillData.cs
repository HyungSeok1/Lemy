using UnityEngine;

[CreateAssetMenu(fileName = "DashSkillData", menuName = "Game/Skill/DashSkillData")]
public class DashSkillData : SkillData
{
    public float dashPower;
    public float cooldown;
    public float turnDuration;
}
