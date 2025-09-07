using UnityEngine;

/// <summary>
/// 작은 열쇠 생성기 (원 범위)
/// </summary>
public class SmallKeyMaker : MonoBehaviour
{
    [SerializeField] private GameObject smallKeyPrefab;      // 생성할 SmallKey 프리팹
    [SerializeField] private GameObject keyPrefab;      // 생성할 Key 프리팹

    [SerializeField] private int totalKeys = 5;         // 생성할 총 개수
    [SerializeField] private float radius = 5f;         // 생성 반경

    private int collectedCount = 0;   // 먹힌 키 개수

    void Start()
    {
        SpawnNextKey();
    }

    public void OnKeyCollected()
    {
        collectedCount++;

        if (collectedCount < totalKeys)
        {
            SpawnNextKey();
        }
        else
        {
            TriggerFinalEvent();
        }
    }

    private void SpawnNextKey()
    {
        Vector3 randomPos = GetRandomPosition();
        GameObject key = Instantiate(smallKeyPrefab, randomPos, Quaternion.identity);

        // SmallKey가 자신을 만든 Maker를 알 수 있도록 참조 전달
        SmallKey keyScript = key.GetComponent<SmallKey>();
        keyScript.maker = this;
    }

    private Vector3 GetRandomPosition()
    {
        // 원 안에서 랜덤 위치 (극좌표 사용)
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float r = Mathf.Sqrt(Random.Range(0f, 1f)) * radius; // 반지름 분포 균일
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);

        return transform.position + new Vector3(x, y, 0f);
    }

    private void TriggerFinalEvent()
    {
        Debug.Log("모든 작은 열쇠를 먹음! 이벤트 발생!");
        collectedCount = 0;

        // 자식에 KeyPrefab이 있는지 확인
        Transform existingKey = transform.Find("Key"); // 이름으로 찾는 방법
        if (existingKey != null)
        {
            existingKey.gameObject.SetActive(true);
        }
        else
        {
            // 못 찾았으면 새로 생성하고 부모로 붙여줌
            GameObject key = Instantiate(keyPrefab, transform.position, Quaternion.identity, transform);
            key.name = "Key"; // 다음에 찾을 수 있게 이름 고정
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // 원 형태를 Gizmos로 표시 (간단하게 30개 선분)
        int segments = 30;
        float angleStep = 360f / segments;
        Vector3 prevPoint = transform.position + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = transform.position + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0f);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
