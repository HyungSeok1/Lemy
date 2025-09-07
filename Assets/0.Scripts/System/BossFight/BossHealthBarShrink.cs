using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// PlayerHealth의 Current/Max 값에 따라 메인 바를 즉시 업데이트하고,
///  
/// 일정 시간 후 감쇠(shrink) 효과를 적용하는 뷰 컴포넌트입니다.
/// 
/// 데이터 저장 및 처리는 StackSystem이 담당합니다.
/// 
/// StackBarShrink와 거의 동일한 구조를 가지고 있습니다.
/// 
/// </summary>

public class BossHealthBarShrink : MonoBehaviour
{
    [SerializeField] private float shrinkDelay = 1f;
    [SerializeField] private Image shrinkBarImage;
    [SerializeField] private Image barImage;
    [SerializeField] private float shrinkSpeed = 1f;

    private BossBase boss => BossFightSystem.Instance.boss;

    private float shrinkTimer;
    private bool isShrinking = false;
    private Coroutine shrinkCoroutine;    // 코루틴 레퍼런스 보관 => 만약 보스 체력이 다시 차오를 수 있다면 로직 필요. 지금은 안씀




    public void InitBar(BossBase bossBaseRef)
    {
        barImage.fillAmount = boss.Health;
        shrinkBarImage.fillAmount = boss.Health;

        boss.OnHealthChanged += HealthDecreased;
    }


    public void HealthDecreased()
    {
        // 1) 값 갱신
        float curr = (float)boss.Health / (float)boss.MaxHealth;
        barImage.fillAmount = curr;

        if (curr < shrinkBarImage.fillAmount && !isShrinking)
        {
            shrinkTimer = shrinkDelay;
            shrinkCoroutine = StartCoroutine(DoShrink());
        }
    }

    private IEnumerator DoShrink()
    {
        isShrinking = true;

        // 타이머 ↓
        while (shrinkTimer > 0f)
        {
            shrinkTimer -= Time.deltaTime;
            yield return null;
        }

        // 실제 감쇠 루프
        while (shrinkBarImage.fillAmount > barImage.fillAmount)
        {
            // delta 만큼 줄여주고
            shrinkBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            // 혹시 바 이하로 내려가면 clamp
            if (shrinkBarImage.fillAmount < barImage.fillAmount)
                shrinkBarImage.fillAmount = barImage.fillAmount;

            yield return null;
        }

        isShrinking = false;
    }

}
