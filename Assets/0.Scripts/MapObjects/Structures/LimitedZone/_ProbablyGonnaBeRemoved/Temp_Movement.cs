using UnityEngine;

/// <summary>
/// 
/// 테스트용 임시 스크립트. 삭제해도 무방
/// 
/// </summary>
public class Temp_Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    private void Start()
    {
        Debug.LogWarning("상하좌우키로이동");
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical");   

        Vector3 move = new Vector3(h, v, 0).normalized;
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
