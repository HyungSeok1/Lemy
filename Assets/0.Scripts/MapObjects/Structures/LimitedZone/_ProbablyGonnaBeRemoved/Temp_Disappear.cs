using UnityEngine;

/// <summary>
/// 테스트용 임시 스크립트. 삭제해도 무방
/// </summary>
public class Temp_Disappear : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌한 경우 삭제 (태그로 확인하거나 생략 가능)
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

