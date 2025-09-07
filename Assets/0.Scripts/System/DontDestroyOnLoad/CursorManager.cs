using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : PersistentSingleton<CursorManager>
{
    [Header("Art (택1: Texture 권장)")]
    [SerializeField] private Texture2D normalTexture; // ← 여기에 Texture2D 할당

    [Header("Size")]
    [SerializeField, Range(0.1f, 5f)] private float baseScale = 0.3f; // ← 기본 크기 비율(기본 50%)


    [Header("기존 Canvas 재사용")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private int subCanvasSortingOrder = 32760;

    [Header("Default (idle)")]
    [SerializeField] private float defaultRotateSpeed = 60f;

    [Header("Click Hold Pattern")]
    [SerializeField] private float clickCycle = 1.0f;
    [SerializeField] private float clickRotationsPerCycle = 1f;
    [SerializeField] private float clickScaleA = 1.12f;
    [SerializeField] private float clickScaleB = 0.88f;
    [SerializeField] private float clickScaleC = 1.00f;

    private Vector2 hotspot = new(0.5f, 0.5f);

    [Header("Dash Spin Impulse")]
    [Tooltip("대시 액션 직후 회전 속도 배수(즉시). 예: 4 => 4배에서 시작")]
    [SerializeField] private float dashStartMultiplier = 4f;
    [Tooltip("감쇠 시간(초). 3초 동안 서서히 1배로 복귀")]
    [SerializeField] private float dashSpinDuration = 3f;

    private enum State { Default, ClickHold }
    private State state = State.Default;

    private Canvas canvas;
    private RectTransform rt;

    // Image/RawImage 둘 다 지원
    private Graphic graphic;
    private Image image;
    private RawImage rawImage;

    private float cycleTimer;
    private float currentRotation;
    private bool dashForced;

    private PlayerMovement pm;
    private float dashSpinTimer = -1f; // -1: 대시 중이 아님
    private float spinMult = 1f;

    protected override void Awake()
    {
        base.Awake();
        SetupUI();
        ApplyVisual();
        Cursor.visible = false;
        TryBindPlayerMovement();
    }

    private void Update()
    {
        UpdateStateByInput();
        FollowMouse();
        Animate();
    }
    private void TryBindPlayerMovement()
    {
        var player = Player.Instance; // 프로젝트에서 이미 쓰는 싱글턴
        if (player == null) return;
        var cand = player.GetComponent<PlayerMovement>();
        if (cand == null) return;

        // 중복 구독 방지
        if (pm == cand) return;

        // 이전 것 해제
        if (pm != null) pm.OnSkillImpulse -= OnDashImpulse;

        pm = cand;
        pm.OnSkillImpulse += OnDashImpulse;
    }

    private void OnDashImpulse(Vector2 v)
    {
        Debug.Log($"CursorManager: Dash Impulse {v}");
        dashSpinTimer = 0f;
    }

    private float ConsumeSpinMultiplier(float dt)
    {
        if (dashSpinTimer < 0f) return 1f;

        dashSpinTimer += dt;
        float t = Mathf.Clamp01(dashSpinTimer / dashSpinDuration); // 0→1
        float k = 1f - t; // 남은 비율
        float mult = 1f + (dashStartMultiplier - 1f) * (k * k * k); // 부드러운 감쇠

        if (t >= 1f) dashSpinTimer = -1f; // 종료
        return mult;
    }


    private void SetupUI()
    {
        if (rootCanvas != null)
        {
            var subGO = new GameObject("CursorSubCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            subGO.transform.SetParent(rootCanvas.transform, false);
            var sub = subGO.GetComponent<Canvas>();
            sub.renderMode = rootCanvas.renderMode;
            sub.worldCamera = rootCanvas.worldCamera;
            sub.sortingLayerID = rootCanvas.sortingLayerID;
            sub.overrideSorting = true;
            sub.sortingOrder = subCanvasSortingOrder;

            var rootScaler = rootCanvas.GetComponent<CanvasScaler>();
            var subScaler = subGO.GetComponent<CanvasScaler>();
            if (rootScaler && subScaler)
            {
                subScaler.uiScaleMode = rootScaler.uiScaleMode;
                subScaler.referenceResolution = rootScaler.referenceResolution;
                subScaler.screenMatchMode = rootScaler.screenMatchMode;
                subScaler.matchWidthOrHeight = rootScaler.matchWidthOrHeight;
                subScaler.referencePixelsPerUnit = rootScaler.referencePixelsPerUnit;
                subScaler.scaleFactor = rootScaler.scaleFactor;
            }

            canvas = sub;
            CreateCursorGraphic(subGO.transform);
        }
        else
        {
            var canvasGO = new GameObject("CursorCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var c = canvasGO.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = subCanvasSortingOrder;
            DontDestroyOnLoad(canvasGO);

            canvas = c;
            CreateCursorGraphic(canvasGO.transform);
        }
    }

    private void CreateCursorGraphic(Transform parent)
    {
        // Texture가 있으면 RawImage, 없으면 Image 사용
        if (normalTexture != null)
        {
            var go = new GameObject("Cursor", typeof(RawImage));
            go.transform.SetParent(parent, false);
            rawImage = go.GetComponent<RawImage>();
            rawImage.raycastTarget = false;
            graphic = rawImage;
            rt = rawImage.rectTransform;
        }
        rt.pivot = hotspot;
    }

    private void ApplyVisual()
    {
        if (rawImage != null)
        {
            rawImage.texture = normalTexture;
            rawImage.color = Color.white;
            rawImage.SetNativeSize();
        }
    }

    private void UpdateStateByInput()
    {
        bool held = Mouse.current != null && Mouse.current.leftButton.isPressed;
        if (held && state != State.ClickHold) ChangeState(State.ClickHold);
        else if (!held && state != State.Default) ChangeState(State.Default);
    }

    private void ChangeState(State next)
    {
        state = next;
        cycleTimer = 0f;      // a 지점부터
        ApplyVisual();        // 동일 비주얼 유지
    }

    private void FollowMouse()
    {
        Vector2 screenPos = Player.Instance.currentMousePosition;

        rt.position = screenPos;

    }

    private void Animate()
    {
        float dt = Time.unscaledDeltaTime;
        spinMult = ConsumeSpinMultiplier(dt);
        switch (state)
        {
            case State.Default:
                cycleTimer = 0f;
                currentRotation += defaultRotateSpeed * spinMult * dt;
                rt.localRotation = Quaternion.Euler(0, 0, currentRotation);
                rt.localScale = Vector3.one * baseScale;
                break;

            case State.ClickHold:
                AnimateCycle(clickCycle, clickRotationsPerCycle, clickScaleA, clickScaleB, clickScaleC, dt);
                break;
        }
    }

    private void AnimateCycle(float period, float rotationsPerCycle, float a, float b, float c, float dt)
    {
        if (period <= 0f) period = 0.001f;
        cycleTimer += dt;
        float tn = (cycleTimer % period) / period;

        float scale = tn < 1f / 3f ? Mathf.Lerp(a, b, tn * 3f)
                   : tn < 2f / 3f ? Mathf.Lerp(b, c, (tn - 1f / 3f) * 3f)
                                : Mathf.Lerp(c, a, (tn - 2f / 3f) * 3f);

        rt.localScale = Vector3.one * scale * baseScale;
        currentRotation += (rotationsPerCycle * 360f / period) * spinMult * dt;
        rt.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}
