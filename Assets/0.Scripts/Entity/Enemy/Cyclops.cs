using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 접촉해서 데미지 주는 Enemy
/// 
/// 
/// </summary>

public class Cyclops : Enemy
{
    [SerializeField] private CyclopsData data;

    // Enemy 관련 데이터들
    private int Maxhealth => data.maxHealth;
    private float MoveSpeed => data.moveSpeed;
    private float DetectionRange => data.detectionRange;
    private float Damage => data.damage;
    private float WanderInterval => data.wanderInterval;
    private float WanderRadius => data.wanderRadius;
    private float StayTime => data.stayTime;
    private float ChaseDetectionRangeMultiplier => data.chaseDetectionRangeMultiplier;

    Animator anim;
    private NavMeshAgent agent;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected override void Start()
    {
        base.Start();
        currentHealth = Maxhealth;

        originPoint = transform.position;
    }

    protected override void Attack(Player player)
    {
        player.health.TakeDamage(Damage, (player.transform.position - transform.position).normalized);
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        //이펙트, 애니메이션 등
    }

    protected override void Die()
    {
        print("몹 죽음");
        //파티클같은거
        anim.SetTrigger("isDie");
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        base.Die();
    }

    private void OnDieAnimationEnd()
    {
        if (this.transform.parent != null)
        {
            GameObject parentObject = this.transform.parent.gameObject;
            Destroy(parentObject);
        }
    }

    private void OnDieAnimationStart()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }


#if UNITY_EDITOR
    //너무 많아져서 끄고싶으면 코드 지우셔도 됩니다
    private void OnDrawGizmos()
    {
        if (data == null) return;

        // 탐지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        // 충분히 멀어져 탐지 끊기는 반경
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ChaseDetectionRangeMultiplier * DetectionRange);

        // Wander 반경
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(originPoint, WanderRadius);
    }
#endif

    #region FSM
    protected override void OnEnterState(State state)
    {
        switch (state)
        {
            case State.Idle:
                wanderTimer = 0f;
                break;
            case State.Chase:
                break;
            case State.Stay:
                remainingStayTime = StayTime;
                break;
            case State.Return:
                break;
            case State.Dead:
                print("죽음");
                //파티클, 애니메이션 등
                break;

        }
    }

    /// <summary>
    /// 
    /// 로직은 메서드로 짜고,
    /// 
    /// 상태 전환은 OnUpdateState에 if문으로 넣었습니다.
    /// 
    /// 메서드에 직접 들어가 상태 전환 조건이 무엇인지 확인할 필요 없도록.
    /// 
    /// </summary>
    protected override void OnUpdateState(State state)
    {
        float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

        switch (state)
        {
            case State.Idle:
                WanderAround();
                if (distance <= data.detectionRange)
                    ChangeState(State.Chase);
                break;

            case State.Chase:
                ChasePlayer();
                if (distance > ChaseDetectionRangeMultiplier * DetectionRange) // 멀어지면 어그로 풀림
                {
                    if (IsinWanderZone())
                        ChangeState(State.Idle);
                    else
                        ChangeState(State.Return);
                }
                break;

            case State.Return:
                ReturnToWanderArea();
                if (transform.position == originPoint)
                    ChangeState(State.Idle);
                else if (distance <= data.detectionRange)
                    ChangeState(State.Chase);
                break;

            case State.Stay:
                Stay();
                if (distance <= data.detectionRange)
                    ChangeState(State.Chase);
                else if (remainingStayTime <= 0)
                    ChangeState(State.Return);
                break;
        }
    }

    protected override void OnExitState(State state)
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Chase:
                break;
        }
    }

    #endregion

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
            Attack(player);
    }

    private bool IsinWanderZone()
    {
        return Vector3.Distance(originPoint, transform.position) < WanderRadius;
    }

    private float wanderTimer = 0f;
    private Vector3 originPoint;
    // Idling == Patrolling. ( 어슬렁거린다 )
    private void WanderAround()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= WanderInterval)
        {
            Vector3 newPos = originPoint + Random.insideUnitSphere * WanderRadius;
            newPos.z = originPoint.z; // z 고정 (2D면 필요)
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(Player.Instance.transform.position);
    }

    private void ReturnToWanderArea()
    {
        agent.SetDestination(originPoint);
    }

    private float remainingStayTime;
    // Return할지, 다시 Chase할지 정하는 상황
    private void Stay()
    {
        remainingStayTime -= Time.deltaTime;
    }
}
