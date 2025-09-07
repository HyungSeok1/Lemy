using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ToggleSwitch : MonoBehaviour
{
    [Header("누르는 대상 필터")]
    [SerializeField] private LayerMask activatorLayers;

    [Header("토글 설정")]
    [SerializeField] private List<MonoBehaviour> targets = new(); // ISwitchable를 가진 컴포넌트들
    [SerializeField] private float lockout = 0.2f; // 연타 방지(초)

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color lockedColor = new Color(1f, 0.85f, 0.2f);
    // 내부
    private ISwitchable[] switchables;
    private Collider2D col;
    private SpriteRenderer sprite;
    private float lastToggleTime = -999f;
    private bool Pressed = false;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = normalColor;
        if (!col.isTrigger)
        {
            Debug.LogWarning($"{name}: 버튼 콜라이더는 isTrigger 권장입니다.");
        }

        var list = new List<ISwitchable>();
        foreach (var mb in targets)
        {
            if (mb == null) continue;
            if (mb is ISwitchable s) list.Add(s);
            else Debug.LogWarning($"{name}: 대상 {mb.name} 은 ISwitchable이 아님");
        }
        switchables = list.ToArray();
    }

    bool IsActivator(GameObject go)
    {
        return ((1 << go.layer) & activatorLayers) != 0;
    }

    void TryToggle()
    {
        if (Time.time - lastToggleTime < lockout) return;
        Pressed = !Pressed;
        lastToggleTime = Time.time;
        StartCoroutine(waitingLockout());

        foreach (var s in switchables) s.Toggle();
    }

    // 트리거로 받기
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActivator(other.gameObject)) TryToggle();
    }

    IEnumerator waitingLockout()
    {
        // 버튼 스프라이트 색깔 변경
        sprite.color = lockedColor;
        yield return new WaitForSeconds(lockout);
        if(Pressed)
        {
            sprite.color = pressedColor; // 눌림 상태 색상
        }
        else
        {
            sprite.color = normalColor; // 원래 색상
        }
        lastToggleTime = -999f; // 다시 토글 가능
    }
}
