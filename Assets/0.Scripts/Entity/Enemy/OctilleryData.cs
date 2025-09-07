using UnityEngine;

[CreateAssetMenu(fileName = "NewOctilleryData", menuName = "Game/Enemy/OctilleryData")]
public class OctilleryData : EnemyData
{
    [Header("감지 / 공격")]
    [Tooltip("감지 범위(시야)")]
    public float detectionRange = 18f;   // 튜닝 가능

    [Tooltip("공격 쿨타임(초)")]
    public float attackCooldown = 4f;

    [Header("투사체 속도")]
    public float projectileSpeed = 22f;

    [Header("데미지")]
    public float damage = 20f;
}
