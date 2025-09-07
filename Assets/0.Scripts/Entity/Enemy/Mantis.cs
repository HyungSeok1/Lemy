using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 사마귀: 제자리 근접형. 시야에 들어오면 즉시 경고(부채꼴) 후 1초 뒤 스윙 데미지.
/// 이후 attackInterval 간격으로 반복. 이동 없음.
/// 접촉 시 contactDamage 별도 적용.
/// </summary>
public class Mantis : Enemy
{
    [SerializeField] private MantisData data;
    [SerializeField] private GameObject plainHitEffect;

    // EnemyData 공통
    private int MaxHealth => data.maxHealth;

    // 사마귀 전용
    private float DetectionRange => data.detectionRange;
    private float AttackRange => data.attackRange;
    private float AttackArcDeg => data.attackArcAngle;
    private float ContactDamage => data.contactDamage;
    private float MeleeDamage => data.meleeDamage;
    private float AttackInterval => data.attackInterval;

    [Header("경고(부채꼴) 설정")]
    [Tooltip("경고 표시 유지 시간(초). 설계상 1초 고정 권장")]
    [SerializeField] private float warningDuration = 1f; // 요구사항: 경고 이후 1초 뒤 공격
    [Tooltip("경고 메쉬의 반경 분할 수(값이 클수록 매끄러운 부채꼴)")]
    [SerializeField] private int warningSegments = 24;
    [Tooltip("경고 색상(투명도 포함)")]
    [SerializeField] private Color warningColor = new Color(1f, 0f, 0f, 0.35f);

    private Coroutine attackLoop;
    private bool isAttacking;
    private bool isReady;

    Animator animator;

    private Vector2 lastAttackForward = Vector2.right;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isReady = false;
    }

    protected override void Start()
    {
        base.Start();
        currentHealth = MaxHealth;
    }

    /// <summary>강한 근접 스윙 판정. 플레이어가 부채꼴 안에 있으면 데미지+넉백</summary>
    protected override void Attack(Player player)
    {
        if (player == null) return;
        if (animator != null) animator.SetTrigger("Attack");
        isReady = false;

        Vector2 toPlayer = (player.transform.position - transform.position);
        float dist = toPlayer.magnitude;
        if (dist > AttackRange) return;

        Vector2 aimDir = toPlayer.normalized;

        Vector3 pos = transform.position;
        SoundManager.Instance.PlaySFXAt("mantisSlash1", pos, 0.05f);
        float angle = Vector2.Angle(lastAttackForward, toPlayer.normalized);
        if (angle <= AttackArcDeg * 0.5f)
        {
            player.health.TakeDamage(MeleeDamage, toPlayer.normalized);
        }
    }

    #region FSM
    protected override void OnEnterState(State state)
    {
        switch (state)
        {
            case State.Idle:
                StopAttackLoop();
                break;

            case State.Attack:
                if (attackLoop == null)
                    attackLoop = StartCoroutine(AttackRoutine());
                break;
        }
    }

    protected override void OnUpdateState(State state)
    {
        float distance = Vector3.Distance(transform.position, Player.Instance.transform.position);

        switch (state)
        {
            case State.Idle:
                // 플레이어가 시야에 들어오면 즉시 공격 상태로 진입
                if (distance <= DetectionRange)
                    ChangeState(State.Attack);
                break;

            case State.Attack:
                // 플레이어가 시야에서 벗어나면 공격 루프 중단 및 Idle
                if (distance > DetectionRange)
                    ChangeState(State.Idle);
                break;
        }
    }

    protected override void OnExitState(State oldState)
    {
        // 특별 처리 없음. Attack 루프는 Idle 진입에서 끈다.
    }
    #endregion

    private IEnumerator AttackRoutine()
    {
        // 설계: 상태 진입 시 "즉시 1회" 시전(= 경고 → 1초 후 스윙), 그 후 interval 반복
        while (currentState == State.Attack)
        {
            // 현재 플레이어 방향 기준으로 경고 생성
            if (!isReady)
            {
                if (animator != null) animator.SetTrigger("Ready");
                print("asdfsadfasdf");
                isReady = true;
            }

            Vector2 aimDir = ((Vector2)Player.Instance.transform.position - (Vector2)transform.position).normalized;
            lastAttackForward = aimDir;
            var warn = CreateFanWarning(AttackRange, AttackArcDeg, aimDir, warningSegments, warningColor);
            Destroy(warn, warningDuration);
            isReady = false;
            yield return new WaitForSeconds(warningDuration);




            // 실제 공격 판정
            Attack(Player.Instance);

            // 공격 애니메이션
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Instantiate(plainHitEffect, transform.position, rot, transform);

            // 반복 간격
            float elapsed = 0f;
            while (elapsed < AttackInterval)
            {
                // 중간에 시야에서 벗어나면 즉시 종료
                if (Vector3.Distance(transform.position, Player.Instance.transform.position) > DetectionRange)
                {
                    StopAttackLoop();
                    ChangeState(State.Idle);
                    isReady = false;
                    yield break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void StopAttackLoop()
    {
        if (attackLoop != null)
        {
            StopCoroutine(attackLoop);
            attackLoop = null;
        }
    }

    /// 플레이어와 닿아있는 동안 접촉 데미지. Cyc쿨은 PlayerHealth가 관리)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
            Attack(player);
    }

    protected override void Die()
    {
        base.Die();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        Vector3 pos = transform.position;
        SoundManager.Instance.PlaySFXAt("mantisHit1", pos, 0.05f);

        print("몹 피해 입음");
    }

    #region 부채꼴 경고(빨간 Sprite 대체 구현: 런타임 메쉬 생성)
    // 경고용 부채꼴 메쉬를 즉석 생성해서 표시. 이후 따로 경고 prefab으로 대체 가능

    private GameObject CreateFanWarning(float radius, float arcDeg, Vector2 forward, int segments, Color color)
    {
        // GameObject 구성
        GameObject go = new GameObject("MantisAttackWarning");
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(forward.x, forward.y, 0f));

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var sg = go.AddComponent<SortingGroup>();
        // Sprites/Default로 투명 색상 가능
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.material.color = color;
        mr.sortingLayerName = "Effects"; // 프로젝트 정합에 맞춰 조정

        sg.sortingOrder = 10;

        // 메쉬 생성(원점 기준 삼각형 팬)
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colors = new List<Color>();

        verts.Add(Vector3.zero);
        colors.Add(color);

        float half = arcDeg * 0.5f;
        float start = -half;
        float step = arcDeg / segments;

        for (int i = 0; i <= segments; i++)
        {
            float a = (start + step * i) * Mathf.Deg2Rad;
            verts.Add(new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f));
            colors.Add(color);
        }

        // 삼각형 인덱스
        for (int i = 1; i < verts.Count - 1; i++)
        {
            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mf.sharedMesh = mesh;
        return go;
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        // 시야
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        // 공격 부채꼴 미리보기(현재 바라보는 방향 기준)
        Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
        DrawGizmoFan(transform.position, transform.right, AttackRange, AttackArcDeg, 24);
    }

    private void DrawGizmoFan(Vector3 center, Vector2 forward, float radius, float arcDeg, int segments)
    {
        float half = arcDeg * 0.5f;
        float start = -half;
        float step = arcDeg / segments;

        Vector3 prev = center + (Vector3)(Quaternion.Euler(0, 0, start) * forward) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float a = start + i * step;
            Vector3 cur = center + (Vector3)(Quaternion.Euler(0, 0, a) * forward) * radius;
            Gizmos.DrawLine(prev, cur);
            Gizmos.DrawLine(center, cur);
            prev = cur;
        }
    }
#endif
}
