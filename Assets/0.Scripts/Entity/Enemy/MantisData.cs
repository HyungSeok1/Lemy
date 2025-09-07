using UnityEngine;

[CreateAssetMenu(fileName = "NewMantisData", menuName = "Game/Enemy/MantisData")]
public class MantisData : EnemyData
{
    [Header("감지/공격")]
    [Tooltip("부채꼴 근접 공격의 반경(거리)")]
    public float attackRange = 10f;                 // 플레이어 기본 공격보다 넓게(임시 10)

    [Tooltip("공격 부채꼴 각도(도)")]
    [Range(10f, 180f)] public float attackArcAngle = 180f;

    [Tooltip("시야 범위")]
    public float detectionRange = 14f;              // 

    [Header("데미지/쿨타임")]
    public float contactDamage = 20f;               // 접촉 데미지
    public float meleeDamage = 40f;                 // 근접 스윙 데미지

    [Tooltip("시야에 들어오면 즉시 1회 공격 후 반복 간격")]
    public float attackInterval = 3f;
}
