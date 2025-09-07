using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DamageEffectImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fadeTime;
    [SerializeField] private float targetAlpha;

    private void Start()
    {
        Player.Instance.health.OnDamaged += BlinkImageTwice;

        print(Player.Instance);
        print(Player.Instance.health);
    }

    private void BlinkImageTwice(float _)
    {
        Color color = image.color;
        color.a = targetAlpha;
        image.color = color;

        Sequence seq = DOTween.Sequence();
        seq.Append(image.DOFade(0f, fadeTime));
    }

}
