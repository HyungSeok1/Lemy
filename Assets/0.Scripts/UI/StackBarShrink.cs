using System.Collections;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// StackSystem에 연결된, View를 담당하는 스크립트입니다.
/// 
/// 데이터 저장 및 처리는 StackSystem이 담당합니다.
/// 
/// barImage는 데이터에 연결되어 즉시 반응하며, shrinkBarImage는 fillAmount 값이 서서히 줄어들며 지연된 효과를 줍니다.
/// 
/// 
/// </summary>
public class StackBarShrink : MonoBehaviour
{
    [SerializeField] private float shrinkDelay = 1f;


    [SerializeField] private Image barImage;
    [SerializeField] private Image shrinkBarImage;

    [SerializeField] private float shrinkSpeed = 1f;

    private float shrinkTimer;
    private bool isShrinking = false;
    private Coroutine shrinkCoroutine;    // ← 코루틴 레퍼런스 보관
    private StackSystem stackSystem;

    private void Awake()
    {
        barImage.fillAmount = 0;
        shrinkBarImage.fillAmount = 0;
    }


    private void Start()
    {
        stackSystem = Player.Instance.gameObject.GetComponent<StackSystem>();
        stackSystem.OnRemoveStack += StackSystem_Decresed;
    }

    private void OnDisable()
    {
        stackSystem.OnRemoveStack -= StackSystem_Decresed;
    }


    private void Update()
    {
        // 1) 값 갱신
        float curr = stackSystem.GetNormalizedCurrentStack();
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
