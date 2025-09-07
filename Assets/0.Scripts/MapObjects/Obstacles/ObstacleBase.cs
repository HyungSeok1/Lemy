using UnityEngine;

/// <summary>
/// 닿으면 데미지를 입는 기능뿐인 Obstacle이 전부 상속하는 추상 클래스입니다.
/// 
/// </summary>
public abstract class ObstacleBase : MonoBehaviour
{
    protected abstract float Damage { get; }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player == null) return;

        player.health.TakeDamage(Damage, (player.transform.position - transform.position).normalized);
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;

        player.health.TakeDamage(Damage, (player.transform.position - transform.position).normalized);
    }
    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player == null) return;
        player.health.TakeDamage(Damage, (player.transform.position - transform.position).normalized);
    }
}
