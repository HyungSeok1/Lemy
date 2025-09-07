using UnityEngine;

/// <summary>
/// Octillery
/// 이동 없음, 활동 범위 없음
/// 시야에 플레이어가 한 번 들어오면 어그로 래치되어 계속 공격
/// 기존 투사체 script 적용
/// </summary>
public class Octillery: Enemy
{
    [SerializeField] private OctilleryData data;
    [SerializeField] private GameObject projectilePrefab; // 투사체 prefab

    [SerializeField] private Animator cannon;

    // Data shortcuts
    private int MaxHealth => data.maxHealth;      // 기본 1권장
    private float DetectionRange => data.detectionRange;
    private float AttackCooldown => data.attackCooldown;
    private float ProjectileSpeed => data.projectileSpeed;
    private float Damage => data.damage;

    private Animator animator;
    private bool aggroLatched = false;
    private float lastAttackTime = -999f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        currentHealth = Mathf.Max(1, MaxHealth); // 최소 1
    }

    protected override void Attack(Player player)
    {
        if (player == null || projectilePrefab == null) return;

        // 공격 애니메이션
        if (animator != null) animator.SetTrigger("Attack");
        if (cannon != null) cannon.SetTrigger("Attack");
        

        GameObject projectile = Instantiate(projectilePrefab); // 투사체 생성
        Vector2 dir = (new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
        projectile.GetComponent<Projectile>().Initialize(transform.position, dir, 15f, 15.0f, Damage, 1.0f); // 투사체 initialize
        lastAttackTime = Time.time;
    }

    // ===== FSM =====
    protected override void OnEnterState(State state)
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Attack:
                Attack(Player.Instance);
                // 한 번 쏜 뒤 Idle로 돌아가 쿨타임 체크
                ChangeState(State.Idle);
                break;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
            Attack(player);
    }
    protected override void OnUpdateState(State state)
    {
        var player = Player.Instance;
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        switch (state)
        {
            case State.Idle:
                // 한 번 시야에 들어오면 래치
                if (!aggroLatched && dist <= DetectionRange)
                    aggroLatched = true;

                // 어그로가 래치되면 쿨마다 계속 사격
                if (aggroLatched && Time.time - lastAttackTime >= AttackCooldown)
                    ChangeState(State.Attack);
                break;

            case State.Attack:
                // OnEnter에서 즉시 한 발 쏘고 Idle로 복귀
                break;
        }
    }

    protected override void OnExitState(State state) { }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
#endif
}
