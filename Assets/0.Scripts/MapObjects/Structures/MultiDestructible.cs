using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MultiDestructible : MonoBehaviour
{
    [Header("벽 레이어들 (왼쪽부터 오른쪽 순서로)")]
    [SerializeField] private List<GameObject> wallLayers = new();

    [Header("이펙트 설정")]
    [SerializeField] private float flashDuration = 2f;
    [SerializeField] private Color flashColor = Color.white;

    private bool isProcessing = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isProcessing) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        if (!collision.gameObject.TryGetComponent<Player>(out var player)) return;

        isProcessing = true;
        int stack = player.stackSystem.GetCurrentStackCount();
        int layerCount = wallLayers.Count;

        // 플레이어가 어느 쪽에서 왔는지 계산 (방향성 기반)
        Vector2 hitDirection = (collision.GetContact(0).point - (Vector2)transform.position).normalized;
        bool fromLeft = Vector2.Dot(hitDirection, transform.right) < 0f;

        if (stack >= layerCount)
        {
            StartCoroutine(BreakAllWalls(player, collision));
        }
        else
        {
            StartCoroutine(FlashWalls(stack, collision));
        }
    }

    private IEnumerator FlashWalls(int count, Collision2D collision)
    {
        // Step 1: 충돌 지점에서 가장 가까운 벽을 찾음
        Vector2 contactPoint = collision.GetContact(0).point;
        int centerIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < wallLayers.Count; i++)
        {
            float distance = Vector2.Distance(wallLayers[i].transform.position, contactPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                centerIndex = i;
            }
        }

        // Step 2: 퍼질 범위 계산
        List<int> flashIndexes = new();

        // 좌우 균등하게 확산되도록 인덱스 선택
        int left = centerIndex;
        int right = centerIndex;

        flashIndexes.Add(centerIndex);
        for (int i = 1; i < count; i++)
        {
            if ((i % 2 == 1 && left > 0) || right >= wallLayers.Count - 1)
            {
                left--;
                flashIndexes.Add(left);
            }
            else if (right < wallLayers.Count - 1)
            {
                right++;
                flashIndexes.Add(right);
            }
        }

        // Step 3: SpriteRenderer 색상 변경
        List<SpriteRenderer> renderers = new();
        List<Color> originalColors = new();

        foreach (int idx in flashIndexes)
        {
            if (idx < 0 || idx >= wallLayers.Count) continue;
            var sr = wallLayers[idx].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                renderers.Add(sr);
                originalColors.Add(sr.color);
                sr.color = flashColor;
            }
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].color = originalColors[i];
        }

        isProcessing = false;
    }

    private IEnumerator BreakAllWalls(Player player, Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        int startIndex = 0;
        float minDistance = float.MaxValue;

        // Step 1: 접촉한 벽 인덱스 구하기
        for (int i = 0; i < wallLayers.Count; i++)
        {
            float distance = Vector2.Distance(wallLayers[i].transform.position, contactPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                startIndex = i;
            }
        }

        // Step 2: 접촉한 벽 기준으로 정방향 순차 파괴
        int layerCount = wallLayers.Count;

        for (int i = startIndex; i < layerCount; i++)
        {
            GameObject wall = wallLayers[i];
            if (wall != null)
            {
                wall.SetActive(false);
                player.stackSystem.RemoveStackGauge(25);
                yield return new WaitForSeconds(0.1f);
            }
        }

        for (int i = startIndex - 1; i >= 0; i--)
        {
            GameObject wall = wallLayers[i];
            if (wall != null)
            {
                wall.SetActive(false);
                player.stackSystem.RemoveStackGauge(25);
                yield return new WaitForSeconds(0.1f);
            }
        }

        Destroy(gameObject);
    }
}
