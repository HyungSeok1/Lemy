using DG.Tweening;
using UnityEngine;

public class LogoWobble : MonoBehaviour
{
    [SerializeField] private RectTransform[] letters; // L, e, m, y 각각 할당
    [SerializeField] private float moveDistance = 10f; // 좌우 흔들림 거리 (픽셀)
    [SerializeField] private float duration = 15f; // 왕복 시간 (15초 정도)

    void Start()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            float delay = Random.Range(0f, 2f); // 약간 랜덤하게 시작 박자 달리기

            letters[i].DOAnchorPosX(
                letters[i].anchoredPosition.x + moveDistance, duration / 2f
            )
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복, 왕복
            .SetDelay(delay); // 서로 박자 어긋나게
        }
    }
}
