using UnityEngine;

[CreateAssetMenu(fileName = "NewSlashSkillData", menuName = "Game/Skill/SlashSkillData")] // 전방 베기
public class SlashSkillData : SkillData
{
    public int damage = 1;
    public float cooldown = 0.5f;
    //public string animationTriggerName; // 애니메이터를 그냥 SlashSkill 내에 넣어버림

    public GameObject slashPrefabUpToDown; // 베기 이펙트 프리팹 1
    public GameObject slashPrefabDownToUp; // 베기 이펙트 프리팹 2

    public float effectDuration = 2f;
    public float animationTime = 0.1f; // animationTime 이후 데미지 들어감
}
