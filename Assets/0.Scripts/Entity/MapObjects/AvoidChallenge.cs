using UnityEngine;

/// <summary>
/// 작은 열쇠 생성기 (원 범위)
/// 번개생성은 여기서 안함
/// 키 생성만 해주는거. 이름만 AvoidChallenge임 (번개피하기니까)
/// </summary>
public class AvoidChallenge : ChallengeZone
{
    [SerializeField] private GameObject smallKeyPrefab;      // 생성할 SmallKey 프리팹
    [SerializeField] private GameObject keyPrefab;      // 생성할 Key 프리팹

    [SerializeField] private int totalKeys = 5;         // 생성할 총 개수
    [SerializeField] private float radius = 5f;         // 생성 반경

    private int collectedCount = 0;   // 먹힌 키 개수

    public override void Challenge()
    {
        base.Challenge();
        SpawnSmallKey();
    }

    public override void BeatChallenge()
    {
        base.BeatChallenge();
    }

    private void SpawnSmallKey()
    {
        Vector3 randomPos = GetRandomPosition();

        GameObject smallKey = Instantiate(smallKeyPrefab, randomPos, Quaternion.identity);
        SmallKey keyScript = smallKey.GetComponent<SmallKey>();
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

    private void SpawnKey()
    {
        collectedCount = 0;

        GameObject key = Instantiate(keyPrefab, transform.position, Quaternion.identity, transform);
        key.SetActive(true);
        key.GetComponent<Key>().OnGetKey += BeatChallenge;
    }

    public void OnKeyCollected()
    {
        collectedCount++;

        if (collectedCount < totalKeys)
        {
            SpawnSmallKey();
        }
        else
        {
            SpawnKey();
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
