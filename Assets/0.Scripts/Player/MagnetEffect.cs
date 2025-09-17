using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 자석 효과를 구현하는 스크립트
/// </summary>
public class MagnetEffect : MonoBehaviour
{
    public float moveSpeed = 10.0f; // 아이템이 끌려오는 속도
    public float detectionRadius = 5.0f; // 감지 반경
    public float accelerationRate = 5.0f; // 초당 속도 증가량
    public float maxSpeed = 0.0f; // 0 이면 무제한, 아니면 최대 속도 제한
    private List<Transform> attractedItems = new List<Transform>();
    private Dictionary<Transform, float> currentSpeeds = new Dictionary<Transform, float>();

    private void Start()
    {
        CircleCollider2D circleCollider = gameObject.GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = detectionRadius;
    }

    private void Update()
    {
        // 리스트에 있는 모든 아이템을 캐릭터 위치로 이동시킴
        for (int i = attractedItems.Count - 1; i >= 0; i--)
        {
            if (attractedItems[i] != null)
            {
                var floating = attractedItems[i].GetComponent<Floating>();
                if (floating != null)
                {
                    floating.isFloating = false; // 아이템이 움직일 때는 떠다니지 않도록 설정
                }
                // 아이템별 현재 속도 갱신 (가속 적용)
                if (!currentSpeeds.TryGetValue(attractedItems[i], out float speed))
                {
                    speed = moveSpeed;
                }
                speed += accelerationRate * Time.deltaTime;
                if (maxSpeed > 0.0f && speed > maxSpeed)
                {
                    speed = maxSpeed;
                }
                currentSpeeds[attractedItems[i]] = speed;

                attractedItems[i].position = Vector2.MoveTowards(
                    attractedItems[i].position,
                    transform.position,
                    speed * Time.deltaTime
                );
            }
            else
            {
                // 아이템이 파괴된 경우 리스트에서 제거
                attractedItems.RemoveAt(i);
            }
        }
        // 속도 상태 테이블 정리: null 이거나 리스트에 없는 키 제거
        if (currentSpeeds.Count > 0)
        {
            var toRemove = new List<Transform>();
            foreach (var kvp in currentSpeeds)
            {
                if (kvp.Key == null || !attractedItems.Contains(kvp.Key))
                {
                    toRemove.Add(kvp.Key);
                }
            }
            for (int r = 0; r < toRemove.Count; r++)
            {
                currentSpeeds.Remove(toRemove[r]);
            }
        }
    }

    // 트리거 범위에 아이템이 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item") && !attractedItems.Contains(other.transform))
        {
            attractedItems.Add(other.transform);
            // 초기 속도 설정
            currentSpeeds[other.transform] = moveSpeed;
        }
    }
}