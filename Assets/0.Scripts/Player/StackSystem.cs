using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Stack의 실질적인 데이터 저장 및 로직 처리를 담당하는 클래스입니다.
/// 
/// 
/// </summary>
public class StackSystem : MonoBehaviour
{
    [Header("거리 기반 스택 설정")]
    [Tooltip("스택 1개가 차는 데 필요한 값 (25)")]
    [SerializeField] public float valuePerStack = 25f;
    [SerializeField] public float maxStacksValue = 100f;
    [SerializeField] public int maxStacks = 4;
    [Tooltip("정지 후 스택이 사라지기까지의 시간")]
    [SerializeField] public float stackDuration = 2f;

    [Tooltip("거리 누적 속도 계수")]
    [SerializeField] public float distanceCoef = 1f;

    // 현재 스택 게이지
    public float currentValueTotal = 0f;

    public float frameDeltaDistance = 0f;
    public int CurrentStackCount => (int)Mathf.Min(currentValueTotal / valuePerStack, maxStacks);

    private Coroutine timerCoroutine;
    private Vector3 lastFramePosition;

    private void Start()
    {
        lastFramePosition = Player.Instance.transform.position;
    }

    public void UpdateSpinCharge()
    {
        Vector3 currentPos = Player.Instance.transform.position;

        // 이동 거리 계산
        float deltaDist = Vector2.Distance(currentPos, lastFramePosition);
        lastFramePosition = currentPos;

        frameDeltaDistance = deltaDist * distanceCoef;
        currentValueTotal = Mathf.Clamp(currentValueTotal + frameDeltaDistance, 0, maxStacksValue);
    }

    // 이동 안 하고 있으면 돌아가는 스택 지우기 타이머 
    public void StackTimer()
    {
        if (frameDeltaDistance > Mathf.Epsilon && timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null; //isRightClickHeld true되어 타이머 중단
        }

        if (frameDeltaDistance <= Mathf.Epsilon && timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(RemoveTimer());
        }
    }

    private IEnumerator RemoveTimer()
    {
        yield return new WaitForSeconds(stackDuration);
        RemoveStackGauge(25);
        timerCoroutine = null;
    }

    /// <summary>
    /// amount만큼의 스택 게이지를 소모하는 메서드
    /// (스택의 최대치는 100)
    /// </summary>
    public void RemoveStackGauge(float amount)
    {
        currentValueTotal = Mathf.Max(0, currentValueTotal - amount);
        OnRemoveStack?.Invoke();
    }


    public Action OnRemoveStack;

    /// <summary>
    /// 현재 스택의 개수를 정수화하여 반환하는 메서드
    /// 0 ~ 4 스택
    /// </summary>
    /// <returns></returns>
    public float GetCurrentStackValue()
    {
        return currentValueTotal;
    }

    /// <summary>
    /// 현재 스택의 개수를 정수화하여 반환하는 메서드
    /// 0 ~ 4 스택
    /// </summary>
    /// <returns></returns>
    public int GetCurrentStackCount()
    {
        return CurrentStackCount;
    }


    public float GetNormalizedCurrentStack()
    {
        return currentValueTotal / maxStacksValue;
    }
}
