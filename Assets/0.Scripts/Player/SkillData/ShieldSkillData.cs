using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldSkillData", menuName = "Game/Skill/ShieldSkillData")]
public class ShieldSkillData : SkillData
{
    [Header("쿨다운 설정")]
    public float cooldown = 5f;

    [Header("스택 게이지 설정")]
    public float stackGaugeUsagePerSecond = 25f; // 초당 스택게이지 사용량

    [Header("방향 조작 설정")]
    public float shieldDirectionInputTime = 0.5f; // 쉴드 방향 변경 입력 시간

    [Header("실드 형태 설정")]
    public float radius = 1.5f; // 레미 가로 x radius
    [Range(30f, 180f)]
    public float arcAngle = 90f; // 쉴드 호 각도 (부채꼴)

    [Header("프리팹 설정")]
    public GameObject shieldPrefab; // 실드 시각적 프리팹

    [Header("애니메이션 설정")]
    public string animationTriggerName = "Shield";
}