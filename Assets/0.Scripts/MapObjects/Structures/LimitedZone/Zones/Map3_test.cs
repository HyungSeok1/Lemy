using UnityEngine;
using System.Collections;


public class Map3_test : BaseZone
{
    // [SerializeField] private Vector2 zoneCenterPos;
    [SerializeField] private Player player;
    [SerializeField] private Collider2D limitedZoneRange;
    [SerializeField] private GameObject door;

    private void Start()
    {
        // if (zoneCenterPos == null) { zoneCenterPos = transform.position; } 
        if (player == null) {player = Player.Instance;}
        if (limitedZoneRange == null) { limitedZoneRange = transform.Find("zoneRange").GetComponent<Collider2D>(); }
        if (door == null) { door = transform.Find("3_parallax2").gameObject; }
    }
    private void Update()
    {
        
    }

    protected override IEnumerator OnStart() // 제약구간 시작 부분
    {
        StartCoroutine(Spin(door, 3f, 22.5f));
        Debug.LogWarning("Map3: 시작 준비 중");
        yield return new WaitForSeconds(3f);
        Debug.LogWarning("Map3: 시작");
        yield break;
    }

    protected override IEnumerator OnEnd()
    {
        Debug.Log("Map3: 종료 처리 중");
        yield return new WaitForSeconds(3f);
        Debug.Log("Map3: 종료");
        StartCoroutine(Spin(door, 3f, -22.5f));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Spin(door, 3f, 22.5f));
            TryExecute();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Spin(door, 3f, -22.5f));
            TryExecute();
        }
    }

    IEnumerator Spin(GameObject obj, float duration, float rotationAmount)
    {
        float elapsed = 0f;
        float startZ = obj.transform.localEulerAngles.z;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentZ = startZ + rotationAmount * t;
            obj.transform.localRotation = Quaternion.Euler(0f, 0f, currentZ);
            yield return null;
        }

        obj.transform.localRotation = Quaternion.Euler(0f, 0f, startZ + rotationAmount);
    }

}
