using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LaserBeam2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint; // 발사대에서 빔 시작점(보통 Emitter의 총구)
    [SerializeField] private SpriteRenderer spriteRenderer; // 또는 LineRenderer로 대체 가능
    [SerializeField] private LayerMask hitMask; // 빔을 막는 레이어 (벽/구조물 등)
    [SerializeField] private LayerMask ignoreMaskForBeam; // 레이저/트리거 등 무시

    [Header("Beam Settings")]
    [SerializeField] private float maxLength = 50f;
    [SerializeField] private float thickness = 1f;
    [SerializeField] private float colliderPadding = 0.05f;

    [Header("Damage")]
    [SerializeField] private int playerDamage = 30;
    [SerializeField] private float damageInterval = 0.25f; // 충돌 유지 시 피해 주기
    [SerializeField] private float knockbackForce = 6f;

    private BoxCollider2D box;
    private Rigidbody2D rb;

    // 피격 쿨다운 관리
    private readonly Dictionary<Collider2D, float> _nextDamageTime = new();

    // 현재 유효 길이
    private float currentLength = 0f;
    public bool IsOn { get; private set; } = false;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0;
        box.isTrigger = true; // 통과 가능
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        SetEnabled(false, true);
    }

    private void FixedUpdate()
    {
        if (!IsOn || !firePoint) return;

        // Raycast로 빔 끝점 계산
        Vector2 origin = firePoint.position;
        Vector2 dir = firePoint.right; // 오른쪽 방향을 발사 방향으로 사용(오브젝트 회전으로 제어)
        float length = maxLength;

        // 첫 충돌체 확인
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxLength, hitMask);
        if (hit.collider)
        {
            length = hit.distance;
        }

        currentLength = length;

        // Sprite 길이/위치 갱신 (Sprite pivot이 가운데라 가정)
        if (spriteRenderer)
        {
            // 가로로 늘리는 방식: localScale.x = length / spritePixelToUnit
            // 단순화: sprite는 1x1 유닛 가정 후 scale로 늘림
            transform.position = origin + dir * (length * 0.5f);
            transform.right = dir;
            transform.localScale = new Vector3(length, thickness, 1f);
        }
    }

    public void SetEnabled(bool on, bool instant = false)
    {
        IsOn = on;
        gameObject.SetActive(true); // 본체는 항상 켜두고, on/off는 렌더/충돌로 제어해도 됨

        // 시각/충돌 토글
        if (spriteRenderer) spriteRenderer.enabled = on;
        box.enabled = on;

        if (!on)
        {
            _nextDamageTime.Clear();
        }

        if (!on && instant)
        {
            // 완전 비활성할거면 오브젝트 끄기
            if (spriteRenderer) spriteRenderer.enabled = false;
            box.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!IsOn) return;

        // 충돌 평균 법선 → 레이저에서 밀어내는 방향
        Vector2 avgNormal = Vector2.zero;
        //foreach (var cp in collision.contacts) avgNormal += cp.normal;
        //avgNormal = avgNormal.sqrMagnitude > 0.0001f ? avgNormal.normalized : -transform.right; 

        // 1) Enemy: 즉사
        if (col.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(int.MaxValue); 
            return;
        }

        // 2) Player: PlayerHealth로 데미지 + 넉백X (무적/리스폰 규칙은 PlayerHealth 내부에서 처리)
        if (col.TryGetComponent<Player>(out Player player))
        {
            float now = Time.time;
            if (!_nextDamageTime.TryGetValue(col, out float next) || now >= next)
            {
                _nextDamageTime[col] = now + damageInterval;
                player.health.TakeDamage(playerDamage, avgNormal);
            }
        }
    }

    private bool IsInLayer(LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        if (!firePoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * (currentLength > 0 ? currentLength : maxLength));
    }
}
