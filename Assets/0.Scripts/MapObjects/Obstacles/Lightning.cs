using UnityEngine;

/// <summary>
/// 소환 후 1초 뒤에 자동으로 사라지는 번개
/// 필요하다면 Damage, 이펙트, 사운드 등을 추가해 확장할 수 있습니다.
/// </summary>
public class Lightning : ObstacleBase
{
    [Tooltip("몇 초 뒤에 삭제할지")]
    [SerializeField] float lifeTime = 1f;
    [SerializeField] private float _damage;
    [SerializeField] private float damageTime = 0.3f;
    private bool hasDamage = true;

    protected override float Damage => _damage;
    [SerializeField] Animator animator;

    void Awake()
    {
        // lifeTime이 지나면 오브젝트 파괴
        SoundManager.Instance.PlaySFX("lightning2", 0.1f);
        if (!animator) animator = GetComponent<Animator>();
        if (!animator || animator.runtimeAnimatorController == null)
            Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (damageTime > 0)
            damageTime -= Time.deltaTime;
        else hasDamage = false;
    }

    public void AnimEvent_Destroy()
    {
        if (this && gameObject) Destroy(gameObject);
        Debug.Log("Lightning Destroyed by animator");
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!hasDamage) return;
        base.OnTriggerEnter2D(collider);
    }
}