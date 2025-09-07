using UnityEngine;
using System.Collections;

/// <summary>
/// · blinkCount 회 깜빡이는 동안 target(보통 Player)을 계속 추적
/// · 깜빡임이 끝나면 delayAfterBlink 초 기다렸다가 실제 번개를 소환
/// · 자신은 번개를 소환한 직후 파괴
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class LightningWarner : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target;                 // ZoneController가 넣어 줌

    //[Header("깜빡임 설정")]
    //[Range(1, 10)] public int blinkCount = 2;
    //public float blinkInterval = 0.25f;

    [Header("번개 소환 지연")]
    [Tooltip("깜빡임 종료 후 실제 번개가 나타나기까지의 지연(초)")]
    public float delayAfterBlink = 0.15f;    // Inspector에서 조정 가능

    [Header("실제 번개 프리팹")]
    public GameObject lightningPrefab;       // ZoneController가 주입

    public float warningDuration = 1f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1f, 1f, 0f);

    private Vector3 initialScale;

    private Transform maskTransform;

    /* ----------------------------------------------------------- */
    SpriteRenderer sr;
    //bool followTarget = true;      // 깜빡이는 동안만 true

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        maskTransform = transform.Find("MaskSprite");
        if (maskTransform == null)
        {
            Debug.LogError("MaskSprite 자식 오브젝트가 없습니다.");
            return;
        }
    }

    public void Setting(float scaleMultiplier, float time)
    {
        warningDuration = time;
        initialScale = transform.localScale * scaleMultiplier;
        transform.localScale = initialScale; // 초기 스케일 설정
    }

    public void StartWarning()
    {
        //sr.enabled = true; // 깜빡임 시작 시 스프라이트 렌더러 활성화
        //StartCoroutine(BlinkFollowAndSpawn());
        StartCoroutine(ShrinkAndSpawn());
    }

    IEnumerator ShrinkAndSpawn()
    {
        float timer = 0f;
        Vector3 originalScale = maskTransform.localScale;
        while (timer < warningDuration)
        {
            float t = timer / warningDuration;
            float scale = scaleCurve.Evaluate(t);
            maskTransform.localScale = new Vector3(originalScale.x * scale, originalScale.y, originalScale.z);

            timer += Time.deltaTime;
            yield return null;
        }
        maskTransform.localScale = new Vector3(0f, originalScale.y, originalScale.z);

        Destroy(gameObject);
    }

    //void Update()
    //{
    //    // 깜빡이는 동안 매 프레임 target 위치로 스냅
    //    if (followTarget && target != null)
    //        transform.position = target.position;
    //}

    //IEnumerator BlinkFollowAndSpawn()
    //{
    //    bool visible = true;
    //    for (int i = 0; i < blinkCount; i++)
    //    {
    //        sr.enabled = visible;
    //        visible = !visible;
    //        yield return new WaitForSeconds(blinkInterval);
    //    }

    //    /* 깜빡임 종료 → 화면에서 사라짐 */
    //    sr.enabled = false;
    //    followTarget = false;         // 더 이상 플레이어를 따라가지 않음

    //    /* 지연 시간 대기 */
    //    yield return new WaitForSeconds(delayAfterBlink);

    //    /* 실제 번개 소환 */
    //    if (lightningPrefab != null)
    //        Instantiate(lightningPrefab, transform.position, transform.rotation);

    //    Destroy(gameObject);
    //}
}

