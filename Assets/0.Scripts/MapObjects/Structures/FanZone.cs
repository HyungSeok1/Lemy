using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class FanZone : MonoBehaviour, ISwitchable
{
    [Header("바람 설정")]
    public Vector2 windDirection = Vector2.right;   // 월드 기준 바람 방향
    public float maxEnvSpeed = 28f;                // 최대 환경 속도
    public float minEnvSpeed = 2f;                // 최소 환경 속도 
    public AnimationCurve strengthByDepth =         // 0(출구 근처)=강, 1(끝)=약
        AnimationCurve.EaseInOut(0, 1, 1, 0.2f);
    public bool depthAlongLocalX = true;            // 로컬 X로 멀어질수록 약해짐

    private BoxCollider2D box;
    private SpriteRenderer sprite;
    private Vector2 dirN;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color offColor = new Color(0.2f, 0.8f, 0.2f);

    [SerializeField] private bool isOn = true;
    public bool IsOn => isOn;

    [SerializeField] private Vector2 windDirectionLocal = Vector2.right;

    [Header("대상 필터")]
    [Tooltip("이 레이어들에 속한 오브젝트만 영향을 받음 (Player, Enemy 등)")]
    [SerializeField] private LayerMask affectedLayers = ~0;
    private readonly HashSet<EnvFieldReceiver> insiders = new();

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        box.isTrigger = true;
    }        

    public void SetOn(bool on)
    {
        // ISwitchable 인터페이스 구현
        isOn = on;
        if (!isOn)
        {
            foreach (var p in insiders)
                p?.RemoveContribution(this);
            sprite.color = offColor;
        }
        else
        {
            sprite.color = normalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerOK(other.gameObject.layer)) return;
        // Player, Enemy 등 상관없이 Receiver만 찾는다
        var r = other.GetComponentInParent<EnvFieldReceiver>();
        if (r != null) insiders.Add(r);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isOn) return;
        if (!LayerOK(other.gameObject.layer)) return;

        var r = other.GetComponentInParent<EnvFieldReceiver>();
        if (r == null) return;

        Vector2 dirN = ((Vector2)transform.TransformDirection(windDirectionLocal)).normalized;

        // 로컬 좌표로 깊이 0..1 계산
        Vector2 local = transform.InverseTransformPoint(other.transform.position);
        Vector2 half = box.size * 0.5f;
        float depth = depthAlongLocalX
            ? Mathf.InverseLerp(-half.x, +half.x, local.x) // 왼(0)→오(1)
            : Mathf.InverseLerp(-half.y, +half.y, local.y); // 아래(0)→위(1)

        // 세기 곡선 + 최소 속도 보정
        float coef = Mathf.Clamp01(strengthByDepth.Evaluate(depth));
        float speed = maxEnvSpeed * coef + minEnvSpeed;

        Vector2 envVel = dirN * speed;

        // 수신자에 환경 속도 기여
        r.SetContribution(this, envVel);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!LayerOK(other.gameObject.layer)) return;

        var r = other.GetComponentInParent<EnvFieldReceiver>();
        if (r == null) return;

        r.RemoveContribution(this);
        insiders.Remove(r);
    }
    private bool LayerOK(int layer) => ((1 << layer) & affectedLayers) != 0;
}
