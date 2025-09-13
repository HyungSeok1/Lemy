using DG.Tweening;
using UnityEngine;

public class LemyWobble : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] private float delay;

    [SerializeField] private float moveDistance;
    [SerializeField] private float cycleDuration;

    void Start()
    {
        transform.DOMoveX(transform.position.x + moveDistance, cycleDuration / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(delay);
    }
}
