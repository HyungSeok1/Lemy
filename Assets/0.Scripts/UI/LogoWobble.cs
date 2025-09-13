using DG.Tweening;
using UnityEngine;

public class LogoWobble : MonoBehaviour
{
    [SerializeField] private RectTransform[] letters; // L, e, m, y 각각 할당
    [SerializeField] private float[] delay;

    [SerializeField] private float moveDistance;
    [SerializeField] private float cycleDuration;

    void Start()
    {
        for (int i = 0; i < letters.Length; i++)
        {

            letters[i].DOAnchorPosX(letters[i].anchoredPosition.x + moveDistance, cycleDuration / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo) 
            .SetDelay(delay[i]); 
        }
    }
}
