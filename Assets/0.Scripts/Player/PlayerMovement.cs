using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    //이동 관련 변수들
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float accelerationRate;
    [SerializeField] private float decelerationRate;
    [SerializeField] public float brakeCooldown;

    public float lastBrakeTime = -Mathf.Infinity;

    private Rigidbody2D rb;
    private StackSystem stackSystem;
    private Animator animator;
    private Player player;

    [HideInInspector] public Vector2 lastVelocity;
    [HideInInspector] Vector2 currentVelocity;
    [HideInInspector] public bool isLeftHeld = false;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public Vector2 skillVelocity;
    [HideInInspector] public float ghostSkillSpeed;


    // 핵심 변수 두개
    [HideInInspector] public float speed;
    [HideInInspector] public Vector2 dir;
    //외부에 의한 속도
    public Vector2 EnvSum => env != null ? env.EnvSum : Vector2.zero;
    private EnvFieldReceiver env;

    public event Action<Vector2> OnSkillImpulse;



    private void Start()
    {
        rb = Player.Instance.rb;
        stackSystem = Player.Instance.stackSystem;
        animator = Player.Instance.animator;
        env = Player.Instance.envFieldReceiver;

        speed = 0f;
        isMoving = false;
        player = Player.Instance;
    }

    private void FixedUpdate()
    {
        lastVelocity = currentVelocity;

        // 기본조작 입력받아 값 적용 & 플레이어 회전
        if (isLeftHeld)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(player.currentMousePosition);
            Vector2 moveDir = (mouseWorld - (Vector2)transform.position).normalized;
            float targetAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

            dir = transform.right;
            if (speed < maxSpeed) // 속도 한계치 안 넘었을때만 가속 적용
            {
                speed = Mathf.MoveTowards(speed, maxSpeed, accelerationRate * Time.fixedDeltaTime);

                if (ghostSkillSpeed != 0f)
                    speed = ghostSkillSpeed;
            }
        }

        // 감속
        if (!isLeftHeld || speed >= maxSpeed)
            speed = Mathf.Lerp(speed, 0, Time.fixedDeltaTime * decelerationRate);

        else if (speed < 0.2f) // 속도가 거의 0에 가까워지면 이동 중이 아니라고 판단
        {
            speed = 0f;
            isMoving = false;
            animator.SetBool("isFlying", false); // 비행 애니메이션 중지
        }

        // 방향을 즉시 바꾸는 스킬을 사용한 경우 적용되는 코드 (ex. Dash)
        if (skillVelocity != Vector2.zero)
        {
            SetBodyVelocity(skillVelocity);
            float targetAngle = Mathf.Atan2(skillVelocity.normalized.y, skillVelocity.normalized.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

            dir = rb.linearVelocity.normalized;
            speed = GetBodyVelocity().magnitude;

            OnSkillImpulse?.Invoke(skillVelocity);
            skillVelocity = Vector2.zero;
        }
        else
            SetBodyVelocity(dir * speed);

        currentVelocity = rb.linearVelocity;
    }

    public void TryBrake() // 브레이크
    {
        if (Time.time - lastBrakeTime < brakeCooldown) return; // 쿨타임 체크
        lastBrakeTime = Time.time;
        speed = 0f;
        SetBodyVelocity(Vector2.zero);
        isMoving = false;
        SoundManager.Instance.PlaySFX("dashCancel", 0.05f);
        animator.SetBool("isFlying", false);
    }

    public void StartMovement() // 기본 속도와 함께 이동 시작
    {
        if (!isMoving)
        {
            isMoving = true;
            speed = baseSpeed; // 기본 속도로 시작
            SetBodyVelocity(transform.right * speed); // 현재 방향으로 속도 설정
            animator.SetBool("isFlying", true); // 비행 애니메이션 시작
        }
    }

    public void StopMovement()
    {
        speed = 0f;
        dir = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    private Vector2 GetBodyVelocity()
    {
        return rb.linearVelocity - EnvSum;
    }

    private float GetBodySpeed() => GetBodyVelocity().magnitude;

    private void SetBodyVelocity(Vector2 bodyVelocity)
    {
        rb.linearVelocity = bodyVelocity + EnvSum;
    }

}
