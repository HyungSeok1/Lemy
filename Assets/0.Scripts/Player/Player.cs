using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어의 입력, 애니메이션 등 관리
/// 
/// 
/// </summary>
public class Player : PersistentSingleton<Player>, ISaveable<PositionData>, ICutsceneObject, ICameraFollowable
{
    [Tooltip("죽고 나서 줌했을때의, 시네머신 줌 사이즈")]
    [SerializeField] private float zoomedOrthographicSize;
    [SerializeField] private float zoomedOutOrthographicSize;
    [SerializeField] private float zoomTime; // 죽자마자 줌되는동안 걸리는 시간
    [SerializeField] private float dieWaitTime; // 죽음->카메라 줌 이후 DieAnim2 전까지 걸리는 시간
    [SerializeField] private float waitTimeForFadein; //Die2Anim 발동 후 페이드아웃이 발동될때까지 기다리는 시간
    [SerializeField] private float zoomOutTime;


    [SerializeField] private GameObject playerAvatar;


    [HideInInspector] public Vector2 currentMousePosition;

    private bool isSceneloaded = false;
    private SpriteRenderer avatarSpriteRenderer;

    Vector3 _spawnPos; // 스폰 시 위치

    [HideInInspector] public bool isInvincible = false;

    /// <summary>
    /// 컴포넌트들 직접 끌어다 할당 필요
    /// </summary>
    public PlayerSkillController playerSkillController;
    public StackSystem stackSystem;
    public PlayerHealth health;
    public PlayerInput playerInput;
    public Rigidbody2D rb;
    public Animator animator;
    new public Collider2D collider;
    public EnvFieldReceiver envFieldReceiver;
    public PlayerStats stats;
    public PlayerMovement movement;
    public PlayerInventory inventory;
    public PlayerInputController playerInputController;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // SlashSkill에서 움직임 상태가 바뀌면 스킬 멈춰야해서 만든 프로퍼티 - 박재용
    public bool IsMoving => movement.isMoving;

    protected override void Awake()
    {
        base.Awake();
        _spawnPos = transform.position;       // 씬 로드 직후 좌표 저장
    }

    void Start()
    {
        playerInputController.EnablePlayerActionMap();
        CutsceneManager.Instance.OnTimelineChanged += BindCutsceneTrackReference;
        RegisterToSwitchTarget();
    }

    private void OnDisable()
    {
        if (CutsceneManager.Instance != null)
            CutsceneManager.Instance.OnTimelineChanged -= BindCutsceneTrackReference;
        UnRegisterToSwitchTarget();
    }

    void Update()
    {
        currentMousePosition = playerInput.actions["Pointer"].ReadValue<Vector2>();

        // 스택시스템 각도 계산 갱신 
        stackSystem.UpdateSpinCharge();

        if (Keyboard.current.spaceKey.wasPressedThisFrame) //급정지
        {
            movement.TryBrake();
        }
    }


    public float minRadius;
    /// <summary>
    /// 마우스 위치 갱신 - 커서와 플레이어가 일정한 최소 거리를 유지해서, 플레이어 주변에 보이지 않는 원(안전 거리) 안쪽으로는 들어가지 못하도록 함.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetProcessedPointerPosition()
    {
        Vector2 raw = playerInput.actions["Pointer"].ReadValue<Vector2>();
        Vector2 playerPos = Camera.main.WorldToScreenPoint(Player.Instance.transform.position);
        Vector2 dir = raw - playerPos;
        float sqrDir = dir.sqrMagnitude;

        if (sqrDir < minRadius * minRadius)
            raw = playerPos + dir.normalized * minRadius;

        return raw; // 가공된 값 반환
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            movement.isLeftHeld = true;
            movement.StartMovement(); // 마우스 클릭 시작 시 이동 시작
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            movement.isLeftHeld = false;
        }
    }

    public void DieEffect()
    {
        transform.rotation = quaternion.identity;
        rb.linearVelocity = Vector2.zero;
        movement.speed = 0;
        movement.dir = Vector2.right;

        SoundManager.Instance.PlaySFX("playerDie", 0.1f);

        StartCoroutine(DieEffectAndRespawn());
    }

    // 사망 연출
    private IEnumerator DieEffectAndRespawn()
    {
        yield return null;

        // 조작 끄기
        playerInputController.TurnOnGlobalOnly();

        var follow = MainCameraScript.Instance.GetActiveCinemachineCam().GetCinemachineComponent(0) as CinemachineFollow;
        var originDamping = follow.TrackerSettings.PositionDamping;
        follow.TrackerSettings.PositionDamping = new Vector3(0, 0, 0);

        spriteRenderer.enabled = false;
        collider.enabled = false;

        // 카메라 바꿔치기
        MainCameraScript.Instance.overlayCamera.gameObject.SetActive(true);
        Camera overlayCam = MainCameraScript.Instance.overlayCamera;
        overlayCam.transform.position = Camera.main.transform.position;


        // 아바타 위치를 플레이어와 똑같이 설정
        playerAvatar.transform.position = transform.position;
        playerAvatar.transform.rotation = Quaternion.identity;

        Animator avatarAnimator = playerAvatar.GetComponent<Animator>();
        avatarSpriteRenderer = playerAvatar.GetComponent<SpriteRenderer>();
        avatarSpriteRenderer.enabled = true;
        // 1. 첫 번째 애니메이션 실행 & FadeOut
        avatarAnimator.SetTrigger("Die1Trigger");

        // 씬 바꿈 + 페이드아웃
        StartCoroutine(LoadSceneWithFadeOut(() => isSceneloaded = true));
        UICanvasManager.Instance.FadeOutUI(); // UI 페이드아웃

        // 2. 카메라 급격히 확대 (overlay도 같이 확대)
        var liveCam = MainCameraScript.Instance.GetActiveCinemachineCam();
        float startorthographicSize = liveCam.Lens.OrthographicSize;
        float elapsedTime = 0f;
        var lens = liveCam.Lens;
        while (true)
        {
            if (elapsedTime > zoomTime) break; // 시간 초과시 루프 종료

            // overlay, Base 캠 둘다 동시에 같이 변경
            overlayCam.orthographicSize = lens.OrthographicSize = Mathf.Lerp(startorthographicSize, zoomedOrthographicSize, elapsedTime / zoomTime);  // 원하는 값으로 변경

            liveCam.Lens = lens; // 반드시 구조체 통째로 다시 대입!
            elapsedTime += Time.deltaTime;


            yield return null; // 다음 프레임까지 대기
        }
        lens.OrthographicSize = zoomedOrthographicSize;
        overlayCam.orthographicSize = zoomedOrthographicSize;
        liveCam.Lens = lens;

        // 3. dieWaitTime초 대기
        yield return new WaitForSeconds(dieWaitTime);

        // 씬 전환 안됐으면 대기
        yield return new WaitUntil(() => isSceneloaded);

        isSceneloaded = false; // 사용 후 플래그 변수 원상복구
        playerInputController.TurnOnGlobalOnly();// 임시방편 막기


        // 4. 두 번째 애니메이션 실행 후 대기
        avatarAnimator.SetTrigger("Die2Trigger");

        yield return new WaitForSeconds(waitTimeForFadein);

        // 페이드인 시작하는 동시에, 투명해지면서 "이미 존재하는" 플레이어 보이기 시작.
        spriteRenderer.enabled = true;
        collider.enabled = true;



        // 카메라 사이즈 복구 (작아져있음)
        var liveCam2 = MainCameraScript.Instance.mainCinemachineCamera;
        float startorthographicSize2 = liveCam2.Lens.OrthographicSize;
        elapsedTime = 0f;
        var lens2 = liveCam2.Lens;
        lens2.OrthographicSize = zoomedOutOrthographicSize;
        liveCam2.Lens = lens2;
        ;
        yield return StartCoroutine(SceneTransitionManager.Instance.FadeInCoroutine());
        UICanvasManager.Instance.FadeInUI(); // UI 페이드인
        playerInputController.EnablePlayerActionMap(); // 조작 복구

        //while (true)
        //{
        //    if (elapsedTime > zoomOutTime) break; // 시간 초과시 루프 종료
        //    lens2.OrthographicSize = Mathf.Lerp(startorthographicSize2, zoomedOutOrthographicSize, elapsedTime / zoomOutTime);  // 원하는 값으로 변경
        //    liveCam2.Lens = lens2; // 반드시 구조체 통째로 다시 대입!
        //    elapsedTime += Time.deltaTime;

        //    yield return null; // 다음 프레임까지 대기
        //}

        // 그냥 한번에 변경. 기존에는 서서히 축소되도록 했었음.

        MainCameraScript.Instance.overlayCamera.gameObject.SetActive(false);
        follow.TrackerSettings.PositionDamping = originDamping;
    }


    [SerializeField] private float stunSpeedThreshold;
    private float pow;
    [SerializeField] private float slowDownDuration;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {

        }

        if (collision.gameObject.CompareTag("Destructible"))
        {
            rb.linearVelocity = movement.lastVelocity;
        }


        if (collision.gameObject.layer != LayerMask.NameToLayer("Wall")) return; // 벽이 아니면 리턴
        if (movement.lastVelocity.magnitude < stunSpeedThreshold) return; //속도 기준치 안넘으면 아무것도 안함

        // 벽 충돌 체크 부분
        /*
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal; // 충돌 표면의 법선
        Vector2 incident = lastVelocity.normalized; // 현재 이동 방향
        Vector2 reflected = Vector2.Reflect(incident, normal);
        */

        if (movement.isMoving) // 법선 벡터와 속도 고려해야함.
        {

            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal; // 충돌 표면의 법선
            Vector2 incident = movement.lastVelocity.normalized; // 현재 이동 방향
            float cosAbs = Mathf.Abs(Vector2.Dot(normal, incident)); //법선과 이동방향 코사인 절댓값

            // 테스트용
            //Debug.LogError($"{cosAbs * speed}");

            if (cosAbs * movement.speed > stunSpeedThreshold)
            {
                StartCoroutine(WallBonk());
            }
        }
    }


    // Skill and Visual Effect Prefabs
    [SerializeField] private GameObject stunEffect;
    public GameObject DashEffect;

    IEnumerator WallBonk() // 벽 충돌 반응 (플레이어 animator 관련)
    {
        // Bonk 판정
        movement.isMoving = false;
        stunEffect.SetActive(true);
        animator.SetTrigger("Bonk");
        animator.SetBool("isBonk", true);
        movement.speed = 0;
        rb.linearVelocity = Vector2.zero;

        pow = 1f; // 밀려날 정도

        float elapsed = 0f;
        while (elapsed < slowDownDuration) // Bonk 상태 도중
        {
            if (playerInput.actions["Left Click"].WasPressedThisFrame() ||
                playerInput.actions["Right Click"].WasPressedThisFrame()) // 이동 시도할 경우 코루틴 바로 풀어줌
            {
                stunEffect.SetActive(false);
                animator.SetBool("isBonk", false);
                movement.StartMovement();

                yield break;
            }
            elapsed += Time.deltaTime;

            transform.position -= transform.right * pow * Time.deltaTime; // 뒤로 밀려나는 코드

            yield return null;
        }

        // Bonk 해제
        stunEffect.SetActive(false);
        animator.SetBool("isBonk", false);
        movement.speed = 0f;
        movement.isMoving = true;
    }


    IEnumerator LoadSceneWithFadeOut(Action action)
    {
        yield return StartCoroutine(SceneTransitionManager.Instance.FadeOutCoroutine());


        if (SceneTransitionManager.Instance != null)
        {
            GameSaveData data = SaveLoadManager.Instance.GetCurrentData();
            yield return SceneTransitionManager.Instance.TransitionCoroutine(data.playerData.stateData, data.playerData.positionData);

            // 씬 바뀐 직후에 OrthographicSize 바꿔야함. (씬 바뀔시 축소된 OrthographicSize 적용안댐)
            CinemachineCamera liveCamA = MainCameraScript.Instance.mainCinemachineCamera;
            var lensA = MainCameraScript.Instance.mainCinemachineCamera.Lens;
            lensA.OrthographicSize = zoomedOrthographicSize;
            liveCamA.Lens = lensA;
            // 씬 바꾸기 끝
            liveCamA.transform.position = transform.position;
        }
        else
            Debug.Log("SceneTransitionManager.Instance is null");

        action?.Invoke();
    }

    #region SaveLoad
    public void Save(ref PositionData data)
    {
        data.pos = transform.position;
    }

    public void Load(PositionData data)
    {
        transform.position = data.pos;
    }
    #endregion

    public void BindCutsceneTrackReference()
    {
        CutsceneManager.Instance.BindByTrackName(gameObject, "Player");
    }

    public void RegisterToSwitchTarget()
    {
        MainCameraScript.Instance.RegisterSwitchTarget(this.gameObject);
    }

    public void UnRegisterToSwitchTarget()
    {
        if (MainCameraScript.Instance != null)
            MainCameraScript.Instance.RegisterSwitchTarget(this.gameObject);
    }
}

