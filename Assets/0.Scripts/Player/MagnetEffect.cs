using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 자석 효과를 구현하는 스크립트
/// </summary>
public class MagnetEffect : MonoBehaviour
{
    public float moveSpeed = 10.0f; // 아이템이 끌려오는 속도
    public float detectionRadius = 5.0f; // 감지 반경
    private List<Transform> attractedItems = new List<Transform>();

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
                attractedItems[i].GetComponent<Floating>().isFloating = false; // 아이템이 움직일 때는 떠다니지 않도록 설정
                attractedItems[i].position = Vector2.MoveTowards(
                    attractedItems[i].position,
                    transform.position,
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                // 아이템이 파괴된 경우 리스트에서 제거
                attractedItems.RemoveAt(i);
            }
        }
    }

    // 트리거 범위에 아이템이 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item") && !attractedItems.Contains(other.transform))
        {
            attractedItems.Add(other.transform);
        }
    }

    // 트리거 범위에서 아이템이 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            attractedItems.Remove(other.transform);
        }
    }
}