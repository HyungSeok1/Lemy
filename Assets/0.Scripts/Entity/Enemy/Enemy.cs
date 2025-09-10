using System;
using System.Collections;
using UnityEngine;

/// <summary>
///
/// </summary>
public abstract class Enemy : Entity, IDamageable
{
    public static event Action<Vector3, Vector3> OnEnemyDamaged;
    public event Action OnDie;

    protected int currentHealth;
    [SerializeField] protected ItemData DropItemData;
    [SerializeField] protected int DropItemCount = 1;

    //색깔 변경용 변수

    private SpriteRenderer colorSpriteRenderer;
    private Color originalColor;

    private void Initialize()
    {

        colorSpriteRenderer = GetComponent<SpriteRenderer>();
        if (colorSpriteRenderer != null)
        {
            originalColor = colorSpriteRenderer.color;
        }

        ChangeState(State.Idle);

    }


    protected virtual void Start()
    {
        Initialize();
    }


    public virtual void TakeDamage(int damage)
    {
        if (currentHealth - damage <= 0)
            Die();
        else
        {
            currentHealth -= damage;
        }


        // 데미지 입는순간 살짝 하얘지기
        if (colorSpriteRenderer != null)
        {
            StartCoroutine(FlashWhite());
        }

        OnEnemyDamaged?.Invoke(Player.Instance.transform.position, transform.position);

        SoundManager.Instance.PlaySFX("frontSlash2", 0.1f);
    }

    public virtual void Knockback(Vector2 direction, float force = 5f)
    {
        StartCoroutine(KnockbackCoroutine(direction, force));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)direction.normalized * force;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    protected abstract void Attack(Player player);

    protected virtual void Die()
    {
        OnDie?.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()

    {
        colorSpriteRenderer.color = new Color(3, 3, 3);
        yield return new WaitForSeconds(0.2f); // 0.2초 동안 흰색 유지
        colorSpriteRenderer.color = originalColor;
    }



    #region Enemy FSM
    public enum State { Idle, Chase, Attack, Dead, Return, Stay }

    // 현재 State 변수
    protected State currentState;

    protected abstract void OnEnterState(State newState);
    protected abstract void OnUpdateState(State state);
    protected abstract void OnExitState(State oldState);

    void Update()
    {
        OnUpdateState(currentState);
    }

    public void ChangeState(State newState)
    {
        OnExitState(currentState);
        currentState = newState;
        OnEnterState(currentState);
    }

    #endregion
}

