using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 삼각형 한 기기(경고등 + 세그먼트들)
public class FlameGenerator : MonoBehaviour
{

    [SerializeField] private GameObject FlameActivated;
    [SerializeField] private Animator FlameWarner;
    Animator animator;

    [Header("Indicator (경고등)")]
    [SerializeField] private SpriteRenderer indicator; // 중앙 원형 문양
    [SerializeField] private Color indicatorIdle = Color.white;
    [SerializeField] private Color indicatorActive = new Color(1f, 0.1f, 0.1f, 1f);

    [Header("세그먼트(변)들 List")]
    [SerializeField] private List<FlameLaneSegment> segments = new List<FlameLaneSegment>();

    [Header("타이밍")]
    [SerializeField] private float warnDuration = 2.0f;  // 분홍 경고 유지
    [SerializeField] private float activeDuration = 5.0f;  // 빨강 화염 유지

    [SerializeField] private float Damage = 20f; // 데미지 양
    private bool isFlaming = false;


    void Start()
    {
        if (indicator) indicator.color = indicatorIdle;
        foreach (var s in segments) s.RemoveActive();
        foreach (var s in segments) s.damage = Damage; 
        isFlaming = false;
    }

    void Update()
    {
        if (!isFlaming)
        {
            Flame();
            isFlaming = true; // 한 번만 시작
        }
    }

    public void Flame()
    {
        StartCoroutine(MakeFlame());
    }

    IEnumerator MakeFlame()
    {
        // 1) 경고등 빨강
        if (indicator) indicator.color = indicatorActive;
        // 2) 분홍 경고(2초) – 길은 지나갈 수 있고 색만 분홍
        foreach (var s in segments) s.AddWarning();
        yield return new WaitForSeconds(warnDuration);
        foreach (var s in segments) s.RemoveWarning();

        // 3) 화염 활성(1초) – 길이 빨강 + 데미지 ON
        foreach (var s in segments) s.AddActive();
        FlameActivated.SetActive(true);
        FlameWarner.speed = 1.5f;

        yield return new WaitForSeconds(activeDuration);
        foreach (var s in segments) s.RemoveActive();
        FlameActivated.SetActive(false);
        FlameWarner.speed = 1f;

        // 꺼짐 & 약간의 텀
        if (indicator) indicator.color = indicatorIdle;
        yield return new WaitForSeconds(1f);
        isFlaming = false;
    }
}
