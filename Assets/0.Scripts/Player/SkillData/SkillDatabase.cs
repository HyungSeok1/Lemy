using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Game/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private List<SkillData> allSkillDataList;

    private Dictionary<string, SkillData> skillDataDict;

    //public IReadOnlyList<SkillData> AllSkillDataList => allSkillDataList; // 읽기전용 프로퍼티 추가


    private void OnEnable()
    {
        skillDataDict = new Dictionary<string, SkillData>();
        foreach (var skillData in allSkillDataList)
        {
            skillDataDict[skillData.skillName] = skillData;
        }
    }

    /// <summary>
    /// 지정된 GameObject에 스킬 컴포넌트를 추가하고 반환합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="skillName"></param>
    /// <returns></returns>
    public ISkill AddSkillComponent(GameObject target, string skillName)
    {
        // 대응되는 SkillData 찾기
        if (!skillDataDict.TryGetValue(skillName, out var skillData))
        {
            Debug.Log("data 못 찾음");
            return null;
        }

        switch (skillName)
        {
            case "CircleSlash":
                var existingCircleSlash = target.GetComponent<CircleSlashSkill>();
                if (existingCircleSlash != null) return existingCircleSlash;
                var skill = target.AddComponent<CircleSlashSkill>();
                skill.data = (CircleSlashSkillData)skillData;
                return skill;

            case "Slash":
                var existingSlash = target.GetComponent<SlashSkill>();
                if (existingSlash != null) return existingSlash;
                var slash = target.AddComponent<SlashSkill>();
                slash.data = (SlashSkillData)skillData;
                return slash;

            case "Explosion":
                var existingExplosion = target.GetComponent<ExplosionSkill>();
                if (existingExplosion != null) return existingExplosion;
                var explosion = target.AddComponent<ExplosionSkill>();
                explosion.data = (ExplosionSkillData)skillData;
                return explosion;

            case "Heal":
                var existingHeal = target.GetComponent<HealSkill>();
                if (existingHeal != null) return existingHeal;
                var heal = target.AddComponent<HealSkill>();
                heal.data = (HealSkillData)skillData;
                return heal;

            case "Dash":
                // 이미 있으면 그냥 Get해서 return (에디터에서 테스트하려고 스킬 넣어놓은 경우)
                var existingDash = target.GetComponent<DashSkill>();
                if (existingDash != null) return existingDash;
                // 없으면 더해줌 (인게임에선 항상 사실상 이렇게 됨)
                var dash = target.AddComponent<DashSkill>();
                dash.data = (DashSkillData)skillData;
                return dash;
            case "Ghost":
                var existingSkill = target.GetComponent<GhostSkill>();
                if (existingSkill != null) return existingSkill;
                var ghost = target.AddComponent<GhostSkill>();
                ghost.data = (GhostSkillData)skillData;
                return ghost;
            case "Shield":
                var existingShield = target.GetComponent<ShieldSkill>();
                if (existingShield != null) return existingShield;
                var shield = target.AddComponent<ShieldSkill>();
                shield.data = (ShieldSkillData)skillData;
                return shield;
            default:
                Debug.LogWarning("skillName 지정이 잘못됨");
                return null;
        }
    }
}
