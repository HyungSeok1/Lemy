using DG.Tweening;
using UnityEngine;

public class SmallKey : MonoBehaviour
{
    /// <summary>
    /// small Key maker와 함께 사용
    /// 먹으면 알림을 보내고 자신을 제거하는 열쇠
    /// 
    /// </summary>
    /// 

    [HideInInspector]
    public AvoidChallenge maker;

    [SerializeField] GameObject keyEffect;

    private void OnEnable()
    {
        if (keyEffect == null)
        {
            keyEffect = transform.Find("KeyEffect").gameObject;
        }

        if (keyEffect != null)
        {
            var sr = keyEffect.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.DOKill(); // 혹시 모를 중복 애니메이션 제거
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
                sr.DOFade(0f, 1f) // 1초 동안 alpha 0까지
                  .SetLoops(-1, LoopType.Yoyo); // 무한 반복 (1 ↔ 0 ↔ 1)
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Maker에게 알림
            if (maker != null)
                maker.OnKeyCollected();

            // 자신 제거
            Destroy(gameObject);
        }
    }
}
