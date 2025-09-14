using UnityEngine;

/// <summary>
/// 생성 '직후'에만 겹친 대상에게 1회 데미지, 이후에는 데미지 없음
/// 애니메이션이 있다면 AnimEvent_Destroy()로 소멸, 없다면 lifeTime 후 파괴
/// </summary>
public class Lightning : ObstacleBase
{
    [Header("Lifetime")]
    [Tooltip("몇 초 뒤에 삭제할지 (Animator 없을 때만 사용)")]
    [SerializeField] private float lifeTime = 1f;

    [Header("Damage")]
    [SerializeField] private float _damage;

    [SerializeField] private bool disableColliderAfterSpawnHit = true;

    [SerializeField] private Animator animator;

    private Collider2D _col;
    private bool _appliedSpawnHit = false;

    protected override float Damage => _damage;

    private void Awake()
    {
        SoundManager.Instance.PlaySFX("lightning2", 0.1f);

        if (!animator) animator = GetComponent<Animator>();
        _col = GetComponent<Collider2D>();

        if (!animator || animator.runtimeAnimatorController == null)
            Destroy(gameObject, lifeTime);
    }

    private void Start()
    {
        TryApplySpawnOverlapHit();
    }

    /// <summary>
    /// 생성 시점에 이미 겹쳐 있는 플레이어(또는 대상)에게만 1회 판정
    /// </summary>
    private void TryApplySpawnOverlapHit()
    {
        if (_appliedSpawnHit || !_col) return;

        // Player 레이어만 맞게 설정 (프로젝트 레이어명에 맞춰 변경)
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true; // 플레이어 콜라이더가 트리거여도 잡도록
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        filter.useLayerMask = true;

        // 필요 시 버퍼 크기 조정
        Collider2D[] results = new Collider2D[8];
        int count = _col.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = results[i];
            if (!hit) continue;

            base.OnTriggerEnter2D(hit);
        }

        _appliedSpawnHit = true;

        //콜라이더 비활성화
        if (disableColliderAfterSpawnHit && _col) _col.enabled = false;
    }

    public void AnimEvent_Destroy()
    {
        if (this && gameObject) Destroy(gameObject);
        Debug.Log("Lightning Destroyed by animator");
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        // 생성 시 1회 판정만 허용
        if (!_appliedSpawnHit) return; 
    }
}
