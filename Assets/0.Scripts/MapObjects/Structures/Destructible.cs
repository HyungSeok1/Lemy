using UnityEngine;

/// <summary>
/// 플레이어가 닿으면 스택 1개를 소모하며 벽을 부수며 그대로 나아가는 구조물에 붙는 클래스입니다
/// 
/// 
/// </summary>
public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject[] debrisPrefabs; // 잔해 프리팹 배열
    [SerializeField] private int debrisCount = 8; // 총 잔해 개수
    [SerializeField] private float explosionForce = 5f; // 폭발 힘

    private int debrisIndex = 0; // 현재 생성할 잔해 인덱스

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1) Fast tag check
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // 2) Get the component just once
        if (collision.gameObject.TryGetComponent<Player>(out var pc) && pc.stackSystem.CurrentStackCount >= 1f)
        {
            Crash();
            pc.stackSystem.RemoveStackGauge(25);
        }
    }

    public void Crash()
    {
        this.gameObject.SetActive(false);


        // 잔해 프리팹 확인
        if (debrisPrefabs == null || debrisPrefabs.Length == 0)
        {
            Debug.LogError("debrisPrefabs 배열이 비어 있습니다. Unity Editor에서 프리팹을 할당해주세요.");
            return;
        }

        // 게임 오버 상태일 때만 폭발 파티클 효과 실행


        for (int i = 0; i < debrisCount; i++)
        {
            // 잔해 인스턴스 생성
            GameObject debris = Instantiate(debrisPrefabs[debrisIndex], transform.position, Quaternion.identity);

            // 순환 방식으로 잔해 종류 선택
            debrisIndex = (debrisIndex + 1) % debrisPrefabs.Length;

            // Rigidbody2D가 없으면 추가
            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = debris.AddComponent<Rigidbody2D>();
            }

            // 중력 영향 제거
            rb.gravityScale = 0f;
            rb.linearDamping = 2f;

            // 폭발 방향 계산 및 적용
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDir * explosionForce, ForceMode2D.Impulse);

            // 잔해 관리 스크립트 추가
            Debris debrisScript = debris.AddComponent<Debris>();
        }
    }



    public class Debris : MonoBehaviour
    {
        [SerializeField] private float fadeOutDuration = 1f; // 투명해지는 데 걸리는 시간
        private SpriteRenderer spriteRenderer;
        private Color initialColor;

        private void Start()
        {
            // SpriteRenderer 컴포넌트 가져오기
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // 초기 색상 저장
                initialColor = spriteRenderer.color;
                // 투명화 코루틴 실행
                StartCoroutine(FadeOutAndDestroy());
            }
            else
            {
                Debug.LogWarning("SpriteRenderer가 없습니다!");
            }
        }

        private System.Collections.IEnumerator FadeOutAndDestroy()
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeOutDuration)
            {
                // 색상의 알파 값 감소
                float alpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / fadeOutDuration);
                spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 완전히 투명해지면 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}


