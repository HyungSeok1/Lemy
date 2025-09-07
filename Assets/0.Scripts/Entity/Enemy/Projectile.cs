using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 startPosition;
    private Vector2 direction;
    private float duration;
    private float moveSpeed;
    private float scale;

    private float startTime;
    private Rigidbody2D rb;

    private bool hit = false;
    private float damage;

    public void Initialize(Vector2 pos, Vector2 dir, float dur, float speed, float dmg, float size)
    {
        startPosition = pos;
        transform.position = startPosition;
        direction = dir;
        duration = dur;
        moveSpeed = speed;
        damage = dmg;
        scale = size;
        transform.localScale = new Vector3(size, size, size);

        startTime = Time.time;

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (((Time.time - startTime) <= duration) && !hit)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player player))
        {
            player.health.TakeDamage(damage, (player.transform.position - transform.position).normalized);
            hit = true;
        }

        if(collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
