using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 
/// 쓰레기덩어리 적입니다
/// 
/// RangedAttackEnemy상속 받으며, 기본 정보들은 Prefab>Enemy>JunkMass의 JunkMassData를 통해 수정 가능하며, 해당 data에 없는 세부 정보는 해당 파일을 직접 수정해야 합니다.
/// 
/// 3초 간격으로 projectile 오브젝트를 생성하여 플레이어 방향으로 발사합니다.
/// 
/// 첫 위치에서 일정 범위 내부를 랜덤하게 움직입니다.
/// 
/// 적 발견 시, 안전 거리 밖에 플레이어가 있다면 플레이어 방향으로, 안전거리 안쪽이면 플레이어에게서 도망가며, 활동 범위를 벗어나지 않습니다 
/// 플레이어에게 도망가는 백무빙 속도는 기본 이동속도보다 느립니다.
/// 
/// </summary>


// TODO: 로직 구현
public class JunkMass : Enemy
{
    // Idle, Chase, Attack, Search, Dead

    [SerializeField] private JunkMassData data;
    [SerializeField] private GameObject projectilePrefab;

    private int Maxhealth => data.maxHealth;
    private float MoveSpeed => data.moveSpeed;
    private float DetectionRange => data.detectionRange;
    private float ActivityRange => data.activityRange;
    private float AttackCooldown => data.attackCooldown;

    private float Damage => data.damage;
    private float WanderInterval => data.wanderInterval;
    private float WanderRadius => data.wanderRadius;
    private float StayTime => data.stayTime;
    private float ChaseDetectionRangeMultiplier => data.chaseDetectionRangeMultiplier;

    Vector3 tlqkf;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 homePos;


    private float safeDistance = 15.0f; // 캐릭터와의 유지하고자 하는 거리
    private float lastAttackTime;

    private float randomMoveRange = 10.0f; // 첫 위치에서 무작위로 움직일 수 있는 범위
    private float backMovingSpeed = 1.5f;

    private bool isAutoMoving = true;

    private Vector3 randomDir;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        homePos = transform.position;
        lastAttackTime = Time.time;
        //currentHealth = Maxhealth;

        StartCoroutine(ChangeDirectionRoutine());

    }

    protected override void Attack(Player player)
    {
        // 공격 애니메이션 트리거
        animator.SetTrigger("Attack");

        Vector3 Position = transform.position;
        SoundManager.Instance.PlaySFXAt("enemy_attack2", Position, 0.05f);
        GameObject projectile = Instantiate(projectilePrefab); // 투사체 생성
        Vector2 dir = (new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
        projectile.GetComponent<Projectile>().Initialize(transform.position, dir, 5.0f, 15.0f, Damage, 1.0f); // 투사체 initialize
        lastAttackTime = Time.time;
        // 플레이어에게 공격
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        //이펙트, 애니메이션 등
        print("몹 피해 입음");
    }

    protected override void Die()
    {
        // 죽음 애니메이션 트리거
        animator.SetBool("Die", true);

        SoundManager.Instance.PlaySFX("enemy_vanish2", 0.02f);
        print("몹 죽음");
        //파티클같은거

        // 죽음 애니메이션이 끝난 후 base.Die() 호출
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        // 애니메이터가 존재하는지 확인
        if (animator != null)
        {
            // 애니메이션 길이만큼 대기
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);
        }

        // 애니메이션이 완료된 후 실제 사망 처리 (Exit 노드 진입 시점)
        base.Die(); //todo: 나중에 프리팹 상단에 스크립트 옮기기
    }


    // ChangeState(state);  이전 state에서 OnExitState를 통해 나오고, OnEnterState를 통해 다음 state로 변경시켜줌.

    #region FSM

    protected override void OnEnterState(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                // Idle 애니메이션 트리거
                // animator.SetBool("IsIdle", true);

                isAutoMoving = true;
                StartCoroutine(ChangeDirectionRoutine()); // Idle 상태로 들어오며 무작위 움직임 방향 정하는 코루틴 Start
                break;
            // 사거리 안에 플레이어 진입. Chase지만, 해당 몬스터의 경우 플레이어와 일정 거리 유지하는 방향으로 이동
            case State.Chase:
                break;
            // 원거리 투사체 발사, 이후 쿨타임 3초 확인하도록 초기화
            case State.Attack:
                Attack(Player.Instance);
                break;
            // 사망. currentHealth가 private이라 ContactAttackEnemy에서 사망 판정에 대한 수정 필요할듯
            case State.Dead:
                // 모든 애니메이션 상태 리셋
                // animator.SetBool("IsIdle", false);
                break;
        }
    }

    protected override void OnUpdateState(State state) // 매 프레임 호출 
    {

        float distHomeToSelf = Vector3.Distance(homePos, transform.position);
        float distSelfToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);


        switch (state)
        {
            // 대기상태. 시야 범위 내부로 들어오면 활동 시작
            case State.Idle:
                if (distSelfToPlayer <= DetectionRange && distHomeToSelf <= ActivityRange) //시야 안에 플레이어 진입. Chase state로 변경
                {
                    isAutoMoving = false;
                    ChangeState(State.Chase);
                }
                else //활동 범위 내부를 어슬렁어슬렁 움직임
                    AutoMove();
                break;
            // 사거리 안에 플레이어 진입. Chase지만, 해당 몬스터의 경우 플레이어와 일정 거리 유지하는 방향으로 이동
            case State.Chase:
                if (distSelfToPlayer <= DetectionRange && distHomeToSelf <= ActivityRange) //활동 범위 내부, 시야 안에 플레이어가 있는 경우 안전거리 유지하며 이동
                    KeepSafeDistance(distSelfToPlayer);
                else if (distSelfToPlayer > DetectionRange) // 시야에서 멀어진 경우 Idle로 state 변경
                {
                    ChangeState(State.Idle);
                    break;
                }
                // 시야에 플레이어는 있지만, 활동 범위의 경계에 있는 경우 경계에 있으며 공격은 진행
                if (Time.time - lastAttackTime >= AttackCooldown) // 공격 쿨타임이 지난 경우, attack state로 넘어가 투사체 발사
                    ChangeState(State.Attack);
                break;
            // 원거리 투사체 발사, 이후 쿨타임 3초 확인하도록 초기화
            case State.Attack:
                ChangeState(State.Chase);
                break;
            // 사망. currentHealth가 private이라 ContactAttackEnemy에서 사망 판정에 대한 수정 필요할듯
            case State.Dead:
                Die();
                break;
        }
    }

    protected override void OnExitState(State oldState)
    {
        switch (oldState)
        {
            case State.Idle:
                isAutoMoving = false; // Idel상태에서 무작위 방향으로 이동하던 루틴 정지
                // animator.SetBool("IsIdle", false);
                break;
            // 사거리 안에 플레이어 진입. Chase지만, 해당 몬스터의 경우 플레이어와 일정 거리 유지하는 방향으로 이동
            case State.Chase:
                break;
            // 원거리 투사체 발사, 이후 쿨타임 3초 확인하도록 초기화
            case State.Attack:
                break;
            // 사망. currentHealth가 private이라 ContactAttackEnemy에서 사망 판정에 대한 수정 필요할듯
            case State.Dead:
                break;
        }
    }

    #endregion

    void KeepSafeDistance(float distSelfToPlayer)
    {
        if (Mathf.Abs(distSelfToPlayer - safeDistance) <= 0.1) // 안전거리의 경계에 있는 경우, 정지
        {
            agent.speed = 0f;
            return;
        }
        if (distSelfToPlayer <= safeDistance)// 안전거리보다 가까운 경우 플레이어와 반대 방향으로 이동
            MoveToward((2 * transform.position - Player.Instance.transform.position), backMovingSpeed, true);
        else
            MoveToward(Player.Instance.transform.position, MoveSpeed);
        return;
    }

    void AutoMove()
    {
        if (Vector3.Distance(homePos, transform.position) > randomMoveRange) // 무작위 움직임 범위 넘어 있는 경우, 초기 위치로 방향으로 재설정
        {
            randomDir = homePos;
        }
        MoveToward(randomDir, MoveSpeed);
    }

    void MoveToward(Vector3 direction, float moveSpeed, bool backMoving = false)
    {
        agent.speed = moveSpeed;
        agent.SetDestination(direction); // direction 방향으로 이동
        tlqkf = direction;
        if (!backMoving)
        {
            if (direction.x > 0) // 우측 이동시 sprite flip
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }
        else
        {
            if (direction.x > 0) // 우측 이동시 sprite flip
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
            Attack(player);
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (isAutoMoving)
        {
            randomDir = homePos + new Vector3(Random.Range(-11, 11), Random.Range(-11, 11), 0).normalized;
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }
}

