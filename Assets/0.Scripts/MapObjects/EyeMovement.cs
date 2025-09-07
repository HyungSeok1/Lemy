using UnityEngine;

public class EyeMovement : MonoBehaviour
{
    [Tooltip("이동 속도")]
    public float moveSpeed = 5f;
    [Tooltip("영역 제한 기준이 되는 Eye 오브젝트 (SpriteRenderer가 있어야 함)")]
    public GameObject Eye;

    private Transform playerTransform;
    private SpriteRenderer eyeSpriteRenderer;
    private Bounds eyeBounds;

    void Start()
    {
        // 플레이어 태그를 가진 오브젝트를 찾아서 이동 대상로 설정
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("ConstrainedMovementToPlayer: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");

        // Eye 오브젝트의 SpriteRenderer를 가져옵니다.
        if (Eye != null)
        {
            eyeSpriteRenderer = Eye.GetComponent<SpriteRenderer>();
            if (eyeSpriteRenderer == null)
                Debug.LogError("ConstrainedMovementToPlayer: Eye 오브젝트에 SpriteRenderer가 없습니다!");
        }
        else
        {
            Debug.LogError("ConstrainedMovementToPlayer: Eye 오브젝트가 할당되지 않았습니다!");
        }
    }

    void Update()
    {
        if (playerTransform == null || eyeSpriteRenderer == null)
            return;

        // Eye 오브젝트의 SpriteRenderer의 bounds를 매 프레임 업데이트 (Eye가 움직일 경우에 대비)
        eyeBounds = eyeSpriteRenderer.bounds;

        // 현재 위치에서 플레이어 방향으로 이동할 방향 계산 (정규화)
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // 이동할 새 위치 계산
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        // 새 위치를 Eye의 SpriteRenderer bounds 안으로 제한
        newPosition.x = Mathf.Clamp(newPosition.x, eyeBounds.min.x, eyeBounds.max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, eyeBounds.min.y, eyeBounds.max.y);
        newPosition.z = transform.position.z;  // z축은 그대로 유지

        transform.position = newPosition;
    }
}