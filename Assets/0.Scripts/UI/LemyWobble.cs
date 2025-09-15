using DG.Tweening;
using UnityEngine;

public class LemyWobble : MonoBehaviour
{
    [Range(0f, 10f)]
    [SerializeField] private float jitterRadius;   // 중심에서 벗어나는 최대 반경 (World 단위 or Local 단위)

    [Range(0f, 10f)]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float startDelay;       // 시작 지연

    private Vector3 originalPosition;
    private Tweener brownianTween;
    private const float MIN_DURATION = 0.01f;

    private void Awake()
    {
        // TODO: 앵커에 맞춰서 위치 초기화 (월드 오브젝트라 앵커 적용 안됨)
    }

    private void Start()
    {
        
        SoundManager.Instance.PlayBGM("gwanmoon_bgm", 1f); // 여기 사운드좀 넣겠습니다 죄송합니다 - 박재용
        originalPosition = transform.localPosition;
        Invoke(nameof(StartBrownianMotion), startDelay);

    }

    private void StartBrownianMotion()
    {
        StopBrownianMotion();
        MoveToRandomPosition();
    }

    private void MoveToRandomPosition()
    {
        Vector2 randomOffset2D = Random.insideUnitCircle * jitterRadius;
        Vector3 randomOffset = new Vector3(randomOffset2D.x, randomOffset2D.y, 0f);

        Vector3 target = originalPosition + randomOffset;
        float distance = Vector3.Distance(transform.localPosition, target);

        float duration = moveSpeed > 0f ? Mathf.Max(distance / moveSpeed, MIN_DURATION) : 999999f;

        brownianTween = transform.DOLocalMove(target, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(MoveToRandomPosition); // 반복
    }

    private void StopBrownianMotion()
    {
        if (brownianTween != null && brownianTween.IsActive())
        {
            brownianTween.Kill();
            brownianTween = null;
        }
        transform.localPosition = originalPosition;
    }
}
