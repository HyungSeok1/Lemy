using UnityEngine;

[CreateAssetMenu(fileName = "NewJunkMassData", menuName = "Game/Enemy/JunkMassData")]
public class JunkMassData : EnemyData
{
    [Header("Enemy별 변수")]
    public float moveSpeed;
    public float detectionRange;
    public float activityRange;
    public float attackCooldown;

    [Tooltip("어그로 해제 거리 계수. DetectionRange의 몇 배 이상 멀어지면 어그로를 풀지 결정합니다. \"1.n\" 추천")]
    public float chaseDetectionRangeMultiplier;

    public float damage;

    [Tooltip("쫒아가다 detectionRange 밖으로 넘어가면, 제자리에 멈춰서 잠깐 기다리는 시간")]
    public float stayTime;

    public float wanderRadius;
    public float wanderInterval; // 랜덤하게 돌아다니는 시간 간격
}
