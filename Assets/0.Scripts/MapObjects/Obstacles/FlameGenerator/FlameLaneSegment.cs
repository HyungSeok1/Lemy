using System.Collections;
using UnityEngine;

/// 삼각형의 한 변(길) 조각. 양 끝을 트리밍해서 빈 통로(꼭짓점) 만든다.
public class FlameLaneSegment : MonoBehaviour
{
    Animator animator;

    [Header("Damage")]
    public float damage = 20f;

    [Header("Colors")]
    [SerializeField] private Color idleColor = new Color(1f, 1f, 1f, 0.15f); // 기본
    [SerializeField] private Color warnColor = new Color(1f, 0.6f, 0.6f, 0.55f); // 분홍 경고
    [SerializeField] private Color activeColor = new Color(1f, 0.15f, 0.15f, 0.9f); // 빨강 화염

    private int warningCount = 0;
    private int activeCount = 0;

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        ApplyVisual();
    }

    // ---- 공유 카운트 API ----
    public void AddWarning() { warningCount++; ApplyVisual(); }
    public void RemoveWarning() { warningCount = Mathf.Max(0, warningCount - 1); ApplyVisual(); }

    public void AddActive() 
    { 
        activeCount++;
        transform.localScale = new Vector3(2.5f, 3f, 1f);
        animator.SetBool("IsActive", true);
        ApplyVisual(); 
    }
    public void RemoveActive() 
    { 
        activeCount = Mathf.Max(0, activeCount - 1);
        StartCoroutine(ApplyScaleNextFrame(new Vector3(20f, 3f, 1f)));
        animator.SetBool("IsActive", false);
        ApplyVisual(); 
    }

    IEnumerator ApplyScaleNextFrame(Vector3 targetScale)
    {
        yield return null;
        transform.localScale = targetScale;
    }

    private void ApplyVisual()
    {
        if (activeCount > 0)
        {
            sr.color = activeColor;
            col.enabled = true;                 // 데미지 ON
        }
        else if (warningCount > 0)
        {
            sr.color = warnColor;
            col.enabled = false;                // 경고는 데미지 X
        }
        else
        {
            sr.color = idleColor;
            col.enabled = false;
        }
    }

    // 데미지 로직
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player == null) return;

        player.health.TakeDamage(damage, (player.transform.position - transform.position).normalized);
    }
    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player == null) return;
        player.health.TakeDamage(damage, (player.transform.position - transform.position).normalized);
    }
}
