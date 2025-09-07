using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

/// <summary>
/// Trigger 공간에 들어오면 카메라 확대/축소 담당하는 클래스입니다.
/// 
/// </summary>
public class ActionTriggerZone : MonoBehaviour
{
    [SerializeField] private CinemachineCamera liveCam;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float normalSize;    // 평소 OrthographicSize
    [SerializeField] private float zoomedSize;   // 트리거 진입 시 목표 크기
    [SerializeField] private float zoomDuration = 2f; // 줌에 걸릴 시간(초)


    private BoxCollider2D box;
    private Rigidbody2D rb;
    private static Coroutine zoomCoroutine;
    private static ActionTriggerZone coroutineOwner;


    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        box.isTrigger = true;
        box.size = boxSize;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null)
        {
            Debug.LogWarning("메인 카메라에 CinemachineBrain이 없습니다.");
            return;
        }
        // 2) ActiveVirtualCamera를 VirtualCamera 타입으로 캐스트
        liveCam = brain.ActiveVirtualCamera as CinemachineCamera;
        if (liveCam == null)
        {
            Debug.LogWarning("현재 활성화된 VirtualCamera가 없습니다.");
            return;
        }


        if (!other.CompareTag("Player")) return;
        StartZoom(zoomedSize);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartZoom(normalSize);
    }

    private void StartZoom(float targetSize)
    {
        if (zoomCoroutine != null)
            coroutineOwner.StopCoroutine(zoomCoroutine);

        coroutineOwner = this;
        zoomCoroutine = StartCoroutine(DoZoom(targetSize));
    }

    private IEnumerator DoZoom(float targetSize)
    {
        var lens = liveCam.Lens;
        float startSize = lens.OrthographicSize;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / zoomDuration);
            liveCam.Lens = lens;
            yield return null;
        }

        lens.OrthographicSize = targetSize;
        liveCam.Lens = lens;
    }
}
