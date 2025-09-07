using UnityEngine;

/// <summary>
/// 실드의 투사체 차단 기능을 담당하는 컴포넌트
/// 부채꼴 모양의 방어막으로 지정된 레이어의 투사체만 차단
/// </summary>
public class ShieldComponent : MonoBehaviour
{
    private float radius;
    private float arcAngle;
    private LayerMask projectileLayer;
    private bool isInitialized = false;

    public void Initialize(float shieldRadius, float shieldArcAngle, LayerMask targetLayer)
    {
        radius = shieldRadius;
        arcAngle = shieldArcAngle;
        projectileLayer = targetLayer;
        isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized) return;

        // 지정된 레이어의 오브젝트만 처리
        if (!IsInLayer(other.gameObject, projectileLayer)) return;

        // 부채꼴 범위 내에 있는지 확인
        if (IsInShieldArea(other.transform.position))
        {
            // 투사체 차단 - 투사체를 파괴하거나 비활성화
            HandleProjectile(other);
        }
    }

    private bool IsInLayer(GameObject obj, LayerMask layer)
    {
        return (layer.value & (1 << obj.layer)) > 0;
    }

    private bool IsInShieldArea(Vector3 projectilePosition)
    {
        Vector3 shieldCenter = transform.position;
        Vector3 shieldForward = transform.right; // 실드가 바라보는 방향

        // 거리 확인
        float distance = Vector3.Distance(shieldCenter, projectilePosition);
        if (distance > radius) return false;

        // 각도 확인 (부채꼴 범위)
        Vector3 directionToProjectile = (projectilePosition - shieldCenter).normalized;
        float angle = Vector3.Angle(shieldForward, directionToProjectile);

        return angle <= arcAngle / 2f;
    }

    private void HandleProjectile(Collider2D projectile)
    {
        // 투사체 처리 방법들:
        
        // 1. Projectile 컴포넌트를 가지고 있다면 (주요 투사체 클래스)
        var projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            // 투사체 파괴
            CreateBlockEffect(projectile.transform.position);
            Destroy(projectile.gameObject);
            return;
        }

        // 2. 일반적인 태그 기반 투사체라면
        var projectileGO = projectile.gameObject;
        if (projectileGO.CompareTag("EnemyProjectile") || 
            projectileGO.CompareTag("EnemyTusache") || 
            projectileGO.CompareTag("Projectile"))
        {
            // 이펙트 생성
            CreateBlockEffect(projectile.transform.position);
            
            Destroy(projectileGO);
            return;
        }

        // 3. Rigidbody가 있는 경우 반사 (선택사항)
        var rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.magnitude > 0.1f) // 움직이는 오브젝트인지 확인
        {
            // 투사체를 반사시킬 수도 있음
            ReflectProjectile(rb);
            CreateBlockEffect(projectile.transform.position);
        }
    }

    private void CreateBlockEffect(Vector3 position)
    {
        // TODO: 차단 이펙트 생성
        // 파티클 시스템이나 이펙트 프리팹 인스턴스화
        Debug.Log($"Projectile blocked at {position}");
    }

    private void ReflectProjectile(Rigidbody2D projectileRb)
    {
        // 투사체 반사 로직
        Vector3 incomingDirection = projectileRb.linearVelocity.normalized;
        Vector3 shieldNormal = -transform.right; // 실드 법선 벡터
        
        Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, shieldNormal);
        projectileRb.linearVelocity = reflectedDirection * projectileRb.linearVelocity.magnitude;
        
        // 투사체의 태그를 변경하여 적에게 데미지를 주도록 할 수도 있음
        projectileRb.gameObject.tag = "PlayerProjectile";
    }

    // 디버그용 시각화
    private void OnDrawGizmosSelected()
    {
        if (!isInitialized) return;

        Gizmos.color = Color.blue;
        Vector3 center = transform.position;
        Vector3 forward = transform.right;

        // 부채꼴 그리기
        float halfAngle = arcAngle / 2f;
        int segments = 20;
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + (halfAngle * 2f / segments) * i;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * forward;
            Vector3 point = center + direction * radius;
            
            if (i == 0)
            {
                Gizmos.DrawLine(center, point);
            }
            else
            {
                float prevAngle = -halfAngle + (halfAngle * 2f / segments) * (i - 1);
                Vector3 prevDirection = Quaternion.AngleAxis(prevAngle, Vector3.forward) * forward;
                Vector3 prevPoint = center + prevDirection * radius;
                
                Gizmos.DrawLine(prevPoint, point);
            }
            
            if (i == segments)
            {
                Gizmos.DrawLine(center, point);
            }
        }
    }
}
