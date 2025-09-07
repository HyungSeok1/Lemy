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

public class HealthBarShrink : MonoBehaviour
{
    [SerializeField] private float shrinkDelay = 1f;


    [SerializeField] private Image barImage;
    [SerializeField] private Image shrinkBarImage;

    [SerializeField] private float shrinkSpeed = 1f;

    private Player player;
    private PlayerHealth playerHealth;

    private float shrinkTimer;
    private bool isShrinking = false;
    private Coroutine shrinkCoroutine;    // ← 코루틴 레퍼런스 보관

    private void Start()
    {
        player = Player.Instance;
        playerHealth = player.gameObject.GetComponent<PlayerHealth>();
        barImage.fillAmount = playerHealth.CurrentHealth;
        shrinkBarImage.fillAmount = playerHealth.CurrentHealth;
    }


    /// <summary>
    /// 체력 감소한다고 따로 이벤트 쏴줄 필요 없고, 매 프레임마다 update()에서 감지합니다.
    /// 
    /// </summary>
    private void Update()
    {
        // 1) 값 갱신
        float curr = playerHealth.CurrentHealth / playerHealth.maxHealth;
        barImage.fillAmount = curr;

        // 2) 감소 트리거
        if (curr < shrinkBarImage.fillAmount && !isShrinking)
        {
            shrinkTimer = shrinkDelay;
            shrinkCoroutine = StartCoroutine(DoShrink());  // ← 레퍼런스 저장
        }
        else if (curr > shrinkBarImage.fillAmount)
        {
            // 즉시 따라붙을 때는 코루틴 종료
            shrinkBarImage.fillAmount = curr;
            if (isShrinking)
            {
                StopCoroutine(shrinkCoroutine);
                isShrinking = false;
            }
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

    public void StackSystem_Decresed()
    {
        if (!isShrinking)
        {
            shrinkTimer = shrinkDelay;
            shrinkCoroutine = StartCoroutine(DoShrink());
        }
    }


}
