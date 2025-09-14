using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class KeyParticle : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 키 스프라이트를 파티클에 입히고 즉시 플레이
    /// </summary>
    public void PlayWithSprite(Sprite sprite)
    {
        var tsa = ps.textureSheetAnimation;
        tsa.enabled = true;
        tsa.mode = ParticleSystemAnimationMode.Sprites;

        // 프리팹에 미리 1개 이상 넣어뒀다고 가정하고 0번을 교체
        if (tsa.spriteCount == 0)
        {
            tsa.AddSprite(sprite);
        }
        else
        {
            tsa.SetSprite(0, sprite);
        }

        ps.Play(true);
    }
}
