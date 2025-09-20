using UnityEngine;

[CreateAssetMenu(fileName = "NewExplosionSkillData", menuName = "Game/Skill/ExplosionSkillData")]
public class ExplosionSkillData : SkillData
{
    public float cooldown;
    public float castRadius; // 사용 가능 범위
    public int damage;
    public float knockbackForce;
    public GameObject rangePrefab; // 시전 범위를 표시하는 프리팹
    public GameObject explosionEffectPrefab;
    public GameObject explosionParticlePrefab;

    // 애니메이션 관련 변수
    public string animationTriggerName;
}