using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ShieldSkill class
/// 부채꼴 모양의 방어막으로 원거리 공격만 막을 수 있음.
/// 처음 사용 시 마우스 방향으로 쉴드를 생성, 스킬 키를 shieldDirectionInputTime 초간 누르고 있으면 마우스 방향으로 쉴드 방향 변경 가능
/// </summary>

public class ShieldSkill : MonoBehaviour, ISkill
{
    public ShieldSkillData data;
    public SkillData skilldata => data;

    [Header("Shield Visual")]
    [SerializeField] private GameObject shieldPrefab; // 실드 시각적 프리팹
    [SerializeField] private LayerMask projectileLayer = -1; // 막을 투사체 레이어

    private StackSystem stackSystem;
    private GameObject currentShield; // 현재 활성화된 실드
    private Coroutine shieldCoroutine; // 실드 실행 코루틴
    private Coroutine stackDrainCoroutine; // 스택 소모 코루틴
    private bool isShieldActive = false;
    private float lastCooldownTime = 0f;

    public bool CanExecute =>
        !isShieldActive &&
        stackSystem.GetCurrentStackValue() >= data.stackGaugeCost &&
        Time.time >= lastCooldownTime + data.cooldown;

    public void ExecuteSkill()
    {
        if (currentShield != null && isShieldActive)
        {
            // 이미 활성화된 실드가 있다면 종료

            return;
        }

        if (!CanExecute) return;

        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        shieldCoroutine = StartCoroutine(PerformShield());
    }

    private IEnumerator PerformShield()
    {
        isShieldActive = true;
        lastCooldownTime = Time.time;

        // 초기 스택 게이지 소모
        stackSystem.RemoveStackGauge(data.stackGaugeCost);

        // 마우스 방향 계산
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Player.Instance.currentMousePosition);
        mouseWorldPos.z = 0f;
        Vector3 shieldDirection = (mouseWorldPos - transform.position).normalized;

        // 실드 생성
        CreateShield(shieldDirection);

        // 방향 변경 가능 시간 동안 입력 감지
        float directionInputStartTime = Time.time;
        bool canChangeDirection = true;

        // 스택 게이지 지속 소모 시작
        if (stackDrainCoroutine != null)
            StopCoroutine(stackDrainCoroutine);
        stackDrainCoroutine = StartCoroutine(DrainStackGauge());

        while (isShieldActive && stackSystem.GetCurrentStackValue() > 0)
        {
            // 방향 변경 입력 시간 확인
            if (canChangeDirection && Time.time - directionInputStartTime >= data.shieldDirectionInputTime)
            {
                canChangeDirection = false;
            }

            // 스킬 키가 눌려있고 방향 변경 가능한 시간 내라면 방향 업데이트
            if (canChangeDirection && IsSkillKeyPressed())
            {
                Vector3 newMouseWorldPos = Camera.main.ScreenToWorldPoint(Player.Instance.currentMousePosition);
                newMouseWorldPos.z = 0f;
                Vector3 newDirection = (newMouseWorldPos - transform.position).normalized;
                UpdateShieldDirection(newDirection);
            }
            // 스킬 키를 떼면 종료
            else if (!IsSkillKeyPressed())
            {
                break;
            }

            yield return null;
        }

        // 실드 종료
        DestroyShield();

    }

    private IEnumerator DrainStackGauge()
    {
        while (isShieldActive && stackSystem.GetCurrentStackValue() > 0)
        {
            yield return new WaitForSeconds(1f);

            if (isShieldActive && stackSystem.GetCurrentStackValue() >= data.stackGaugeUsagePerSecond)
            {
                stackSystem.RemoveStackGauge(data.stackGaugeUsagePerSecond);
            }
            else
            {
                DestroyShield();
                break;
            }
        }
    }

    private bool IsSkillKeyPressed()
    {
        // 현재 스킬이 할당된 키 입력 확인
        // PlayerSkillController에서 어떤 키가 이 스킬에 할당되었는지 확인해야 함
        // 임시로 Skill0 키 사용
        return Player.Instance.playerInput.actions["Skill0"].IsPressed();
    }

    private void CreateShield(Vector3 direction)
    {
        if (shieldPrefab != null)
        {
            Vector3 shieldPosition = transform.position + direction * 0.5f; // 플레이어 앞쪽에 생성
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            currentShield = Instantiate(shieldPrefab, shieldPosition, Quaternion.AngleAxis(angle, Vector3.forward));
            currentShield.transform.SetParent(transform); // 플레이어를 따라다니도록

            // 실드 콜라이더 설정
            SetupShieldCollider();
        }
    }

    private void UpdateShieldDirection(Vector3 direction)
    {
        if (currentShield != null)
        {
            Vector3 shieldPosition = transform.position + direction * 0.5f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            currentShield.transform.position = shieldPosition;
            currentShield.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void SetupShieldCollider()
    {
        if (currentShield == null) return;

        // 실드에 트리거 콜라이더 추가 (투사체 차단용)
        var shieldCollider = currentShield.GetComponent<Collider2D>();
        if (shieldCollider == null)
        {
            // 부채꼴 모양에 맞는 콜라이더 생성
            var circleCollider = currentShield.AddComponent<CircleCollider2D>();
            circleCollider.radius = data.radius;
        }

        shieldCollider.isTrigger = true;

        // TODO: 투사체 차단 로직은 별도 컴포넌트에서 처리
        // 현재는 시각적 효과만 제공
    }

    private void DestroyShield()
    {
        if (isShieldActive)
        {
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
                shieldCoroutine = null;
            }

            if (stackDrainCoroutine != null)
            {
                StopCoroutine(stackDrainCoroutine);
                stackDrainCoroutine = null;
            }

            if (currentShield != null)
            {
                Destroy(currentShield);
                currentShield = null;
            }

            isShieldActive = false;
        }
    }

    public void InitializeSkill()
    {
        stackSystem = Player.Instance.stackSystem;
        if (stackSystem == null)
        {
            Debug.LogError("StackSystem is not found. ShieldSkill cannot be initialized.");
            return;
        }
    }

    public float GetNormalizedRemainingCooldown()
    {
        if (!CanExecute && Time.time < lastCooldownTime + data.cooldown)
        {
            return (Time.time - lastCooldownTime) / data.cooldown;
        }
        return 0f; // 쿨다운 없음
    }

    public void ReleaseSkill()
    {
        DestroyShield();
    }

    private void OnDestroy()
    {
        ReleaseSkill();
    }
}
