using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class LightningZoneController : MonoBehaviour
{
    [Header("참조")]
    [Tooltip("번개가 실제로 떨어질 위치(구역 내부 빈 자식)")]
    public Transform spawnMarker;          // 필수 — Inspector에서 지정
    [Tooltip("프리팹")]
    public GameObject lightningPrefab;        // 실제 데미지 번개
    public GameObject lightningWarnerPrefab;  // 깜빡이는 경고 번개 ← 새로 추가

    [Header("설정")]
    public float spawnInterval = 1.2f;   // 번개 주기
    [Range(0f, 90f)]
    public float minTiltFromHorizontal = 15f;    // 수평 ±금지 각도(°)

    public float warningDuration = 1f;

    /* 내부 상태 ------------------------------------------------------ */
    Transform player;        // Player 태그 오브젝트
    bool playerInside;  // 플레이어가 현재 구역 안?
    float timer;         // 번개 주기 타이머

    /*------------------------------*/
    void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null)
        {
            Debug.LogError("<LightningZone> 'Player' 태그 오브젝트를 찾을 수 없습니다.");
            enabled = false;
            return;
        }
        player = p.transform;

        if (spawnMarker == null)
        {
            Debug.LogError("<LightningZone> SpawnMarker(빈 자식)를 Inspector에 지정하세요.");
            enabled = false;
        }
    }

    /*------------------------------*/
    void Update()
    {
        if (!playerInside) return;

        spawnMarker.position = player.position;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            if (lightningWarnerPrefab == null || lightningPrefab == null) return;
            StartCoroutine(SpawnLightning());
            timer = 0f;
        }
    }

    /* 트리거 판정 --------------------------------------------*/
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInside = true;
            timer = 0f;               // 새로 들어올 때부터 주기 계산
            spawnMarker.position = player.position; // 즉시 스냅
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0f;               // 주기 초기화
        }
    }

    /* 번개 대신 'Warner' 먼저 소환 ------------------------------------*/
    IEnumerator SpawnLightning()
    {
        // 각도 결정(수평 ±minTilt 금지)
        float angle;
        float fromHorizontal;
        do
        {
            angle = Random.Range(0f, 360f);
            fromHorizontal = Mathf.Abs(Mathf.DeltaAngle(angle, 0f));
        } while (fromHorizontal < minTiltFromHorizontal ||
                 fromHorizontal > 180f - minTiltFromHorizontal);

        Vector3 spawnPos = spawnMarker.position;

        // 워너 프리팹을 소환
        var warnObj = Instantiate(
            lightningWarnerPrefab,
            spawnPos,
            Quaternion.Euler(0f, 0f, angle));

        /* 워너에게 실제 번개 프리팹을 넘겨 줌 ----------------------*/
        var warn = warnObj.GetComponent<LightningWarner>();
        warn.warningDuration = warningDuration;
        warn.StartWarning();

        yield return new WaitForSeconds(warningDuration);

        var lightning = Instantiate(lightningPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        SoundManager.Instance.PlaySFXAt("lightning3", spawnMarker.position, 0.1f);
    }
}
