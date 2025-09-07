using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ExplosionEffect class
/// 폭발 이펙트가 발생했을 때, 해당 위치의 적에게 데미지와 넉백을 적용하는 역할
/// 이펙트는 프리팹이 생성된 1틱에만 적용된다.
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    [HideInInspector] public ExplosionSkillData data; // 데미지, 넉백 등
    private void Start()
    {
        Collider2D skillCol = GetComponent<Collider2D>();
        List<Collider2D> hitColliders = new List<Collider2D>();

        // isTrigger 콜라이더를 위해 Overlap 사용
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(Physics2D.AllLayers);
        filter.useTriggers = true;
        skillCol.Overlap(filter, hitColliders);

        foreach (var collider in hitColliders)
        {
            // Player 태그가 붙은 오브젝트는 무시
            if (collider.CompareTag("Player")) continue;

            IDamageable damageable = collider.GetComponentInChildren<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(data.damage + Player.Instance.stats.bonusATK);

                Vector2 knockbackDir = (collider.transform.position - transform.position).normalized;

                if (damageable is Enemy enemy)
                    enemy.Knockback(knockbackDir, data.knockbackForce);
            }
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                float animationLength = clipInfo[0].clip.length;
                Destroy(gameObject, animationLength);
            }
            else
            {
                Destroy(gameObject, 1f);
            }
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }
}
