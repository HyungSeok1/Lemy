using UnityEngine;

[CreateAssetMenu(fileName = "NewGhostSkillData", menuName = "Game/Skill/GhostSkillData")]
public class GhostSkillData : SkillData
{
    public float coolDown;
    public float speedupCoef;
    public float damageDecreaseRate;

    /// <summary>
    /// 초당 사용량 (기획서상 2)
    /// </summary>
    public int stackGaugeCostPerSecond;
    [Tooltip("데미지 입을 시 줄어드는 스택의 양")]
    public int stackGaugeDecreateAmount; 
}